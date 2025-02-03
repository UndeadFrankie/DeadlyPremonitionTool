##Deadly Premonition Tool

A simple toolkit designed to making modding Deadly Premonition easier!
Uses SharpZipLib and BetterFolderBrowser!
https://www.virustotal.com/gui/file/34fef478eea2531a6aa20680ecd7992e0cb3a03543e894497945ae15a584330f

##FILE FORMATS

.DDS = DirectDraw Surface, the default format for textures in Deadly Premonition. Oddly the game doesn't seem to store the mipmaps in the files so they might be generated during runtime.
.XPC = ???, Texture container responsible for associaiting dds files with their respective models and so on. Indiviual textures are compressed with zlib 1.2.1
DPSerial. = Proprietary archive format used to store nearly all game files. Each file has different contents. 001 = Mixed, 002 = Models, 003 Textures.

##USAGE

UNPACK XPC -Allows the user to select XPC files and unpack the contents to a folder named "FILENAME_UNPACKED". Textures are automatically decompressed from zlib (best compression)
PACK XPC - Allows the user to select a .DDS file and reimport it into an XPC file. At the moment this only supports reimporting a single dds at a time but can repeated to add all modified textures. Textures are automatically recompressed with zlib (best compression)
UNPACK DPSERIAL - Allows the user to select a DPSerial file and unpack the contents to a folder called "programmer_pc". Generates an XML for reimporting later.
PACK DPSERIAL - Allows the user to select an XML file and an unpacked "programmer_pc" folder to repack into a DPSerial file. File size is derrived from the files themselves so don't bother editing the XML.

CURRENTLY DPSERIAL CAN'T BE A DIFFERENT SIZE WITHOUT CRASHING! ENSURE THAT ANY FILES REPLACED ARE SMALLER OR EQUAL TO THE SIZE OF THE ORIGINAL FILE.
SMALLER FILES WILL BE FILLED WITH 0'S TO MAKE UP THE DIFFERENCE. THIS CAN BE DISABLED WITH THE FILE SIZE FIX CHECKBOX BUT IT'S NOT RECCOMMENDED AND WILL LIKELY BREAK FILE LOADING MAKING THE GAME UNPLAYABLE.

##EXAMPLE USEAGE

Player Model Swap: 
1.) Unpack both DPSerial.002 and DPSerial.003 (in different locations!)
2.) Find a character in "programmer_PC/main/UPDATA/CHARA/" folder see model key for more info
3.) Copy the XPC and XMD files to another folder.
4a.) Go to "programmer_PC/main/UPDATA/CHARA/01_PL/CPL011"
4b.) Go to "programmer_PC/main/UPDATA/CHARA/01_PL/CPL011"
3.) Find a character in  folder see model key for more info

MODEL KEY:
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
