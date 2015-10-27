using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeUtility
{
    public class StrategyFile
    {

        const string Strategy_File_Name = "Strategy.txt";

        const string Config_Dir = "Config";//設定檔目錄

        static StrategyFile strategyInstance = null;

        string strategyFilePath = "";

        string strategyFileName = "";

        TradeFile strategyFile;

        Dictionary<int, int> loseLine;  //認賠的底線

        Dictionary<int, int> winLine;  //停利的底線

        Dictionary<int, int> reverseLine;  //動態停利反轉的底線

        Dictionary<int, int> stopRatio;  //逆勢動態停利

        int maxStrategyCount = 0; //停損停利規則最大有幾種

        public void close()
        {
            strategyFile.close();
        }

        public int getMaxStrategyCount()
        {
            return maxStrategyCount;
        }


        public static StrategyFile getInstance()
        {

            if (strategyInstance == null)
            {
                strategyInstance = new StrategyFile();
            }

            return strategyInstance;
        }

        public Dictionary<int, int> getStopRatio()
        {
            return stopRatio;
        }

        public Dictionary<int, int> getReverseLine()
        {
            return reverseLine;
        }

        public Dictionary<int, int> getLoseLine()
        {
            return loseLine;
        }

        public Dictionary<int, int> getWinLine()
        {
            return winLine;
        }

        public Boolean dealStrategyRule(string appDir)
        {
            return dealStrategyRule(appDir, Strategy_File_Name);
        }

        public Boolean dealStopRatioRule(string appDir, string fileName)//讀取逆勢動態停利規則檔
        {
            try
            {

                strategyFileName = fileName;

                strategyFilePath = appDir + "\\" + Config_Dir + "\\" + fileName;

                strategyFile = new TradeFile(strategyFilePath);

                strategyFile.prepareReader();

                stopRatio = new Dictionary<int, int>();

                int checkRange;//最高點與最低點的範圍

                int ratio;//停利或是停損的計算百分比

                String tmpLine = "";

                String[] tmpData = new String[2];

                while (strategyFile.hasNext())
                {

                    tmpLine = strategyFile.getLine();

                    tmpData = tmpLine.Split(',');

                    checkRange = int.Parse(tmpData[0]);

                    ratio = int.Parse(tmpData[1]);

                    stopRatio.Add(checkRange, ratio);


                }

                Console.WriteLine(stopRatio.ToString());

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return false;
            }
            return true;
        }



        public Boolean dealStrategyRule(string appDir, string fileName)//讀取停損停利規則檔
        {
            try
            {

                strategyFileName = fileName;

                strategyFilePath = appDir + "\\" + Config_Dir + "\\" + fileName;

                strategyFile = new TradeFile(strategyFilePath);

                strategyFile.prepareReader();

                loseLine = new Dictionary<int, int>();

                winLine = new Dictionary<int, int>();

                reverseLine = new Dictionary<int, int>();

                int strategyCount = 1;//讀取停損停利規則檔案的行數

                int losePoint;//停損點範圍

                int winPoint;//停利點範圍

                int reversePoint;//動態停利反轉範圍

                String tmpLine = "";

                String[] tmpData = new String[3];

                while (strategyFile.hasNext())
                {


                    tmpLine = strategyFile.getLine();

                    if (tmpLine.StartsWith("#") || tmpLine.Trim().Equals(""))
                    {
                        continue;
                    }

                    tmpData = tmpLine.Split(',');

                    losePoint = int.Parse(tmpData[0]);

                    winPoint = int.Parse(tmpData[1]);

                    try
                    {
                        reversePoint = int.Parse(tmpData[2]);
                    }
                    catch (Exception e)
                    {
                        reversePoint = 10;
                    }

                    loseLine.Add(strategyCount, losePoint);

                    winLine.Add(strategyCount, winPoint);

                    reverseLine.Add(strategyCount, reversePoint);

                    strategyCount++;
                }

                maxStrategyCount = --strategyCount;

                for (int i = 1; i <= maxStrategyCount; i++)
                {
                    Console.WriteLine("loseLine[" + i + "] : " + loseLine[i]);
                    Console.WriteLine("winLine[" + i + "] : " + winLine[i]);
                    Console.WriteLine("reverseLine[" + i + "] : " + reverseLine[i]);
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return false;
            }
            return true;
        }

    }
}
