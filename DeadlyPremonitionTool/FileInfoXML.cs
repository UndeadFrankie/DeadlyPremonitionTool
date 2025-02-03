using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadlyPremonitionTool
{
    public class FileInfoXML
    {
        public int FileNumber { get; set; }
        public string FileName { get; set; }
        public string Directory { get; set; }
        public int Size { get; set; }
        public bool ExtraPadded { get; set; }
        public string FileExtensions { get; set; }
    }
}
