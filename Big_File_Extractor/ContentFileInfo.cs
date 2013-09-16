using System;
using System.Collections.Generic;
using System.Text;

namespace Big_File_Extractor
{
    class ContentFileInfo
    {
        public int startAddress;
        public int dataLength;
        public string fileName;

        public ContentFileInfo(int startaddress, int length, string filename)
        {
            this.startAddress = startaddress;
            this.dataLength = length;
            this.fileName = filename;
        }
    }
}
