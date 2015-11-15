using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TradeUtility
{
    public class FileManager
    {

       
        

            public List<ConfigFile> getConfigFileList(String dir)
        {
            return getConfigFileList(dir, "*.txt");
        }

        private List<ConfigFile> getConfigFileList(String dir, String extension)
        {
            DirectoryInfo di = new DirectoryInfo(dir);

            List<ConfigFile> fileList = new List<ConfigFile>();

            FileInfo[] rgFiles = di.GetFiles(extension);

            foreach (FileInfo fi in rgFiles)
            {
                //Console.WriteLine("FileName:" + fi.Name);
                ConfigFile oFile = new ConfigFile(dir + "/" + fi.Name);
                fileList.Add(oFile);
            }

            return fileList;
        }

        public List<TradeFile> getTradeFileList(String dir)
        {
            return getTradeFileList(dir, "*.rpt");
        }

        private List<TradeFile> getTradeFileList(String dir, String extension)
        {
            DirectoryInfo di = new DirectoryInfo(dir);

            List<TradeFile> fileList = new List<TradeFile>();

            FileInfo[] rgFiles = di.GetFiles(extension);

            foreach (FileInfo fi in rgFiles)
            {
                //Console.WriteLine("FileName:" + fi.Name);
                TradeFile oFile = new TradeFile(dir + "/" + fi.Name);
                fileList.Add(oFile);
            }

            return fileList;
        }

    }
}
