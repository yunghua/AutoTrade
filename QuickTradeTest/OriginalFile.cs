using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace QuickTradeTest
{
    class OriginalFile
    {
        String sFileName;
        StreamReader reader;

        String currentLine = "";

        public String CurrentLine
        {
            get { return currentLine; }
            set { currentLine = value; }
        }

        public String getFileName()
        {
            return sFileName;
        }

        public void setFileName(String fileName)
        {
            sFileName = fileName;
            reader = new StreamReader(fileName);
        }

        public OriginalFile(String fileName)
        {
            sFileName = fileName;
            reader = new StreamReader(fileName);

        }

        public Boolean hasNext()
        {
            //if (reader.Peek() >= 0)
            //{
            //    return true;
            //}


            if (reader.EndOfStream)
            {
                return false;
            }
            else
            {
                return true;
            }
        }



        public String getLine()
        {

            currentLine = reader.ReadLine();

            return currentLine;
        }

        public void close()
        {
            if (reader != null)
            {
                reader.Close();
                reader.Dispose();
            }
        }

    }
}
