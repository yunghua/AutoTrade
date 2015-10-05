using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TradeUtility
{
    public class TradeFile
    {
        String sFileName;

        StreamReader reader;

        StreamWriter writer;

        String currentLine = "";

        Boolean isFileExist = false;

        FileStream fs;

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

        }

        public TradeFile()
        {

        }

        public TradeFile(String fileName)
        {
            sFileName = fileName;
        }

        public Boolean isExist()
        {
            isFileExist = File.Exists(sFileName);
            return isFileExist;
        }

        public void prepareReader()
        {
            try
            {
                reader = new StreamReader(sFileName);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }


        public void prepareWriter()
        {
            try
            {

                isFileExist = File.Exists(sFileName);

                if (isFileExist)
                {
                    fs = new FileStream(sFileName, FileMode.Append);
                }
                else
                {
                    fs = new FileStream(sFileName, FileMode.CreateNew);
                }

                writer = new StreamWriter(fs);
                writer.AutoFlush = true;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }



        public Boolean hasNext()
        {
            //if (reader.Peek() >= 0)
            //{
            //    return true;
            //}

            if (reader == null)
            {

                return false;

            }

            if (reader.EndOfStream)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void writeLine(String line)
        {

            try
            {

                writer.WriteLine(line);

            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }



        public String getLine()
        {

            currentLine = reader.ReadLine();

            return currentLine;
        }

        public void close()
        {

            try
            {

                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }

            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }


            try
            {

                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }

            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }


            try
            {

                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }

            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
