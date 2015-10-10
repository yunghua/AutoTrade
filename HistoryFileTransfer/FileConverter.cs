using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradeUtility;

namespace HistoryFileTransfer
{
    class FileConverter
    {

        string appDir = "";//應用程式所在目錄

        TradeFile sourceFile;//來源舊檔

        TradeFile destnationFile;//目的新檔

        string sourceDir = "";//來源目錄

        string destDir = "";//新檔目錄

        string destFullPath = "";//新檔案完整路徑        

        private void transferFile()
        {

            string oneLineText = "";//一行文字

            while (sourceFile.hasNext())
            {

                oneLineText = sourceFile.getLine();

                if (oneLineText.Contains("MTX"))
                {
                    destnationFile.writeLine(oneLineText);
                }
            }
        }

        public string convert()
        {
            List<TradeFile> oFileList;

            try
            {

                appDir = System.Windows.Forms.Application.StartupPath;

                sourceDir = appDir + "\\Old\\";

                destDir = appDir + "\\New\\";

                FileManager fm = new FileManager();

                oFileList = fm.getTradeFileList(sourceDir);

                if (oFileList == null || oFileList.Count <= 0)
                {
                    return "目錄內無檔案!";
                }

                for (int j = 0; j < oFileList.Count; j++)
                {
                    sourceFile = ((TradeFile)oFileList[j]);

                    sourceFile.prepareReader();

                    destFullPath = destDir + sourceFile.getFileName();

                    destnationFile = new TradeFile(destFullPath);

                    destnationFile.prepareWriter(FileMode.CreateNew);

                    transferFile();

                }

                sourceFile.close();

                destnationFile.close();

            }
            catch (Exception e)
            {
                return e.StackTrace;
            }

            return "轉換成功!共轉換" + oFileList.Count + "個檔案";

        }

    }
}
