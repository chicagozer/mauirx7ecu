## Mazda FD3S Diagnostic Tool

This is an attempt to build a diagnostic tool for the Mazda RX-7 FD (FD3S). It is based on the code from [Enervation] (https://www.rx7club.com/members/enervation-133206/) at rx7club.com and the code https://www.rx7club.com/3rd-generation-specific-1993-2002-16/data-interface-stock-ecu-901169/

The original work describing the command protocol is here - https://kaele-com.translate.goog/~kashima/car/rx7.html?_x_tr_sl=auto&_x_tr_tl=en&_x_tr_hl=en&_x_tr_pto=wapp#read

## How it works

The program connects to the diagnostic connector on the FD3S. The interface is pretty simple - there are 4 connections. 

PWR (B+) - 12V from the car.
GND - ground.
TEN - A signal line that requires a 20hz square wave 50% duty cycle.
FEN - The bidirectional async communication line running at 976 baud.

The car uses a 12V signal so there needs to be some hardware to convert TTL logic (5v) to 12V.

https://www.rx7club.com/attachments/3rd-generation-specific-1993-2002-16/385803-data-interface-stock-ecu-rx7-schem.png

The ECU is a Motorola 8-bit 68HC11. Seems to be shared with early 90s Miata and Ford Probe. Also known as MECS-II.

## The hardware

I built this board on a perfboard. As mentioned on the diagram there is optical isolation between the 12V side (car) and the 5V side (PC). The 5V side is powered by the USB connection.

I used this connector. https://www.amazon.com/dp/B0BJKCSZZW?ref=ppx_yo2ov_dt_b_fed_asin_title

You have some choices about how to send the 20hz square wave. You can either do this with hardware (adding a 555 timer chip) or let the software handle it by flipping a control line like DTR or RTS.

I have not found a good connector for the diagnostic port. The Mazda 17 pin to OBDII connectors available on Ebay/Amazon don't connect to the right pins.

## DOTNET Maui

I updated the original code to Maui. It seems to run fine on either Windows or Mac but unfortunately won't run on an iphone or android because the serial comms are not supported.

## TODO

Ideally I'd like to display the CEL diagnostic codes. This is available through the blinking light procedure but I have to think these codes are available through the serial communications.



## Resources

ECU pinouts - https://stocksray.com/ecupins.html
Temperature Sensor - https://kaele-com.translate.goog/~kashima/car/tempmeter/?_x_tr_sch=http&_x_tr_sl=ja&_x_tr_tl=en&_x_tr_hl=en
Original Schematic - https://kaele.com/~kashima/car/MAZDA_IF.pdf?_x_tr_sl=ja&_x_tr_tl=en&_x_tr_hl=en&_x_tr_sch=http
ECU Commands - https://kaele-com.translate.goog/~kashima/car/rx7.html?_x_tr_sl=auto&_x_tr_tl=en&_x_tr_hl=en&_x_tr_pto=wapp#read
Data sheet for 68HC11 - https://www.nxp.com/docs/en/data-sheet/M68HC11E.pdf
Error Codes - https://www.banzai-racing.com/FD&S5_error_codes.htm
