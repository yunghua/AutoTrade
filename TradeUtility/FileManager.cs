using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TradeUtility
{
    public class FileManager
    {

        public List<TradeFile> getTradeFileList(String dir)
        {
            DirectoryInfo di = new DirectoryInfo(dir);

            List<TradeFile> fileList = new List<TradeFile>();

            FileInfo[] rgFiles = di.GetFiles("*.rpt");

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
