using DeadlyPremonitionTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using WK.Libraries.BetterFolderBrowserNS;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.IO.Compression;
using System.Windows.Forms.VisualStyles;
using static DeadlyPremonitionTool.Form1;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace DeadlyPremonitionTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static class TextureFileProcessor
        {
            public static (uint CompressedTexSize, string NewTexName, string FinalTexName, byte[] CompressedTexData) ProcessXPCFile()
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Browse New DDS Files",
                    Multiselect = false,
                    Filter = "DDS files (*.dds*)|*.dds*",
                    RestoreDirectory = true
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileName = openFileDialog.FileName;
                    using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
                    {
                        // Read the uncompressed file data
                        byte[] newTexData = reader.ReadBytes((int)reader.BaseStream.Length);

                        // Compress the file data
                        byte[] compressedTexData;
                        using (MemoryStream compressedStream = new MemoryStream())
                        {
                            // Set Deflater with BEST_COMPRESSION
                            Deflater deflater = new Deflater(Deflater.BEST_COMPRESSION, false); // false for zlib header
                            using (DeflaterOutputStream deflaterStream = new DeflaterOutputStream(compressedStream, deflater))
                            {
                                deflaterStream.Write(newTexData, 0, newTexData.Length);
                                deflaterStream.Finish(); // Ensure all data is written and flushed
                            }
                            compressedTexData = compressedStream.ToArray();
                        }

                        // Get compressed size
                        uint compressedTexSize = (uint)compressedTexData.Length;

                        // Generate file name metadata
                        string newTexName = Path.GetFileNameWithoutExtension(fileName) + ".";
                        string finalTexName = newTexName + "DDS" + "\0\0\0\0\0";

                        // Return the compressed size and data
                        return (compressedTexSize, newTexName, finalTexName, compressedTexData);
                    }
                }

                return (0, null, null, null); // Return defaults if no file is selected
            }
        }

        public static string DecodeByteArray(byte[] data, string prefixToRemove = null)
        {
            // Decode the byte array into a string
            string result = Encoding.UTF8.GetString(data);

            // Find the first null terminator and trim the string
            int nullIndex = result.IndexOf('\0');
            if (nullIndex >= 0)
            {
                result = result.Substring(0, nullIndex);
            }

            // Remove the specified prefix (e.g., "D:\")
            if (!string.IsNullOrEmpty(prefixToRemove) && result.StartsWith(prefixToRemove, StringComparison.OrdinalIgnoreCase))
            {
                result = result.Substring(prefixToRemove.Length);
            }

            return result;
        }

        private void unpackXPC_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Browse XPC Files",
                Multiselect = true, // Allow user to select multiple files in the OpenFileDialog
                Filter = "XPC files (*.xpc*)|*.xpc*",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string fileName in openFileDialog.FileNames)
                {
                    string baseFileName = Path.GetFileNameWithoutExtension(fileName);
                    string newFileName = null;

                    FileInfo fileInfo = new FileInfo(fileName); // Instantiate FileInfo on Input XPC to get properties from it.
                    long fileSize = fileInfo.Length; // Set fileSize equal to entire XPC file

                    List<FileInfoXML> files = new List<FileInfoXML>(); // Instantiate a list for storing XML properties.
                    string fileExt = Path.GetExtension(fileName); // Isolate file extension from path

                    using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
                    {
                        byte[] magicNum = reader.ReadBytes(4); // XPC2
                        int xpcFileSize = reader.ReadInt32(); // Total Size of File in Bytes.
                        short xpcFileCount = reader.ReadInt16(); // Total Number of Textures Stored in XPC.
                        short xpcMatCount = reader.ReadInt16(); // ???
                        byte[] xpcUseless1 = reader.ReadBytes(20); // ???
                        int xpcStringOffset = reader.ReadInt32(); // Offset to the start of String Table 
                        int xpcDataOffset = reader.ReadInt32(); // Offset to the start of Data Table
                        byte[] xpcUseless2 = reader.ReadBytes(24); // ???
                        string createFolder = ""; // I don't remember why I made this var.
                        long totalZFileSize = 0; // PLACEHOLDER - DO NOT REMOVE

                        if (xpcFileCount == 1) 
                        {
                            reader.BaseStream.Position = xpcStringOffset; //Move stream to the start of String Table
                            byte[] xpcFileName = reader.ReadBytes(16); //Read the file name max array size 16 chars.
                            string decodedString = DecodeByteArray(xpcFileName, "D:\\"); //Helper func to strip unnessary bytes and return the proper string. The Args take (byte[] fileName, str "string_to_remove");

                            xpcStringOffset += 16; // Advance the offset of the String Table Offset to highlight the data.
                            reader.BaseStream.Position = xpcStringOffset; // THIS MIGHT BE UNNECESSARY - TEST AND REMOVE.

                            int xpcCurrentDataOffset = reader.ReadInt32(); // Read Current Entry Data Offset - This is our texture data offset
                            int xpcCurrentDataSize = reader.ReadInt32(); // Read Current Entry Data Size - This is our texture data size
                            int xpcCurrentChunkSize = reader.ReadInt32(); // Read Current Chunk Size - This isn't useful but it should be tied to zlib somehow.
                            short xpcCurrentUnkShort1 = reader.ReadInt16();
                            short xpcCurrentUnkShort2 = reader.ReadInt16();

                            reader.BaseStream.Position = xpcCurrentDataOffset; // Move the reader to start of the current texture data offset

                            byte[] xpcCurrentFileContent = reader.ReadBytes(xpcCurrentDataSize); // Read in the entire contents of the current file based on DataSize
                            string folderPath = Path.GetDirectoryName(fileName);
                            createFolder = folderPath + "/" + baseFileName + "_unpacked" + "/";
                            newFileName = createFolder + decodedString; // Ugly way of making the directories for the unpacked XPC Folder

                            Directory.CreateDirectory(createFolder); // This assumes the user hasn't created the XPC_UNPACKED folder. TODO: Check if this crashes the tool if the folder already exists.

                            using (MemoryStream compressedStream = new MemoryStream(xpcCurrentFileContent))
                            using (InflaterInputStream inflaterInputStream = new InflaterInputStream(compressedStream))
                            using (FileStream newFileStream = new FileStream(newFileName, FileMode.Create, FileAccess.Write))
                            {
                                inflaterInputStream.CopyTo(newFileStream);
                            }
                            if (messageBoxCheckbox.Checked == true)
                            {
                                MessageBox.Show($"File: {baseFileName} unpacked successfully.\n" + $"Original size: {xpcCurrentFileContent.Length} bytes, Uncompressed size: {new FileInfo(newFileName).Length} bytes", "Unpacking Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                            

                            files.Add(new FileInfoXML
                            {
                                FileName = decodedString,
                                FileExtensions = fileExt,
                                FileNumber = 0,
                                Directory = newFileName,
                                Size = xpcCurrentDataSize,
                            });

                            // Add all relavent file info from the current entry to the XML we create 

                            FileInfoXMLHandler xmlHandler = new FileInfoXMLHandler();
                            string currentDirectoryXML = createFolder;
                            string outputPathXML = Path.Combine(currentDirectoryXML, baseFileName);
                            xmlHandler.WriteToXml(files, outputPathXML + fileExt + ".xml");

                            // XML might be unnecessary here because it's not used during the repacking of XPC Files!
                        } //Special file handling if XPC only contains one texture. This might not be necessary anymore but I'm just gonna leave it.
                        else if (xpcFileCount > 1)
                        {
                            reader.BaseStream.Position = xpcStringOffset;
                            for (int i = 0; i < xpcFileCount; i++)
                            {
                                byte[] xpcTexName = reader.ReadBytes(16); //Read the file name max array size 16 chars.
                                string decodedString = DecodeByteArray(xpcTexName, "D:\\");
                                uint xpcTexOffset = reader.ReadUInt32();
                                uint xpcTexSize = reader.ReadUInt32();
                                uint xpcChunkSize = reader.ReadUInt32();
                                ushort xpcUnkShort1 = reader.ReadUInt16();
                                ushort xpcUnkShort2 = reader.ReadUInt16();

                                long xpcCurrentPosition = reader.BaseStream.Position;

                                reader.BaseStream.Position = xpcTexOffset;

                                byte[] xpcTexData = reader.ReadBytes((int)xpcTexSize);

                                reader.BaseStream.Position = xpcCurrentPosition;

                                string folderPath = Path.GetDirectoryName(fileName);
                                createFolder = folderPath + "/" + baseFileName + "_unpacked" + "/";
                                newFileName = createFolder + decodedString;

                                Directory.CreateDirectory(createFolder);

                                // A bunch of ugly code for handling the ZLIB Decompression of the files.

                                using (MemoryStream compressedStream = new MemoryStream(xpcTexData))
                                using (InflaterInputStream inflaterInputStream = new InflaterInputStream(compressedStream))
                                using (FileStream newFileStream = new FileStream(newFileName, FileMode.Create, FileAccess.Write))
                                {
                                    inflaterInputStream.CopyTo(newFileStream);
                                    totalZFileSize += newFileStream.Length;
                                }

                                // Add all relavent file info from the current entry to the XML we create 

                                files.Add(new FileInfoXML
                                {
                                    FileName = decodedString,
                                    FileExtensions = fileExt,
                                    FileNumber = i,
                                    Directory = newFileName,
                                    Size = xpcTexData.Length,
                                });                         
                            }

                            if (messageBoxCheckbox.Checked == true)
                            {
                                MessageBox.Show($"File: {fileName} unpacked successfully.\n" + $"Original size: {fileSize} bytes, Uncompressed size: {totalZFileSize} bytes", "Unpacking Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            } // ONLY MEANT FOR DEBUG NOT USEFUL.

                            // XML might be unnecessary here because it's not used during the repacking of XPC Files!
                            FileInfoXMLHandler xmlHandler = new FileInfoXMLHandler();
                            string currentDirectoryXML = createFolder;
                            string outputPathXML = Path.Combine(currentDirectoryXML, baseFileName);
                            xmlHandler.WriteToXml(files, outputPathXML + fileExt + ".xml");
                            
                        } // If there's more then one texture inside the XPC use this code.
                        else
                        {
                            MessageBox.Show($"xpcFileCount can't be less than 1 in {fileName}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); //THIS SHOULD NEVER HAPPEN. IT'S ONLY POSSIBLE IF A COMPLETELY INCOMPATIBLE FILE IS USED BREAKING xpcFileCount
                        } // THIS SHOULD NEVER HAPPEN, IT'S ONLY POSSIBLE WITH AN INVALID INPUT OR A MISALIGNED FILE!!
                    }
                }
            }
        } //UNPACK XPC

        private void injectXCP_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Browse XPC Files";
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "XPC files (*.xpc*)|*.xpc*";
            openFileDialog.RestoreDirectory = true;
        
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string fileName in openFileDialog.FileNames)
                {
                    using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
                    {
                        char[] magic = reader.ReadChars(4);
                        uint xpc_fileSize = reader.ReadUInt32();
                        ushort xpc_fileCount = reader.ReadUInt16();
        
                        string xpc_FileName = Path.GetFileNameWithoutExtension(fileName);
        
                        bool moddedTexture = false;
                        uint castTexOffsets = 0;
                        //long preStringOffset = 0;
                        long lastTexInfo = 0;
                        long lastTex = 0;
                        //byte stringPadding = 0;
                        uint trueFileSize = 0;
                        uint trueFileOffset = 0;
                        //uint preheaderLength = 224;
                        int moddedIteration = 0;
        
        
                        string[] xpc_texNames = new string[xpc_fileCount];
                        uint[] xpc_texOffsets = new uint[xpc_fileCount];
                        uint[] xpc_texSizes = new uint[xpc_fileCount];
                        ushort[] xpc_mipCount = new ushort[xpc_fileCount];
                        ushort[] xpc_unk = new ushort[xpc_fileCount];
                        ushort[] xpc_unk1 = new ushort[xpc_fileCount];
                        ushort[] xpc_unk2 = new ushort[xpc_fileCount];
                        //byte[] xpc_texDatas = new byte[xpc_fileCount];
                        //List<byte[]> xpc_texDatas = new List<byte[]>();
                        byte[][] xpc_texDatas = new byte[xpc_fileCount][];
        
                        ushort xpc_matCount = reader.ReadUInt16();
                        ushort xpc_usedTex = reader.ReadUInt16();
                        ushort xpc_unkk3 = reader.ReadUInt16();
                        uint xpc_unkk4 = reader.ReadUInt32();
                        uint xpc_unkk5 = reader.ReadUInt32();
                        uint xpc_unkk6 = reader.ReadUInt32();
                        uint xpc_unkk7 = reader.ReadUInt32();
                        uint xpc_stringOffset = reader.ReadUInt32();
                        uint xpc_dataOffset = reader.ReadUInt32();
        
                        byte[] xpc_padded = reader.ReadBytes(24);
        
                        var (CompressedTexSize, NewTexName, FinalTexName, CompressedTexData) = TextureFileProcessor.ProcessXPCFile();
        
                        for (int i = 0; i < xpc_fileCount; i++)
                        {
                            char[] fileDDSName1 = reader.ReadChars(16);
                            string fixedLengthString1 = new string(fileDDSName1);
                            xpc_texNames[i] = fixedLengthString1;
        
                            if (xpc_texNames[i].TrimEnd('\0', ' ').Contains(FinalTexName.TrimEnd('\0', ' ')))
                            {
                                moddedTexture = true;
                                xpc_texOffsets[i] = reader.ReadUInt32();
                                castTexOffsets = (uint)xpc_texOffsets[i];
                                xpc_texSizes[i] = reader.ReadUInt32();
        
                                moddedIteration = i;
        
                                uint spareSize = xpc_texSizes[i];
                                int cast_spareSize = (int)xpc_texSizes[i];
                                uint ucast_spareSize = (uint)xpc_texSizes[i];
        
                                xpc_texSizes[i] = CompressedTexSize; //Replace texture size with size from imported file
        
                                //trueFileSize += xpc_texSizes[i];
        
                                xpc_mipCount[i] = reader.ReadUInt16();
                                xpc_unk[i] = reader.ReadUInt16();
                                xpc_unk1[i] = reader.ReadUInt16();
                                xpc_unk2[i] = reader.ReadUInt16();
        
                                lastTexInfo = reader.BaseStream.Position; //We want to come back to this when we collect the tex data from this texture.
        
                                reader.BaseStream.Position = xpc_texOffsets[i];
                                xpc_texDatas[i] = CompressedTexData;
                                byte[] garbageTex = reader.ReadBytes(cast_spareSize);
        
                                trueFileOffset = xpc_texOffsets[i];
                                trueFileOffset += CompressedTexSize;
        
                                lastTex = reader.BaseStream.Position; //We want to come back to this when the offset for the next image data is found.
        
                                reader.BaseStream.Position = lastTexInfo;
        
                                continue;
                            }
        
                            if (moddedTexture == true)
                            {
                                xpc_texOffsets[i] = reader.ReadUInt32();
                                //xpc_texOffsets[i] += castTexOffsets; //Add size of Modified texture to original offset for every subsequent texture.
                                xpc_texSizes[i] = reader.ReadUInt32();
                                int cast_xpc_texSizes = (int)xpc_texSizes[i];
                                uint ucast_xpc_texSizes = (uint)xpc_texSizes[i];
        
                                //trueFileSize += xpc_texSizes[i];
        
                                xpc_texOffsets[i] = xpc_texOffsets[i - 1];
                                xpc_texOffsets[i] += xpc_texSizes[i - 1];
        
        
                                //xpc_texOffsets[i] = preheaderLength;
                                //xpc_texOffsets[i] += ucast_xpc_texSizes;
                                //trueFileOffset += castNewTexSize;
        
                                xpc_mipCount[i] = reader.ReadUInt16();
                                xpc_unk[i] = reader.ReadUInt16();
                                xpc_unk1[i] = reader.ReadUInt16();
                                xpc_unk2[i] = reader.ReadUInt16();
        
                                lastTexInfo = reader.BaseStream.Position;
        
                                reader.BaseStream.Position = lastTex;
                                xpc_texDatas[i] = reader.ReadBytes(cast_xpc_texSizes);
        
                                lastTex = reader.BaseStream.Position;
        
                                reader.BaseStream.Position = lastTexInfo;
        
                                continue;
                            }
                            else
                            {
                                xpc_texOffsets[i] = reader.ReadUInt32();
                                xpc_texSizes[i] = reader.ReadUInt32();
                                int cast_xpc_texSizes = (int)xpc_texSizes[i];
        
                                trueFileSize += xpc_texSizes[i];
        
                                xpc_mipCount[i] = reader.ReadUInt16();
                                xpc_unk[i] = reader.ReadUInt16();
                                xpc_unk1[i] = reader.ReadUInt16();
                                xpc_unk2[i] = reader.ReadUInt16();
        
                                lastTexInfo = reader.BaseStream.Position;
        
                                reader.BaseStream.Position = xpc_texOffsets[i];
                                xpc_texDatas[i] = reader.ReadBytes(cast_xpc_texSizes);
        
                                lastTex = reader.BaseStream.Position;
        
                                reader.BaseStream.Position = lastTexInfo;
        
                                continue;
                            }
                        }
        
                        //for (int i = 0; i < xpc_fileCount; i++)
                        //{
                        string tempPath = Path.GetTempFileName();
                        string outputPath = (fileName + ".converted");
                        using (BinaryWriter writer = new BinaryWriter(File.Open(tempPath, FileMode.Create)))
                        {
                            writer.Write(magic); //0x0
                            writer.Write(trueFileSize); //0x4
                            writer.Write(xpc_fileCount); //0x8
                            writer.Write(xpc_matCount); //0xA
                            writer.Write(xpc_usedTex); //0xC 
                            writer.Write(xpc_unkk3); //0xE
                            writer.Write(xpc_unkk4); //0x10
                            writer.Write(xpc_unkk5); //0x14
                            writer.Write(xpc_unkk6); //0x18
                            writer.Write(xpc_unkk7); //0x1C
                            writer.Write(xpc_stringOffset); //0x20
                            writer.Write(xpc_dataOffset); //0x24
                            writer.Write(xpc_padded); //0x28
        
                            for (int i = 0; i < xpc_fileCount; i++)
                            {
                                //writer.BaseStream.Position = xpc_stringOffset;
                                //xpc_stringOffset += 32;
                                string currentString = xpc_texNames[i];
                                byte[] stringBytes = Encoding.ASCII.GetBytes(xpc_texNames[i]);
                                writer.Write(stringBytes); //0x40
                                writer.Write(xpc_texOffsets[i]); //0x50
                                writer.Write(xpc_texSizes[i]); //0x54
                                writer.Write(xpc_mipCount[i]); //0x58
                                writer.Write(xpc_unk[i]); //0x5A
                                writer.Write(xpc_unk1[i]); //0x5C
                                writer.Write(xpc_unk2[i]); //0x5E
        
                                lastTexInfo = writer.BaseStream.Position;
        
                                writer.BaseStream.Position = xpc_texOffsets[i];
                                //writer.BaseStream.Position = lastTex;
        
                                writer.Write(xpc_texDatas[i]);
        
                                lastTex = writer.BaseStream.Position;
        
                                writer.BaseStream.Position = lastTexInfo;
                            }
                            trueFileSize = (uint)writer.BaseStream.Length;
                            writer.BaseStream.Position = 4;
                            writer.Write(trueFileSize);
                        }
                        reader.Close();
                        File.Delete(fileName);
        
                        // Move the temporary file to the original file's path
                        File.Move(tempPath, fileName);
                        MessageBox.Show("DDS Injected Successfully!");
                        //}
                    }
                }
            }
        }

        private void injectDPSerial_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Browse DPSerial Files";
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "DPSerial files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;            

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string fileName in openFileDialog.FileNames)
                {
                    using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
                    {
                        string fileExt = Path.GetExtension(fileName);
                        string inputPath = Path.GetDirectoryName(fileName) ?? string.Empty;
                        fileExt = fileExt.Substring(1); // Remove leading dot
                        string serial001 = "001";
                        string serial002 = "002";
                        string serial003 = "003";

                        int fileCount = 0;
                        //int trimCount = 0;

                        if (fileExt == serial001)
                        {
                            fileCount = 7535;
                            //trimCount = 0;
                        }
                        else if (fileExt == serial002)
                        {
                            fileCount = 3019;
                            //trimCount = 3;
                        }
                        else if (fileExt == serial003)
                        {
                            fileCount = 3774;
                        }
                        else
                        {
                            MessageBox.Show("File Extension Not Recognized!");
                            break;
                        }

                        List<FileInfoXML> files = new List<FileInfoXML>();

                        for (int i = 0; i < fileCount; i++)
                        {
                            // Step 1: Read filename (256 bytes)
                            byte[] serialFileName = reader.ReadBytes(256);
                            string decodedString = DecodeByteArray(serialFileName, "D:\\");
                            decodedString = Path.Combine(inputPath, decodedString);

                            // Step 2: Read file size (4 bytes as long)
                            int serialFileSize = reader.ReadInt32();

                            // Step 3: Save current position (offset)
                            long offset = reader.BaseStream.Position;

                            // Step 4: Read file data
                            byte[] serialFileData = reader.ReadBytes(serialFileSize);

                            // Step 5: Advance to the next aligned position
                            long newPosition = offset + serialFileSize;
                            long remainder = serialFileSize % 16;

                            if (remainder == 0)
                            {
                                // File size is already aligned, skip 16 bytes of padding
                                reader.ReadBytes(16);
                            }
                            else
                            {
                                // File size isn't aligned, skip only the necessary bytes
                                int padding = (int)(16 - remainder);
                                reader.ReadBytes(padding);
                            }

                            // Step 6: Ensure directory exists
                            string serialFileDirectory = Path.GetDirectoryName(decodedString) ?? string.Empty;
                            if (!string.IsNullOrEmpty(serialFileDirectory) && !Directory.Exists(serialFileDirectory))
                            {
                                Directory.CreateDirectory(serialFileDirectory);
                            }

                            // Step 7: Write file if required
                            if (genFoldersOnly.Checked != true)
                            {
                                using (BinaryWriter writer = new BinaryWriter(File.Open(decodedString, FileMode.Create)))
                                {
                                    writer.Write(serialFileData);
                                }
                            }

                            // Add file info to the list
                            files.Add(new FileInfoXML
                            {
                                FileName = decodedString.Replace(inputPath + Path.DirectorySeparatorChar, ""),
                                FileExtensions = fileExt,
                                FileNumber = i,
                                Directory = serialFileDirectory.Replace(inputPath + Path.DirectorySeparatorChar, ""),
                                Size = serialFileSize,
                                ExtraPadded = (remainder == 0) // Mark if padding was applied
                            });

                            // Update UI
                            fileCounter.Text = i.ToString();
                        }

                        // Write XML in the same directory as the input file
                        FileInfoXMLHandler xmlHandler = new FileInfoXMLHandler();
                        string outputPathXML = Path.Combine(inputPath, $"DPSerial_{fileExt}.xml");
                        xmlHandler.WriteToXml(files, outputPathXML);

                        MessageBox.Show("File Unpacked Successfully!");
                    }
                }
            }
        }

        private void packSerialBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Browse .xml Files";
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = ".xml files (*.xml*)|*.xml*";
            openFileDialog.RestoreDirectory = true;

            string lastXMLPath = "";

            int FileSizeFinally = 0;
            int totalFileCounts = 0;
            int expectedSize = 0;
            byte[] currentFileName = null;
            byte[] currentFileExt = null;
            byte[] save_currentFileName = null;
            string string_currentFileName = null;
            byte[] newFileNameArray = null;
            //byte[] newFileSizeArray = null;
            byte[] sizeArray = null;
            byte[] fileBytes = null;
            byte[] USELESSTEMP = { 0x00, 0x00, 0x00, 0x00 };
            string cast_currentFileExt = null;
            byte[] lastTrySize = null;
            //bool[] filesPadded = null;
            //byte[] testbg = 0;

            int tempSize;

            byte[] pathStringIsh = { 0x44, 0x3A, 0x5C };

            byte[][] filePadding = new byte[16][]
            {
                new byte[16],  // Padding for modulus 0 (no padding needed)
                new byte[15], // Padding for modulus 1
                new byte[14], // Padding for modulus 2
                new byte[13], // Padding for modulus 3
                new byte[12], // Padding for modulus 4
                new byte[11], // Padding for modulus 5
                new byte[10], // Padding for modulus 6
                new byte[9],  // Padding for modulus 7
                new byte[8],  // Padding for modulus 8
                new byte[7],  // Padding for modulus 9
                new byte[6],  // Padding for modulus 10
                new byte[5],  // Padding for modulus 11
                new byte[4],  // Padding for modulus 12
                new byte[3],  // Padding for modulus 13
                new byte[2],  // Padding for modulus 14
                new byte[1]  // Padding for modulus 15
            };

            List<byte[]> fileNameByteArrays = new List<byte[]>();
            List<byte[]> fileSizeArrays = new List<byte[]>();
            List<byte[]> fileDataByteArrays = new List<byte[]>();

            List<string> filePaddedStrings = new List<string>();
            string extraPaddedAsString = null;

            //int currentIncrement = 0;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                List<object> arrays = new List<object>();
                foreach (string fileName in openFileDialog.FileNames)
                {
                    var xmlHandler = new FileInfoXMLHandler();
                    List<FileInfoXML> files = xmlHandler.ReadFromXml(fileName);
                    lastXMLPath = Path.GetDirectoryName(fileName);
                    foreach (var file in files)
                    {
                        //string[] fileNum = new string[];
                        int[] fileNumberArray = new int[] { file.FileNumber };  // Storing int as an array of int
                        byte[] fileNameArray = Encoding.UTF8.GetBytes(file.FileName);  // String to byte array
                        byte[] directoryArray = Encoding.UTF8.GetBytes(file.Directory);  // String to byte array
                        //testbg = file.Size;
                        sizeArray = BitConverter.GetBytes(file.Size);  // Long to byte array
                        byte[] extraPaddedArray = BitConverter.GetBytes(file.ExtraPadded);  // Bool to byte array
                        byte[] fileExtensionsArray = Encoding.UTF8.GetBytes(file.FileExtensions);
                        extraPaddedAsString = file.ExtraPadded.ToString();

                        string filePath = file.FileName;

                        if (File.Exists(filePath))
                        {
                            FileInfo fileInfo = new FileInfo(filePath);
                            sizeArray = BitConverter.GetBytes(fileInfo.Length);
                        }

                        arrays.Add(fileNumberArray);
                        arrays.Add(fileNameArray);
                        arrays.Add(directoryArray);
                        arrays.Add(sizeArray);
                        arrays.Add(extraPaddedArray);
                        arrays.Add(fileExtensionsArray);

                        totalFileCounts++;
                    }
                }

                // Prompt the user to select the output folder for the repacked files
                using (BetterFolderBrowser folderDialog = new BetterFolderBrowser())
                {
                    folderDialog.Title = "Browse DPSerial Folder";
                    folderDialog.RootFolder = lastXMLPath;
                    byte[] zeroTemp = { 0x00 };
                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        string outputFolderDirectory = folderDialog.SelectedPath;  // Get the selected folder path
                        string resultPath = outputFolderDirectory.Substring(0, outputFolderDirectory.Length - 14); // Get the selected folder path
                        for (int i = 0; i < totalFileCounts; i++)
                        {
                            currentFileExt = (byte[])arrays[i * 6 + 5];

                            if (fixFileSizes.Checked == true)
                            {
                                lastTrySize = (byte[])arrays[i * 6 + 3]; //Get File size from files
                                expectedSize = BitConverter.ToInt32(lastTrySize, 0);
                            }

                            cast_currentFileExt = Encoding.UTF8.GetString(currentFileExt);
                            currentFileName = (byte[])arrays[i * 6 + 1];
                            save_currentFileName = (byte[])arrays[i * 6 + 1];
                            string_currentFileName = Encoding.UTF8.GetString(save_currentFileName);
                            string trimmed_string = string_currentFileName.Substring(13);

                            string fullFilePath = outputFolderDirectory;
                            fullFilePath += trimmed_string;

                            FileInfo fileInfo1 = new FileInfo(fullFilePath);
                            tempSize = (int)fileInfo1.Length;
                            byte[] actualFileSize = BitConverter.GetBytes(tempSize); // Get actual file size
                            if (fixFileSizes.Checked != true)
                            {
                                lastTrySize = actualFileSize;
                            }
                            Console.WriteLine(sizeArray);

                            if (currentFileName.Length != 256)
                            {
                                newFileNameArray = new byte[256];
                                Array.Copy(currentFileName, newFileNameArray, currentFileName.Length);
                                newFileNameArray[currentFileName.Length] = 0x00;
                                for (int y = currentFileName.Length + 1; y < 253; y++)
                                {
                                    newFileNameArray[y] = 0xCC;  // Fill with 0xCC
                                }
                                arrays[i * 6 + 1] = newFileNameArray;
                                fileNameByteArrays.Add(newFileNameArray);
                            }
                            else
                            {
                                newFileNameArray = currentFileName;
                            }

                            // Remove trailing 0xCC bytes
                            int validLength = save_currentFileName.Length;
                            while (validLength > 0 && (save_currentFileName[validLength - 1] == 0x00 || save_currentFileName[validLength - 1] == 0xCC))
                            {
                                validLength--;
                            }

                            // Trim the array to exclude 0xCC bytes
                            byte[] trimmedBytes = new byte[validLength];
                            Array.Copy(save_currentFileName, trimmedBytes, validLength);

                            // Decode the trimmed byte array to a string
                            string fixedSerialString = Encoding.UTF8.GetString(trimmedBytes);
                            fixedSerialString = fixedSerialString.Substring(13);

                            string serialFileDirectory = Path.GetDirectoryName(fixedSerialString);

                            //fileBytes = File.ReadAllBytes(outputFolderDirectory + fixedSerialString);

                            using (FileStream fs = new FileStream(outputFolderDirectory + fixedSerialString, FileMode.Open, FileAccess.Read))
                            {
                                fileBytes = new byte[fs.Length];
                                fs.Read(fileBytes, 0, fileBytes.Length);
                            }

                            if (fixFileSizes.Checked == true)
                            {
                                if (fileBytes.Length != expectedSize)
                                {
                                    Array.Resize(ref fileBytes, expectedSize);
                                }
                                else if (fileBytes.Length > expectedSize)
                                {
                                    MessageBox.Show("TEXTURES CAN'T BE LARGER THEN ORIGINAL XPC!");
                                    continue;
                                }
                            }

                            FileInfo fileInfo = new FileInfo(outputFolderDirectory + fixedSerialString);
                            FileSizeFinally = (int)fileInfo.Length;
                            fileDataByteArrays.Add((byte[])fileBytes);

                            fileSizeArrays.Add(lastTrySize); //Write File Size from XML

                            filePaddedStrings.Add(extraPaddedAsString);
                        }

                        using (BinaryWriter writer = new BinaryWriter(File.Open(resultPath + "/" + "DPSerial." + cast_currentFileExt, FileMode.Create)))
                        {
                            for (int i = 0; i < totalFileCounts; i++)
                            {
                                writer.Write(pathStringIsh);
                                writer.Write(fileNameByteArrays[i]);
                                writer.BaseStream.Seek(-3, SeekOrigin.Current);
                                writer.Write(fileSizeArrays[i]);
                                writer.Write(fileDataByteArrays[i]);
                        
                                int fileSizeBool = BitConverter.ToInt32(fileSizeArrays[i], 0);
                                int paddingIndex = (16 - (fileSizeBool % 16)) % 16;
                        
                                writer.Write(filePadding[paddingIndex]); // Dynamically writes the correct padding

                                fileDataByteArrays[i] = null;
                                fileSizeArrays[i] = null;
                                fileNameByteArrays[i] = null;
                                fileBytes = null;

                                GC.Collect();
                                GC.WaitForPendingFinalizers();
                            }
                            writer.Close();
                            writer.Dispose();
                        }
                    }
                    fileDataByteArrays.Clear();
                    fileSizeArrays.Clear();
                    fileNameByteArrays.Clear();
                    filePaddedStrings.Clear();                   
                    MessageBox.Show("File Packed Successfully!");
                }
            }
        } //PACK DPSERIAL
    }
}