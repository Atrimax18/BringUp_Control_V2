using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BringUp_Control
{
    internal class PCAL6416A
    {
        //private const byte PCAL6416A_I2C_ADDRESS0 = 0x20; // Fixed I2C address
        private const byte PCAL6416A_I2C_ADDRESS = 0x21;
        private const byte INPUT_PORT_0 = 0x00; // Input port 0 register
        private const byte INPUT_PORT_1 = 0x01; // Input port 1 register
        private const byte OUTPUT_PORT_0 = 0x02; // Output port 0 register
        private const byte OUTPUT_PORT_1 = 0x03; // Output port 1 register
        private const byte CONFIG_PORT_0 = 0x06; // Configuration port 0 register
        private const byte CONFIG_PORT_1 = 0x07; // Configuration port 1 register

        


        public enum PinIndex
        {
            CTRL_SPI_CSN_SEL0 = 0,
            CTRL_SPI_CSN_SEL1,
            CTRL_HMC1119_LE1,
            CTRL_HMC1119_LE2,
            CTRL_HMC1119_LE3,
            CTRL_ADC_CONVST,
            CTRL_HMC8414_VCTRL1,
            CTRL_HMC8414_VCTRL2,
            CTRL_DAC_RSTn,
            CTRL_DAC_TXEN0,
            CTRL_DAC_TXEN1,
            CTRL_DAC_IRQn0,
            CTRL_DAC_IRQn1,
            CTRL_PLL_LKDET,
            CTRL_PLL_MUXOUT,
            CTRL_SPI_EN
        }

        public enum MuxSpiIndex
        {
            MUX_SPI_CSn_DAC = 0,
            MUX_SPI_CSn_PLL,
            MUX_SPI_CSn_ADC,
            MUX_SPI_CSn_SKY_PLL
        }

        private i2cDriver _ft;

        public void Init(i2cDriver ft)
        {
            _ft = ft;
        }

        public void ConfigurePin(byte pin, bool isOutput)
        {
            byte configRegister = pin < 8 ? CONFIG_PORT_0 : CONFIG_PORT_1;
            byte pinMask = (byte)(1 << (pin % 8));

            // Read the current configuration
            ReadByte(configRegister, out byte configValue);

            // Update the configuration for the specified pin
            if (isOutput)
            {
                configValue &= (byte)~pinMask; // Set pin as output (0)
            }
            else
            {
                configValue |= pinMask; // Set pin as input (1)
            }

            // Write the updated configuration back to the register
            WriteByte(configRegister, configValue);

            Console.WriteLine($"Pin {pin} configured as {(isOutput ? "Output" : "Input")}.");
        }
        public void ConfigurePort(byte port, byte stateMask)
        {
            if(port > CONFIG_PORT_1)
            {
                throw new ArgumentOutOfRangeException(nameof(port), "Port must be 0 or 1.");
            }

            // Write the port configuration to the register
            WriteByte(port, stateMask);

            // Output the state mask as a HEX number
            Console.WriteLine($"Port {port} config mask: 0x{stateMask:X2}.");
        }

        public void SetPinState(byte pin, bool value)
        {
            byte outputRegister = pin < 8 ? OUTPUT_PORT_0 : OUTPUT_PORT_1;
            byte pinMask = (byte)(1 << (pin % 8));

            // Read the current output state
            ReadByte(outputRegister, out byte outputValue);

            // Update the output state for the specified pin
            if (value)
            {
                outputValue |= pinMask; // Set pin high
            }
            else
            {
                outputValue &= (byte)~pinMask; // Set pin low
            }

            // Write the updated output state back to the register
            WriteByte(outputRegister, outputValue);

            Console.WriteLine($"Pin {pin} set to {(value ? "High" : "Low")}.");
        }

        public bool GetPinState(byte pin)
        {
            byte inputRegister = pin < 8 ? INPUT_PORT_0 : INPUT_PORT_1;
            byte pinMask = (byte)(1 << (pin % 8));

            // Read the current input state
            ReadByte(inputRegister, out byte inputValue);

            // Return the state of the specified pin
            bool pinState = (inputValue & pinMask) != 0;
            Console.WriteLine($"Pin {pin} is {(pinState ? "High" : "Low")}.");
            return pinState;
        }

        public void SetPortState(byte port, byte stateMask)
        {
            if (port > OUTPUT_PORT_1)
            {
                throw new ArgumentOutOfRangeException(nameof(port), "Port must be 0 or 1.");
            }
            // Write the port state to the register
            WriteByte(port, stateMask);
            // Output the state mask as a HEX number
            Console.WriteLine($"Port {port} state mask: 0x{stateMask:X2}.");
        }

        public byte GetPortState(byte port)
        {
            if (port > INPUT_PORT_1)
            {
                throw new ArgumentOutOfRangeException(nameof(port), "Port must be 0 or 1.");
            }
            // Read the current input state
            ReadByte(port, out byte portValue);
            // Output the state mask as a HEX number
            Console.WriteLine($"Port {port} state mask: 0x{portValue:X2}.");
            return portValue;
        }

        public void SetMuxSpiPin(MuxSpiIndex muxSpiIndex, bool value)
        {
            // Ensure the index is within the valid range (0-3)
            if ((byte)muxSpiIndex > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(muxSpiIndex), "MuxSpiIndex must be between 0 and 3.");
            }

            //First, enable the SPI mux
            SetPinState((byte)PinIndex.CTRL_SPI_EN, true);

            // Extract the 2-bit mask from the MuxSpiIndex
            byte muxMask = (byte)muxSpiIndex;
            byte clearMask = 0x03;
            // Set the state of CTRL_SPI_CSN_SEL0 and CTRL_SPI_CSN_SEL1 based on the mask
            //SetPinState((byte)PinIndex.CTRL_SPI_CSN_SEL0, (muxMask & 0b01) != 0); // LSB
            //SetPinState((byte)PinIndex.CTRL_SPI_CSN_SEL1, (muxMask & 0b10) != 0); // MSB

            // Read the current output state
            ReadByte(INPUT_PORT_0, out byte outputValue);

            outputValue &= (byte)~clearMask; // First clear the bits for the mux mask
            outputValue |= muxMask; // Set the masked bits

            // Write the updated output state back to the register
            WriteByte(INPUT_PORT_0, outputValue);

            Console.WriteLine($"TMUX1104 set to MUX_SPI_CSn_{muxSpiIndex} (Mask: 0b{Convert.ToString(muxMask, 2).PadLeft(2, '0')}).");
        }

        public void WriteByte(byte regAddr, in byte data)
        {
            if (_ft == null)
            {
                throw new InvalidOperationException("I2C driver is not initialized. Call Init() first.");
            }

            //ReadOnlySpan<byte> buff_wr = stackalloc byte[4] { regAddr, data, 0x00, 0x00 };
            ReadOnlySpan<byte> buff_wr = stackalloc byte[2] { regAddr, data };
            _ft.Write(PCAL6416A_I2C_ADDRESS, buff_wr);
        }
        

        private void ReadByte(byte regAddr, out byte data)
        {
            if (_ft == null)
            {
                throw new InvalidOperationException("I2C driver is not initialized. Call Init() first.");
            }

            ReadOnlySpan<byte> registerAddress = stackalloc byte[1] { regAddr };
            Span<byte> buff_rd = stackalloc byte[1];
            _ft.Write(PCAL6416A_I2C_ADDRESS, registerAddress);
            _ft.Read(PCAL6416A_I2C_ADDRESS, buff_rd);
            data = buff_rd[0];
        }  
        public void Led_Test()
        {
            RunLedChase();
        }      


        //GPIO_IN_OUT: 0 for output, 1 for input
        public void PCAL6416A_CONFIG_IO_EXP(int IO_Register, int GPIO_IN_OUT)
        {
            byte value;
            
            if (IO_Register < 6 || IO_Register > 7)
                throw new Exception("IO CONFIG REGISTER must be 6 or 7");
            else
            {
                ReadByte((byte)IO_Register, out value);

                if (value == 255)
                    WriteByte((byte)IO_Register, 0x00);
            }               
                        
        }       

        public void RunLedChase(int delayMs = 500)
        {
            // Turn ON LEDs one by one (active LOW: write 0 to turn ON)
            for (int i = 0; i < 8; i++)
            {
                byte val = (byte)~(1 << i);  // only one bit = 0, others = 1
                WriteByte(OUTPUT_PORT_0, val);
            
                Thread.Sleep(delayMs);
            }

            // All LEDs ON (all bits LOW)
            WriteByte(OUTPUT_PORT_0, 0x00);
            
            Thread.Sleep(500);

            // Turn OFF LEDs one by one (bit by bit go back to 1)
            for (int i = 0; i < 8; i++)
            {
                byte val = (byte)(0x00 | (1 << i)); // bit i = HIGH
                val = (byte)(val | ((1 << i) - 1)); // accumulate bits as OFF
                WriteByte(OUTPUT_PORT_0, val);
                ;
                Thread.Sleep(delayMs);
            }

            // Final: all OFF
            WriteByte(OUTPUT_PORT_0, 0xFF);
            
        }



        
    }
}
