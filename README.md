<img src="https://imgur.com/Sg0hfnB.png" width="300">

# Deadly Premonition Tool

A simple toolkit designed to making modding Deadly Premonition easier!

Virus Scan 1: https://www.virustotal.com/gui/file/34fef478eea2531a6aa20680ecd7992e0cb3a03543e894497945ae15a584330f

Virus Scan 2: https://www.virustotal.com/gui/file/d76b0038ccd91db93f98b31e12b7ed82cddb345fdb522f7751aa5eaf8003d2ed
## Acknowledgements

 - Zeddikins - Documented a lot of findings on steam:  
https://steamcommunity.com/app/247660/discussions/0/666824800729751880/
 - Thief1987 - Wrote QuickBMS scripts for DPSerial file struct which helped me learn the format.
 - HeliosAI - Wrote Noesis plugins for loading XMDs and XPCs which helped me learn the formats.

## Usage/Examples

```
UNPACK XPC -Allows the user to select XPC files and unpack the contents to a folder named "FILENAME_UNPACKED". Textures are automatically decompressed from zlib (best compression)
PACK XPC - Allows the user to select a .DDS file and reimport it into an XPC file. At the moment this only supports reimporting a single dds at a time but can repeated to add all modified textures. Textures are automatically recompressed with zlib (best compression)
UNPACK DPSERIAL - Allows the user to select a DPSerial file and unpack the contents to a folder called "programmer_pc". Generates an XML for reimporting later.
PACK DPSERIAL - Allows the user to select an XML file and an unpacked "programmer_pc" folder to repack into a DPSerial file. File size is derrived from the files themselves so don't bother editing the XML.

CURRENTLY DPSERIAL CAN'T BE A DIFFERENT SIZE WITHOUT CRASHING! ENSURE THAT ANY FILES REPLACED ARE SMALLER OR EQUAL TO THE SIZE OF THE ORIGINAL FILE.
SMALLER FILES WILL BE FILLED WITH 0'S TO MAKE UP THE DIFFERENCE. THIS CAN BE DISABLED WITH THE FILE SIZE FIX CHECKBOX BUT IT'S NOT RECCOMMENDED AND WILL LIKELY BREAK FILE LOADING MAKING THE GAME UNPLAYABLE.
```

## Documentation
```MODEL KEY:
01_PL - Player Models
02_NM - NPC Male Models
03_NF - NPC Female Models
04_NC - NPC Child Models
05_EN - Enemy Models
06_MB - ???
07_AN - Animal Models
08_IT - Item Models
09_WP - Weapon Models
011_FC - ???
FIXED - ???
```
