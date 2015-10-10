using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TradeUtility
{
    public class TradeFile
    {
        String fullPath;//完整路徑

        String fileName;//檔案名稱

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
            if (fileName == null || fileName.Length == 0)
            {
                fileName = Path.GetFileName(fullPath);
            }

            return this.fileName;

        }

        public void setFileName(String name)
        {
            fileName = name;
        }

        public String getFullPath()
        {
            return fullPath;
        }

        public void setFullPath(String fullPath)
        {
            this.fullPath = fullPath;

        }

        public TradeFile()
        {

        }

        public TradeFile(String fullPath)
        {
            this.fullPath = fullPath;
        }

        public Boolean isExist()
        {
            isFileExist = File.Exists(fullPath);
            return isFileExist;
        }

        public void prepareReader()
        {
            isFileExist = File.Exists(fullPath);

            if (!isFileExist)
            {
                throw new Exception(fullPath +
                    "不存在!");
            }


            try
            {
                reader = new StreamReader(fullPath);
            }
            catch (IOException e)
            {
                throw e;
            }
        }


        public void prepareWriter(FileMode mode)
        {
            try
            {
                fs = new FileStream(fullPath, mode);

                writer = new StreamWriter(fs);
                writer.AutoFlush = true;

            }
            catch (IOException e)
            {
                throw e;
            }

        }

        public void prepareWriter()
        {
            try
            {

                isFileExist = File.Exists(fullPath);

                if (isFileExist)
                {
                    fs = new FileStream(fullPath, FileMode.Append);
                }
                else
                {
                    fs = new FileStream(fullPath, FileMode.CreateNew);
                }

                writer = new StreamWriter(fs);
                writer.AutoFlush = true;
            }
            catch (IOException e)
            {
                throw e;
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
        }

    }
}
