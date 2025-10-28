------------------------- BringUp Control V2 SW ver 1.0.0.1 -----------------------------------
1. Fixed input values:
DAC & FPGA tabs: textboxes values no need to press Enter , just hit Write or read button it will
read textbox values and check it's validity.

2. QBD Set button - non relevant its functionality passed to comboBox MAINDAC_PAGE
operator can see register values each time when combobox values chnaged between DAC0 and DAC1
- button will be deleted next version.



------------------------- BringUp Control V2 SW ver 1.0.0.0 -----------------------------------
This SW controls EVB TX for LS BB bringup.

1. Setup:

- connect Power Supply 12 VDC to main connector.
- The Skyworks must be connected via flat cable and also be connected to main supply.
- The Skyworks must be programmed to 100MHZ.
- The bring up board must be connected with SPI adapter and via USB B type and PC.
- The FPGA board must be connected to SPI adapter and via USB B type to PC.
- The FPGA board via 12pin header must be connected to Adwark JTAG interface and must be connected to PC.

2. SW configuration:
- main configuration file location:(installation folder\ini_config.ini)
- it's has files for for configuration PLL, DAC, FPGA, Amplifiers.
- DAC configuration files can be loaded also via load ini file in its tab.
- separate kit of vector files must be added - TODO
- Vector player added in FPGA tab.

3. FPGA configuration
- version for fgpa upload to EVB is: 

4. SW Steps
- Start application , check if both FTDI cards detected
- select PLL tab: press RF PLL INIT button
- select DAC tab: 

- select DAC tab
***********************************************************************************