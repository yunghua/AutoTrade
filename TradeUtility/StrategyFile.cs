using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeUtility
{
    public class StrategyFile
    {

        static  StrategyFile  strategyInstance = null;

        const String strategyFilePath = "C:/Trader/TestStrategy.txt";

        Dictionary<int, int> loseLine;  //認賠的底線

        Dictionary<int, int> winLine;  //停利的底線

        int maxStrategyCount = 0; //停損停利規則最大有幾種

        public int getMaxStrategyCount()
        {
            return maxStrategyCount;
        }

        static TradeFile  strategyFile = new TradeFile(strategyFilePath);         

        public static StrategyFile getInstance(){

            if(strategyInstance==null)
            {
                strategyInstance = new StrategyFile();
            }

            return strategyInstance;
        }

        public Dictionary<int, int> getLoseLine()
        {
            return loseLine;
        }

        public Dictionary<int, int> getWinLine()
        {
            return winLine;
        }


        public Boolean dealStrategyRule()//讀取停損停利規則檔
        {
            try
            {

                loseLine = new Dictionary<int, int>();

                winLine = new Dictionary<int, int>();

                strategyFile.prepareReader();

                int strategyCount = 1;//讀取停損停利規則檔案的行數

                int losePoint;//停損點範圍

                int winPoint;//停利點範圍

                String tmpLine = "";

                String[] tmpData = new String[2];

                while (strategyFile.hasNext())
                {

                    tmpLine = strategyFile.getLine();

                    tmpData = tmpLine.Split(',');

                    losePoint = int.Parse(tmpData[0]);

                    winPoint = int.Parse(tmpData[1]);

                    loseLine.Add(strategyCount, losePoint);

                    winLine.Add(strategyCount, winPoint);

                    strategyCount++;
                }

                maxStrategyCount = --strategyCount;

                for (int i = 1; i <= maxStrategyCount; i++)
                {
                    Console.WriteLine("loseLine[" + i + "] : " + loseLine[i]);
                    Console.WriteLine("winLine[" + i + "] : " + winLine[i]);
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
