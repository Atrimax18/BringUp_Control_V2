using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BringUp_Control
{
    internal class FPGA : IDisposable
    {

        private SpiDriver _ft;

        public void Init(SpiDriver ft)
        {
            _ft = ft;            
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

        
        

        public void Dispose()
        {
            _ft?.Dispose();
        }

    }
}
