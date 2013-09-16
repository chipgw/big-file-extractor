using System;
using System.Collections.Generic;
using System.Text;

namespace Big_File_Extractor
{
    class BigHeaderError : Exception
    {
        public BigHeaderError(string header)
        {
            this.foundheader = header;
        }
        public string foundheader;
    }
    class BigContentsError : Exception
    {
        public BigContentsError(string Description)
        {
            this.description = Description;
        }
        public string description;
    }
    class NonexistentFileError : Exception
    {
        public string filename;

        public NonexistentFileError(string file)
        {
            this.filename = file;
        }
    }
}
