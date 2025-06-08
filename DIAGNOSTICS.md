## Summary

Here are the diagnostic codes and the memory locations. Each diagnostic code is a bit in  

0x101, 0x103, 0x105, 0x107, 0x109, 0x10b, 0x10d 0x10f

the subsequent byte is the complement of the previous byte.

## Baseline

My car with no codes. (N3A1-MP*)  

Address 0100: 5A 00 FF 00 FF 00 FF 00 FF 00 FF 00 FF 00 FF 00  
Address 0110: FF 80 00 80 00 80 00 80 00 7F FF 7F FF 7F FF 7F  

## Test Bench ECU

On the test bench here are the codes thrown.  

- 02 crank position sensor NE (NOT THROWN) 4E 4H
- 03 Crank position sensor G (NOT THROWN) 4G 4H
- 05 knock sensor - 1m ohm  0b00100000 @ 0x101 3M 
- 09 water therm 1k resistor - pot 3E to ground  0b00010000 @ 0x103
- 11 intake air 1k resistor - pot 3L to ground 0b10000000 @ 0x105
- 12 throttle full .1-.7v  -  VREF 3I### 1K
- 13 pressure sensor VREF 3I + 1O
- 14 in ECU pressure (NOT THROWN)
- 15 O2 sensor - inactive (NOT THROWN) 3C
- 16 EGR sensor 3J-4D (NOT THROWN)
- 17 O2 sensor inverted (NOT THROWN)
- 18 throttle narrow  .75-1.25v VREF +  1K
- 20 Metering pump position sensor 3I 3A 4D (NOT THROWN)
- 23 fuel therm - 1k resistor - pot 1U 0b01000000 @ 0x105
- 25 sol valv pressure reg 4M  0b10000000 @ 0x109
- 26 oil metering stepper B+ 22k resistor 4I 4J 4K 4L
- 27 metering oil pump (NOT THROWN)
- 28 solenoid EGR 4O (NOT THROWN)
- 30 sol split air bypass 0b00000010 @ 0x109
- 31 sol relief 1 sec air bypass 3P 0b00001000 @109
- 32 sol second air switching 4N 0b00000100 @ 0x109 
- 33 sol valve port air bypass 3N (NOT THROWN)
- 34 sol idle speed ctl 4Q 0b00000010 @0x10d
- 37 oil metering pump (NOT THROWN)
- 38 sol accel warm up AWS 0b00000001 @0x109 4P
- 39 sol relief 2 3K  0b10000000 @0x10b   
- 40 sol purge 3H 0b01000000 @0x10b
- 42 sol precontrol 4V (NOT THROWN) 0b00100000 @0x10b ?? GUESS
- 43 sol waste gate 4U (NOT THROWN) 0b00010000 @0x10b ?? GUESS
- 44 sol turbo  0b00001000 @ 0x10b 4R
- 45 sol charge ctl 0b00000100  0x10b 4T
- 46 sol chare relief 4S (NOT THROWN) 0b00000010 @0x10b ?? GUESS
- 50 double throttle 3O (NOT THROWN)
- 51 fuel resistor relay resistance?? to B+ 200ohm
- 54 air pump relay 2J - to ground resistance??
- 71 injector front sec B+ 14ohm? try bigger 4X
- 73 inj rear sec B+ 14ohm? try bigger 4Z
- 76 slip lokup off signal (NOT THROWN) 2D 2C 2G
- 77 torque reduced signal (NOT THROWN) 2D 2C 2G
