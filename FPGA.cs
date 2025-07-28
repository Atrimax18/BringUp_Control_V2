﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BringUp_Control
{
    internal class FPGA : IDisposable
    {

        private SpiDriver _ft;
        private FtdiInterfaceManager _interfaceManager;

        private const int BC = 12; // Bit count for I/Q components
        private const int MaxRetries = 3;

        public Dictionary<string, DebuggerInstance> Debuggers = new Dictionary<string, DebuggerInstance>
        {
            ["uplink_modem"] = new DebuggerInstance("debugger_0_", 4, 8, 1024),
            ["uplink_DAC"] = new DebuggerInstance("debugger_1_", 2, 8, 1024),
            ["downlink_modem"] = new DebuggerInstance("debugger_2_", 4, 8, 1024),
            ["downlink_ADC"] = new DebuggerInstance("debugger_3_", 1, 20, 1024)
        };

        public void Init(SpiDriver ft, FtdiInterfaceManager interfaceManager)
        {
            _ft = ft ?? throw new ArgumentNullException(nameof(ft));
            _interfaceManager = interfaceManager ?? throw new ArgumentNullException(nameof(interfaceManager));

            _ft = _interfaceManager.GetSpi(); // Get current SPI interface
        }

        /// <summary>
        /// Sends a 32-bit write command over SPI.
        /// </summary>
        public void SpiWrite(uint address, uint data)
        {
            byte[] txBuffer = new byte[11];

            txBuffer[0] = 0x00; // Write command

            // Address: MSB first
            txBuffer[1] = (byte)((address >> 24) & 0xFF);
            txBuffer[2] = (byte)((address >> 16) & 0xFF);
            txBuffer[3] = (byte)((address >> 8) & 0xFF);
            txBuffer[4] = (byte)(address & 0xFF);

            // Data: MSB first
            txBuffer[5] = (byte)((data >> 24) & 0xFF);
            txBuffer[6] = (byte)((data >> 16) & 0xFF);
            txBuffer[7] = (byte)((data >> 8) & 0xFF);
            txBuffer[8] = (byte)(data & 0xFF);

            // Unused bytes
            txBuffer[9] = 0x00;
            txBuffer[10] = 0x00;

            _ft.Write(txBuffer);
        }

        /// <summary>
        /// Sends a 32-bit read command over SPI and returns the 32-bit register value from FPGA.
        /// </summary>
        public uint SpiRead(uint address)
        {
            byte[] txBuffer = new byte[11];
            byte[] rxBuffer = new byte[11];

            txBuffer[0] = 0x01; // Read command

            // Address: MSB first
            txBuffer[1] = (byte)((address >> 24) & 0xFF);
            txBuffer[2] = (byte)((address >> 16) & 0xFF);
            txBuffer[3] = (byte)((address >> 8) & 0xFF);
            txBuffer[4] = (byte)(address & 0xFF);

            // Bytes 5–10 are dummy (0x00)
            for (int i = 5; i < txBuffer.Length; i++)
                txBuffer[i] = 0x00;

            _ft.TransferFullDuplex(txBuffer, rxBuffer); // Perform full-duplex SPI transfer

            // Extract 32-bit result from bytes 6–9 (MSB first)
            uint result = (uint)(
                (rxBuffer[6] << 24) |
                (rxBuffer[7] << 16) |
                (rxBuffer[8] << 8) |
                rxBuffer[9]);

            return result;
        }

        //convert from uint to hex string
        public string UIntToHexValue(uint value)
        {
            return "0x" + value.ToString("X8");
        }

        //convert from uint to byte array
        public static byte[] UIntToByteArray(uint value)
        {
            return new byte[]
            {
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)(value & 0xFF)
            };
        }
        public uint ReadRegisterWithIntegrity(uint address)
        {
            var list = new List<uint>();
            for (int i = 0; i < MaxRetries; i++)
                list.Add(SpiRead(address));

            return FindMajority(list);
        }

        private uint FindMajority(List<uint> values)
        {
            return values.GroupBy(x => x)
                         .OrderByDescending(g => g.Count())
                         .First().Key;
        }

        public int ConvertFromComplex(Complex a)
        {
            int real = (int)Math.Round(a.Real);
            int imag = (int)Math.Round(a.Imaginary);
            int i = (real + (1 << BC)) % (1 << BC);
            int q = (imag + (1 << BC)) % (1 << BC);
            return (q << BC) | i;
        }

        public Complex ConvertToComplex(uint packed)
        {
            int i = (int)(packed & 0xFFF);
            if (i > 2047) i -= 4096;
            int q = (int)(packed >> BC);
            if (q > 2047) q -= 4096;
            return new Complex(i, q);
        }

        public Complex[] LoadVector(string path)
        {
            return File.ReadAllLines(path)
                       .Select(line => line.Trim().Replace("j", "i"))
                       .Where(line => !string.IsNullOrWhiteSpace(line))
                       .Select(s => ParseComplex(s))
                       .ToArray();
        }

        // Helper method to parse a string into a Complex number
        private static Complex ParseComplex(string s)
        {
            // Handles formats like "1.0+2.0i" or "1.0-2.0i"
            s = s.Replace(" ", "");
            int iIndex = s.LastIndexOf('i');
            if (iIndex < 0)
                throw new FormatException("Invalid complex format: missing 'i'.");

            string withoutI = s.Substring(0, iIndex);
            int plusIndex = withoutI.LastIndexOf('+');
            int minusIndex = withoutI.LastIndexOf('-');

            int splitIndex = Math.Max(plusIndex, minusIndex);
            if (splitIndex <= 0)
                throw new FormatException("Invalid complex format: missing '+' or '-' between real and imaginary.");

            string realPart = withoutI.Substring(0, splitIndex);
            string imagPart = withoutI.Substring(splitIndex);

            double real = double.Parse(realPart, System.Globalization.CultureInfo.InvariantCulture);
            double imag = double.Parse(imagPart, System.Globalization.CultureInfo.InvariantCulture);

            return new Complex(real, imag);
        }

        public void WriteToPlayerMemory(string debuggerKey, int stream, Complex[] samples)
        {
            if (!Debuggers.TryGetValue(debuggerKey, out DebuggerInstance dbg))
            {
                MessageBox.Show("Invalid debugger key: " + debuggerKey);
                return;
            }

            int count = samples.Length;
            if (count % dbg.Parallel != 0 || count / dbg.Parallel > dbg.MemLen)
            {
                MessageBox.Show("Invalid sample count: must be divisible by " + dbg.Parallel);
                return;
            }

            string prefix = dbg.Prefix;
            SpiWrite(StringToAddress(prefix + "rgf_set_active_channel"), (uint)stream);
            SpiWrite(StringToAddress(prefix + "rgf_player_count"), (uint)(count / dbg.Parallel));
            SpiWrite(StringToAddress(prefix + "rgf_set_wr_address"), 0);

            foreach (var c in samples)
            {
                int packed = ConvertFromComplex(c);
                SpiWrite(StringToAddress(prefix + "rgf_wr_sample"), (uint)packed);
            }
        }

        public void ActivatePlayer(string debuggerKey, bool play)
        {
            if (!Debuggers.TryGetValue(debuggerKey, out DebuggerInstance dbg))
            {
                MessageBox.Show("Invalid debugger key: " + debuggerKey);
                return;
            }
            SpiWrite(StringToAddress(dbg.Prefix + "rgf_activate_player"), play ? 1u : 0u);
        }

        public void StopPlayer(string debuggerKey)
        {
            ActivatePlayer(debuggerKey, false);
        }

        private uint StringToAddress(string regName)
        {
            // Placeholder mapping: in real usage, translate regName to actual address
            return 0x80000000; // Replace with real logic or lookup
        }
        public class DebuggerInstance
        {
            public string Prefix { get; set; }
            public int StreamCount { get; set; }
            public int Parallel { get; set; }
            public int MemLen { get; set; }

            public DebuggerInstance(string prefix, int streams, int par, int memlen)
            {
                Prefix = prefix;
                StreamCount = streams;
                Parallel = par;
                MemLen = memlen;
            }
        }


        public void Dispose()
        {
            _ft?.Dispose();
        }

    }
}
