
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace BringUp_Control
{
    internal class Si5518
    {

        private SpiDriver _spi;

        byte[] fullRequest; //used to hold the full request to be sent over SPI/I2C

        public void Init(SpiDriver spi)
        {
            _spi = spi ?? throw new ArgumentNullException(nameof(spi), "SPI driver cannot be null.");
        }

        // Private Methods

        private byte[] SerialTransferWithHeaderPrepending(byte[] bytesToSend, int numBytesToRead)
        {
            byte[] SPICommandHeader = { 0xC0 };
            fullRequest = SPICommandHeader.Concat(bytesToSend).ToArray();
            byte[] dataRx;

            //this data buffer size should be at least numBytesToRead
            //send the assembled byte stream over the serial bus to the Skyworks device.
            //The number of bytes read back should be a least 3 bytes to capture error codes if generated.
            // The reply is an error if FWERR, APIERR, or HWERR bits are set.
            numBytesToRead = Math.Max(numBytesToRead, 3);
            //the 1st bit is the CTS (clear-to-send) bit, so while it's 0, try again.
            int retryCounter = 0;
            do
            {
                byte[] SPIReplyHeader = { 0xD0 };
                byte[] dummyDataForReply = new byte[numBytesToRead]; //defaults to 0's
                fullRequest = (byte[])SPIReplyHeader.Concat(dummyDataForReply);
                //the function below should send the header, then without releasing CSb,
                //continue to clock the SCLK line for the number of bytes in dummyDataForReply
                //and read from the Skyworks device while doing so. If all is well, the reply
                //should start with 0x80 to indicate CTS (clear to send) and no errors.
                dataRx = new byte [fullRequest.Length];
                _spi.TransferFullDuplex(fullRequest, dataRx);
                //check for any errors reported by the Skyworks device related to the command
                //errors are indicated if the 2nd through 4th bits (from MSB side) contains a '1'
                //recall that the code above enforces that the reply length is at least 3 bytes.
                //API and some FWERRs will have attached error codes, check READ_REPLY's documentation
                //to decode the error code. FW and HWERRs are sticky bits, they will not clear unless
                //the device is power cycled.
                //if CTS is valid and (HW/FW/APIERR are set)
                if ((dataRx[0] >> 7 == 1) && (dataRx[0]>>4) > 0x8 && dataRx[0] != 0xFF)
                {
                    Console.WriteLine(@"APIERR/FWERR/HWERR encountered. STATUS= "+
                    dataRx[0].ToString("X2") + " " +
                    "API/FWERR code may be " +
                    dataRx[2].ToString("X2") +" " +
                    dataRx[1].ToString("X2"));
                    Console.WriteLine("Resolve any and all errors before continuing");
                }
                if(dataRx[0]>>8 != 1)
                {
                    retryCounter = retryCounter + 1;
                    Console.WriteLine("Retrying command, CTS=0. " + retryCounter);
                }
            } while (dataRx[0] >> 7 == 0);

            return dataRx; //this data should be of size numBytesToRead
        }

        private void check_for_CTS()
        {
            // CTS is the first bit of the reply, so if it is 0, wait until it is 1.
            // this command will not return until CTS is asserted.

            byte[] dummy = new byte[0];

            SerialTransferWithHeaderPrepending(dummy, 1);

            Console.WriteLine("check_for_CTS() succeeded. The clock device is ready" +
            "to accept commands.");
        }

        private void SIO_TEST()
        {
            // Sends the SIO_TEST command (0x01) to get two bytes 0xABCD echoed
            byte[] sio_test_request = { 0x01, 0xAB, 0xCD };
            var ret = SerialTransferWithHeaderPrepending(sio_test_request, 4);
            printByteArray(ret);
            //ret should contain 0x80 0x01 0xAB 0xCD
        }

        private int SIO_INFO()
        {
            // Sends the SIO_INFO command (0x02) to get the command buffer size
            byte[] sio_info_request = { 0x02 };
            var ret = SerialTransferWithHeaderPrepending(sio_info_request, 5);
            // This following line prints out 0x80 0x00 0x04 0x40 0x00
            Console.Write("SIO_INFO reply:");
            // combine the individual bytes into an int for the command buffer size.
            // Don't care about the reply buffer size, throw it away for now.
            int CMD_BUFFER_SIZE = (ret[2] << 8) + ret[1];
            Console.WriteLine("Command buffer size from SIO_INFO:" + CMD_BUFFER_SIZE);
            return CMD_BUFFER_SIZE;
        }

        private int SIO_INFO_reply_buffer_size()
        {
            // Sends the SIO_INFO command (0x02) to get the reply buffer size
            byte[] sio_info_request = { 0x02 };
            var ret = SerialTransferWithHeaderPrepending(sio_info_request, 5);
            // This following line prints out 0x80 0x00 0x04 0x40 0x00
            Console.Write("SIO_INFO reply:");
            // combine the individual bytes into an int for the reply buffer size.
            // Don't care about the command buffer size, throw it away for now.
            int REPLY_BUFFER_SIZE = (ret[4] << 8) + ret[3];
            Console.WriteLine("Reply buffer size from SIO_INFO:" + REPLY_BUFFER_SIZE);
            return REPLY_BUFFER_SIZE;
        }

        private bool DEVICE_INFO()
        {
            // Get device info, which includes the device part number and grade.
            // See device API documentation for explaination of the bytes sent.
            byte[] device_info_request = { 0x08 };
            var ret = SerialTransferWithHeaderPrepending(device_info_request, 14);
            //check that the part number matches Si5518 grade E part.
            return (ret[2] == 0x55 & ret[1] == 0x18) & (ret[3] == 'E');
        }

        private void RESTART()
        {
            // RESTART by writing raw bytes
            // Restarts the clock device and wait in bootloader mode to accept new firmware.
            byte[] restart_request = { 0xF0, 0x00 };
            SerialTransferWithHeaderPrepending(restart_request, 1);
            Console.WriteLine("RESTART Sent.");
        }

        private void HOST_LOAD(string filepath, int CMD_BUFFER_SIZE)
        {
            // HOST_LOAD splits the input file (binary format) into chunks, then sends
            // individual HOST_LOAD firmware API commands to the clock device to load the
            // contents of the file into the device RAM.
            // open the file on the computer and put everything into a byte array.
            var file_contents = File.ReadAllBytes(filepath);
            // calculate the chunk size.
            // SPI: Minus two because of the two fixed cmd/reply header bytes.
            // I2C: Minus four because of address and header bytes.
            // example below uses – 4 to account for both cases.
            int chunkSize = CMD_BUFFER_SIZE - 4;
            int thisChunkSize = chunkSize;
            // calculate the number of serial transfer transactions that need to be done
            var numberOfChunks = (file_contents.Length / chunkSize) + 1;
            Console.WriteLine("HOST_LOAD " + filepath);
            // loop enough times to write the entire input file to the clock device in smaller chunks.
            for (int chunkNum = 0; chunkNum < numberOfChunks; ++chunkNum)
            {
                Console.Write(chunkNum + " ");
                // last chunk can be smaller than CMD_BUFFER_SIZE
                if (chunkNum == numberOfChunks - 1)
                {
                    thisChunkSize = file_contents.Length % chunkSize;
                }
                // Note: SubArray(startIndex, numBytes)
                byte[] thisChunk = file_contents.AsSpan(chunkNum * chunkSize, thisChunkSize).ToArray();
                byte[] host_load_command = { 0x05 };
                // The command size buffer limit should include the two bytes sent above.
                // Example: buffer limit = 1024 bytes, the max HOST_LOAD chunk length
                // should be <= 1022 bytes (SPI) or 1020 bytes (I2C).
                host_load_command = host_load_command.Concat(thisChunk).ToArray();
                // command request as per the documentation
                var ret = SerialTransferWithHeaderPrepending(host_load_command, 1);
                //optionally print out each byte that is being written.
                printByteArray(host_load_command);
                // check for CTS before continuing
                check_for_CTS();
            }
        }

        private void NVM_LOAD_DATA(string filepath, int CMD_BUFFER_SIZE)
        {
            // Identical to HOST_LOAD except command number
            // open the file on the computer and put everything into a byte array.
            var file_contents = File.ReadAllBytes(filepath);
            // calculate the chunk size.
            // SPI: Minus two because of the two fixed cmd/reply header bytes.
            // I2C: Minus four because of address and header bytes.
            // example below uses – 4 to account for both cases.
            int chunkSize = CMD_BUFFER_SIZE - 4;
            int thisChunkSize = chunkSize;
            // calculate the number of serial transfer transactions that need to be done
            var numberOfChunks = (file_contents.Length / chunkSize) + 1;
            Console.WriteLine("NVM_LOAD_DATA " + filepath);
            // loop enough times to write the entire input file to the clock device in smaller chunks.
            for (int chunkNum = 0; chunkNum < numberOfChunks; ++chunkNum)
            {
                Console.Write(chunkNum + " ");
                // last chunk can be smaller than CMD_BUFFER_SIZE
                if (chunkNum == numberOfChunks - 1)
                {
                    thisChunkSize = file_contents.Length % chunkSize;
                }
                // Note: SubArray(startIndex, numBytes)
                //byte[] thisChunk = file_contents.SubArray(chunkNum * chunkSize, thisChunkSize);

                byte[] thisChunk = file_contents.AsSpan(chunkNum * chunkSize, thisChunkSize).ToArray();

                byte[] nvm_load_data_command = { 0xF1 };
                // The command size buffer limit should include the two bytes sent above.
                // Example: buffer limit = 1024 bytes, the max NVM_LOAD_DATA chunk length
                // should be <= 1022 bytes (SPI) or 1020 bytes (I2C).
                nvm_load_data_command = nvm_load_data_command.Concat(thisChunk).ToArray();
                // command request as per the documentation
                var ret = SerialTransferWithHeaderPrepending(nvm_load_data_command, 1);
                //optionally print out each byte that is being written.
                printByteArray(nvm_load_data_command);
                // check for CTS before continuing
                check_for_CTS();
            }
            Console.WriteLine("NVM_LOAD_DATA compleed for " + filepath);
        }

        private void BOOT()
        {
            // Boots the frequency plan and firmware previously loaded into RAM by HOST_LOAD
            Console.WriteLine("Initiating BOOT.");
            byte[] boot_request = { 0x07 };
            var ret = SerialTransferWithHeaderPrepending(boot_request, 1);
            printByteArray(ret);
            Console.WriteLine("BOOT complete.");
        }

        private bool REFERENCE_STATUS()
        {
            // Checks the status of the reference clock PLLs.
            // Returns true if the PLLs are locked to the reference clock.
            Console.WriteLine("Checking REFERENCE_STATUS.");
            byte[] reference_status_request = { 0x16 };
            var ret = SerialTransferWithHeaderPrepending(reference_status_request, 5);
            // return true if reference PLL is locked, otherwise return false.
            // Check bits 1 and 2 for PLL lock status
            return (
                ret[0] == 0x80 &
                ret[1] == 0x00 &
                ret[2] == 0x00 &
                ret[3] == 0x00 &
                ret[4] == 0x00
            );
        }

        private void NVM_SPACE_AVALIABLE()
        {
            byte[] nvm_space_available_request = { 0xF6 };
            var ret = SerialTransferWithHeaderPrepending(nvm_space_available_request, 4);
            //Byte 0 is the STATUS byte (should be 0x80)
            var space_available = ((int)ret[2] << 8) + ret[1];
            var entries_available = ret[3];
            Console.WriteLine("NVM_SPACE_AVAILABLE:" + space_available);
            Console.WriteLine("NVM entries available:" + entries_available);
            Console.WriteLine("Check the space and entries available against the readme.");
            Console.WriteLine("NVM_SPACE_AVAILABLE Request Complete.");
        }

        private int NVM_FPLAN_READ_INIT(int bufSize)
        {
            //get reply buffer size, then subtract 3 bytes for STATUS byte and BYTE_COUNT.
            byte[] nvm_fplan_read_init_request = { 0xF8, (byte) bufSize, (byte) (bufSize>>8) };
            var ret = SerialTransferWithHeaderPrepending(nvm_fplan_read_init_request, 3);
            check_for_CTS();
            return ((int)ret[2] << 8) + ret[1]; //number of bytes to read back.
        }

        private void NVM_FPLAN_READ(int extractSize, string filepath, int bufSize)
        {
            byte[] nvm_fplan_read_request = { 0xF9 };
            int numChunks = (int) Math.Ceiling((double) extractSize / (double) bufSize);
            //file object
            System.IO.BinaryWriter f = new System.IO.BinaryWriter(System.IO.File.Open(filepath, System.IO.FileMode.Create));
            for (int chunk = 1; chunk <= numChunks; chunk = chunk + 1)
            {
                var ret = SerialTransferWithHeaderPrepending(nvm_fplan_read_request, bufSize + 3);
                var numBytesToSave = ret.Length;
                if (chunk == numChunks)
                {
                    // last chunk may be smaller
                    numBytesToSave = (extractSize % bufSize)+3;
                }
                for (int i = 3; i < numBytesToSave; i = i + 1)
                {
                    f.Write(ret[i]);
                }
            }
            f.Close();
            Console.WriteLine("Finished writing extracted baseline NVM to " + filepath);
        }

        private void NVM_SPACE_AVAILABLE()
        {
            byte[] nvm_space_available_request = { 0xF6 };
            var ret = SerialTransferWithHeaderPrepending(nvm_space_available_request, 4);
            var SPACE_AVAILABLE = ret[2] << 8 + ret[1];
            var ENTRIES_AVAILABLE = ret[3];
            if(ENTRIES_AVAILABLE <= 0)
            {
                Console.WriteLine("Burn cannot proceed, there are no more directory entries remaining.");
            }
            Console.WriteLine(SPACE_AVAILABLE + " bytes available, with ENTRIES_AVAILABLE + directory entries available");
        }

        private void NVM_FPLAN_CRC()
        {
            byte[] nvm_fplan_crc_command = { 0xFA };
            var ret = SerialTransferWithHeaderPrepending(nvm_fplan_crc_command, 10);
            var baselineCRC = ((int)ret[9] << 24) + ((int)ret[8] << 16) + ((int)ret[7] << 8) + (int)ret[6];
            var activeCRC = ((int)ret[5] << 24) + ((int)ret[4] << 16) + ((int)ret[3] << 8) + (int)ret[2];
            var HAS_PATCH = (ret[1] & 0x02) >> 1;
            var HAS_BASE_FPLAN = (ret[1] & 0x01);
            Console.WriteLine("Baseline CRC = " + baselineCRC.ToString("X") + " " +
            "Active frequency plan CRC = " + activeCRC.ToString("X") + " " +
            "HAS_PATCH = " + HAS_PATCH + " " +
            "HAS_BASE_FPLAN = " + HAS_BASE_FPLAN);
            Console.WriteLine("Check the Baseline CRC against what CBPro reports to " +
            "ensure that the patch file was generated using the correct baseline " +
            "project file binary exports");
        }

        private void NVM_FPLAN_PATCH_INVALIDATE()
        {
            byte[] invalidate_command = { 0xF7, 0xDE, 0xC0 };
            var ret = SerialTransferWithHeaderPrepending(invalidate_command, 2);
            Console.WriteLine("Running NVM_FPLAN_PATCH_INVALIDATE. " +
            "Result: INVALIDATED = " + ((ret[1] & 0x02) >> 1) +
            "PATCHED (previously) = " + (ret[1] & 0x01));
        }

        private void NVM_BURN_VERIFY()
        {
            byte[] nvm_burn_verify_command = { 0xF2, 0xDE, 0xC0 };
            var ret = SerialTransferWithHeaderPrepending(nvm_burn_verify_command, 13);
            byte VERIFIED = ret[1];
            if (VERIFIED == 0x01)
            {
                Console.Write("NVM_BURN_VERIFY burn succeeded.");
            }
            else
            {
                Console.WriteLine("NVM_BURN_VERIFY failed. " +
                "Check for any HW/FW/API errors," +
                "and look at ERROR1CNT, ERROR2CNT, and the FILECRC");
            }
            Console.WriteLine("Full reply from NVM_BURN_VERIFY:");
            printByteArray(ret);
        }

        private void printByteArray(byte[] toPrint)
        {
            //loop through each byte in toPrint and print it out.
            foreach (byte i in toPrint)
            {
                Console.Write("{0:X} ", i);
            }
            Console.WriteLine();
        }

        // Public Methods

        public void Write_User_Configuration_And_Firmware_RAM()
        {
            // if RSTb was recently toggled from 0 -> 1, wait some time (check data sheet for minimums)
            // to allow time for the API to be ready for commands after hardware reset
            System.Threading.Thread.Sleep(1000);
            //Thread.Sleep(1000); // ms
            // before starting, check for CTS.
            check_for_CTS();
            // main procedure, complete Steps 1-3 before beginning here.
            // Step 4
            SIO_TEST();
            // Step 5
            var CMD_BUFFER_SIZE = SIO_INFO();
            // int CMD_BUFFER_SIZE = 252; //example fixed buffer size value for testing
            // Step 6
            RESTART();
            //I2C only: wait at least 5ms post-RESTART before sending any further commands.
            // Thread.Sleep(5);
            // Step 7
            var prod_fw_filepath = @".\prod_fw.boot.bin";
            var user_config_filepath = @".\user_config.boot.bin";
            HOST_LOAD(prod_fw_filepath, CMD_BUFFER_SIZE);
            HOST_LOAD(user_config_filepath, CMD_BUFFER_SIZE);
            // Step 8
            BOOT();
            // Step 9
            // Optionally wait some time for the reference PLL to lock. Lock time specifications are located in the datasheet.
            //Thread.Sleep(1000);
            var reference_locked = REFERENCE_STATUS();
            /*
            Instead of waiting a set amount of time, it is suggested to poll
            REFERENCE_STATUS until the command returns the results for a locked PLL.
            This should take no more than a few seconds if the reference
            oscillator is working.
            */
            while (reference_locked == false)
            {
                Console.WriteLine("Waiting for PLL to lock to XO/XTAL/VCXO");
                reference_locked = REFERENCE_STATUS();
                Thread.Sleep(500);
            }
        }

        public void NvmBaselineBurn()
        {
            //Steps 1 and 2, Perform as needed.
            //Step 3
            DEVICE_INFO();
            //Step 4
            RESTART();
            //Thread.Sleep(5); //Step 5a, I2C only. Wait at least 5ms
            //Step 5
            var CMD_BUFFER_SIZE = SIO_INFO();
            var nvm_burn_filepath = @".\nvm_burn_fw.boot.bin";
            //Step 6
            HOST_LOAD(nvm_burn_filepath, CMD_BUFFER_SIZE);
            //Step 7
            BOOT();
            //Step 8
            CMD_BUFFER_SIZE = SIO_INFO();
            //Step 9
            var fw_filepath = @"./prod_fw.burn.bin";
            NVM_LOAD_DATA(fw_filepath, CMD_BUFFER_SIZE);
            //Step 10
            NVM_BURN_VERIFY();
            //Step 11
            var fplan_filepath = @"./user_config.burn.bin";
            //Step 19
            NVM_BURN_VERIFY();
            //Step 20
            Console.WriteLine("Power cycle the device before continuing");
            //Step 21
            //Optionally check the CRC.
            NVM_FPLAN_CRC();
            //Step 22
            //optionally, check that the PLLs has locked to the XTAL/XO/VCXO
            // the available references depends on the device part number.
            REFERENCE_STATUS();
        }

        public void NvmPatchBurnExtractBaselinePlan()
        {
            // If RSTb was recently toggled from 0 -> 1, wait some time (check data sheet for minimums)
            // to allow time for the API to be ready for commands after hardware reset
            System.Threading.Thread.Sleep(1000);
            // Thread.Sleep(1000); // ms
            // Before starting, check for CTS.
            check_for_CTS();
            // NVM Patch Burn Extract
            // Steps 1 and 2 should be completed prior to starting this procedure. NVM Burn Step 3 was skipped, step 4 is DEVICE_INFO.
            if (!DEVICE_INFO())
            {
                Console.Write(@"Part number and device grade does not match the expected value, exiting.");
                while (true) { };
            }
            else
            {
                Console.Write(@"Part number and device grade matches what is expected, continuing.");
            }

            // Step 5
            RESTART();
            Thread.Sleep(5); //Step 5a, I2C only. Wait at least 5ms
            // Step 6
            var CMD_BUFFER_SIZE = SIO_INFO();
            // Step 7
            var nvm_burn_filepath = @".\nvm_burn_fw.boot.bin";
            HOST_LOAD(nvm_burn_filepath, CMD_BUFFER_SIZE);
            // Step 8
            BOOT();
            // Step 8A
            CMD_BUFFER_SIZE = SIO_INFO();
            // Step 9 - get REPLY (not COMMAND) buffer size
            int bufSize = SIO_INFO_reply_buffer_size() - 3;
            // Step 10
            var extractsize = NVM_FPLAN_READ_INIT(bufSize);
            // Step 11
            string whereToSaveExtract = @"C:\Users\ngk\Downloads\Baseline_extracted.bin";
            NVM_FPLAN_READ(extractsize, whereToSaveExtract, bufSize);
            Console.WriteLine("NVM Extract complete, file saved to " + whereToSaveExtract);
            // Step 12
            Console.WriteLine("Follow instructions to complete patch file export within ClockBuilder Pro.");
        }
        
    }
}
