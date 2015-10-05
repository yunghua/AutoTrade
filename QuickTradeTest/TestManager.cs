using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeUtility;

namespace QuickTradeTest
{
    class TestManager
    {

        const int Rule_Count_Win = 1;//停利跑幾種規則

        const int Rule_Count_Lose = 1;//停損跑幾種規則

        const int Run_Count = 1000;//每種規則跑幾次測試

        const int Rule_Period = 0;//每次規則增加幅度
        
        const Boolean isReport = true;

        const String testReportFilePath = "C:/Trader/TestReport/";

        const String testSourceFilePath = "C:/Trader/Test";        

        TradeFile testReportFile;

        string testReportFileName = "";

        TradeFile conclusionReport;

        String conclusionReportFileName = "";//總報告

        int testDayCount = 0;//測試幾天的歷史資料

        int guid = 0;//測試編號

        Dictionary<int, int> loseLine;  //認賠的底線

        Dictionary<int, int> winLine;  //停利的底線

        DateTime now = System.DateTime.Now;

        //Boolean isPrepared = false;

        public TestManager()
        {

        }

        public Boolean prepareTest()
        {


            StrategyFile strategyInstance = StrategyFile.getInstance();

            Boolean isRuleReady = strategyInstance.dealStrategyRule();

            if (!isRuleReady)
            {
                return false;
            }

            this.winLine = strategyInstance.getWinLine();

            this.loseLine = strategyInstance.getLoseLine();

            if (winLine != null && loseLine != null)
            {

                conclusionReportFileName = testReportFilePath + now.Year + "_" + now.Month + "_" + now.Day + "_" + now.Hour + "_" + now.Minute + "_" + now.Second + "_Conclusion.rpt";

                conclusionReport = new TradeFile(conclusionReportFileName);

                conclusionReport.prepareWriter();

                return true;
            }

            return false;

        }



        public void startTest()
        {

            if (!prepareTest())
            {
                reportMsg("規則檔案讀取失敗!");

                return;
            }



            try
            {
                int j = 0;



                Dictionary<int, int> initialLoseLine = new Dictionary<int, int>();

                for (int xx = 1; xx <= loseLine.Count; xx++)
                {
                    initialLoseLine[xx] = loseLine[xx];
                }



                for (int k = 1; k <= Rule_Count_Win; k++)
                {
                    j = 0;

                    int tmpWin = 0;

                    for (j = 1; j <= winLine.Count; j++)
                    {
                        tmpWin = winLine[j] + j * Rule_Period;

                        winLine[j] = tmpWin;
                    }    // end winLine




                    for (int i = 1; i <= Rule_Count_Lose; i++)
                    {

                        j = 0;

                        int tmpLose = 0;

                        for (j = 1; j <= loseLine.Count; j++)
                        {
                            tmpLose = loseLine[j] + j * Rule_Period;

                            loseLine[j] = tmpLose;
                        }

                        startTest(k * 1000 + i);

                    }//end for i

                    for (int i = 1; i <= loseLine.Count; i++)
                    {
                        loseLine[i] = initialLoseLine[i];
                    }

                }//end for k
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Source);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
                return;
            }

        }

        public void startTest(int guid)
        {

            this.guid = guid;

            try
            {



                testReportFileName = testReportFilePath + now.Year + "_" + now.Month + "_" + now.Day + "_" + now.Hour + "_" + now.Minute + "_" + now.Second + "_" + loseLine[1] + "_" + winLine[1] + "_" + guid + ".rpt";

                testReportFile = new TradeFile(testReportFileName);

                testReportFile.prepareWriter();

            }
            catch (Exception ex)
            {
                reportMsg(ex.Source + ex.Message + ex.StackTrace);
            }

            double totalProfit = 0;//所有測試日期，各自跑了XX次之後的總利潤

            double oneDayProfit = 0;//某日的單日利潤

            int winDayCount = 0;//獲利日數

            int loseDayCount = 0;//賠錢日數

            int winCountInOneDayTradeRunManyTimes;//一日交易中有幾次獲利，執行數次測試之後的結果

            int loseCountInOneDayTradeRunManyTimes;//一日交易中有幾次賠錢，執行數次測試之後的結果

            int totalWinCountRumManyTimes = 0;//總獲利次數

            int totalLoseCountRunManyTimes = 0;//總賠錢次數


            int[] profitRange = new int[31];//獲利的範圍

            double maxWinProfit = 0;

            double maxLoseProfit = 0;

            for (int i = 0; i < 31; i++)
            {
                profitRange[i] = 0;
            }

            FileManager fm = new FileManager();

            List<TradeFile> oFileList = fm.getTradeFileList(testSourceFilePath);

            testDayCount = oFileList.Count;

            if (oFileList == null || oFileList.Count <= 0)
            {
                reportMsg("目錄內無檔案!");
                return;
            }

            for (int j = 0; j < oFileList.Count; j++)
            {

                double oneDayRunManyTimesTotalProfit = 0;//某日跑了XX次之後的總利潤

                winCountInOneDayTradeRunManyTimes = 0;//一日交易中有幾次獲利

                loseCountInOneDayTradeRunManyTimes = 0;//一日交易中有幾次賠錢               

                for (int i = 0; i < Run_Count; i++)
                {

                    TradeManager manager = new TradeManager();

                    manager.setWinLine(winLine);

                    manager.setLoseLine(loseLine);

                    manager.setSourceFile(oFileList[j]);

                    oneDayProfit = manager.startTrade();

                    winCountInOneDayTradeRunManyTimes += manager.getWinCount();

                    loseCountInOneDayTradeRunManyTimes += manager.getLoseCount();


                    if (oneDayProfit > 0)
                    {
                        winDayCount++;
                    }
                    else
                    {
                        loseDayCount++;
                    }


                    if (oneDayProfit > maxWinProfit)
                    {
                        maxWinProfit = oneDayProfit;
                    }

                    if (oneDayProfit < maxLoseProfit)
                    {
                        maxLoseProfit = oneDayProfit;
                    }                   

                    if (oneDayProfit * 50 > 0 && oneDayProfit * 50 < 2000)
                    {

                        profitRange[0]++;
                    }
                    else if (oneDayProfit * 50 > 20000)
                    {
                        profitRange[10]++;
                    }
                    else if (oneDayProfit * 50 > 10000)
                    {
                        profitRange[9]++;
                    }
                    else if (oneDayProfit * 50 > 9000)
                    {
                        profitRange[8]++;
                    }
                    else if (oneDayProfit * 50 > 8000)
                    {
                        profitRange[7]++;
                    }
                    else if (oneDayProfit * 50 > 7000)
                    {
                        profitRange[6]++;
                    }
                    else if (oneDayProfit * 50 > 6000)
                    {
                        profitRange[5]++;
                    }
                    else if (oneDayProfit * 50 > 5000)
                    {
                        profitRange[4]++;
                    }
                    else if (oneDayProfit * 50 > 4000)
                    {
                        profitRange[3]++;
                    }
                    else if (oneDayProfit * 50 > 3000)
                    {
                        profitRange[2]++;
                    }
                    else if (oneDayProfit * 50 > 2000)
                    {
                        profitRange[1]++;
                    }
                    else if (oneDayProfit * 50 < -20000)
                    {
                        profitRange[30]++;
                    }
                    else if (oneDayProfit * 50 < -10000)
                    {
                        profitRange[29]++;
                    }
                    else if (oneDayProfit * 50 < -9000)
                    {
                        profitRange[28]++;
                    }
                    else if (oneDayProfit * 50 < -8000)
                    {
                        profitRange[27]++;
                    }
                    else if (oneDayProfit * 50 < -7000)
                    {
                        profitRange[26]++;
                    }
                    else if (oneDayProfit * 50 < -6000)
                    {
                        profitRange[25]++;
                    }
                    else if (oneDayProfit * 50 < -5000)
                    {
                        profitRange[24]++;
                    }
                    else if (oneDayProfit * 50 < -4000)
                    {
                        profitRange[23]++;
                    }
                    else if (oneDayProfit * 50 < -3000)
                    {
                        profitRange[22]++;
                    }
                    else if (oneDayProfit * 50 < -2000)
                    {
                        profitRange[21]++;
                    }
                    if (oneDayProfit * 50 < 0 && oneDayProfit * 50 > -2000)
                    {
                        profitRange[20]++;
                    }



                    totalProfit += oneDayProfit;

                    oneDayRunManyTimesTotalProfit += oneDayProfit;

                    //Console.WriteLine("交易結束，單日交易總利潤 : " + oneTimeProfit * 50);

                }

                totalWinCountRumManyTimes += winCountInOneDayTradeRunManyTimes;

                totalLoseCountRunManyTimes += loseCountInOneDayTradeRunManyTimes;

                reportMsg(oFileList[j].getFileName() + "交易結束，單日交易平均利潤 : " + oneDayRunManyTimesTotalProfit * 50 / Run_Count);

                reportMsg(oFileList[j].getFileName() + "交易結束，單日獲利次數 : " + winCountInOneDayTradeRunManyTimes);

                reportMsg(oFileList[j].getFileName() + "交易結束，單日賠錢次數 : " + loseCountInOneDayTradeRunManyTimes);

                reportMsg(oFileList[j].getFileName() + "交易結束，單日獲利次數的總比率 : " + Convert.ToDouble(winCountInOneDayTradeRunManyTimes) / ((Convert.ToDouble(winCountInOneDayTradeRunManyTimes) + Convert.ToDouble(loseCountInOneDayTradeRunManyTimes))) * 100 + " %");

            }


            reportMsg(" 測試編號 : " + guid);
            reportMsg(" 每個交易日的測試次數 : " + Run_Count);

            reportMsg("獲利次數 : " + totalWinCountRumManyTimes);
            reportMsg("賠錢次數 : " + totalLoseCountRunManyTimes);
            reportMsg("交易結束，獲利次數的總比率 : " + Convert.ToDouble(totalWinCountRumManyTimes) / ((Convert.ToDouble(totalWinCountRumManyTimes) + Convert.ToDouble(totalLoseCountRunManyTimes))) * 100 + " %");

            reportMsg("獲利日數 : " + winDayCount);
            reportMsg("賠錢日數" + loseDayCount);
            reportMsg("交易結束，獲利日數的總比率 : " + Convert.ToDouble(winDayCount) / ((Convert.ToDouble(winDayCount) + Convert.ToDouble(loseDayCount))) * 100 + " %");

            reportMsg("最大獲利 : " + maxWinProfit * 50);
            reportMsg("最大賠錢 : " + maxLoseProfit * 50);


            reportMsg("總手續費 : " + (totalWinCountRumManyTimes + totalLoseCountRunManyTimes) * 50);

            reportMsg("平均手續費 : " + ((totalWinCountRumManyTimes + totalLoseCountRunManyTimes) * 50) / (Run_Count * testDayCount));

            double pureProfit = ((totalProfit * 50 - (totalWinCountRumManyTimes + totalLoseCountRunManyTimes) * 50)) / (Run_Count * testDayCount);

            reportMsg(Run_Count * oFileList.Count + "次，總利潤 : " + totalProfit * 50);

            reportMsg(Run_Count * oFileList.Count + "次，扣除手續費後，總平均利潤 : " + pureProfit);

            if (pureProfit > 0)
            {
                conclusionReport.writeLine("------------loseLine : " + loseLine[1] + "----------------winLine : " + winLine[1] + "-------------PureProfit : " + pureProfit);
            }

            reportMsg("獲利兩千以下次數 : " + profitRange[0]);
            reportMsg("獲利兩千以上次數 : " + profitRange[1]);
            reportMsg("獲利三千以上次數 : " + profitRange[2]);
            reportMsg("獲利四千以上次數 : " + profitRange[3]);
            reportMsg("獲利五千以上次數 : " + profitRange[4]);
            reportMsg("獲利六千以上次數 : " + profitRange[5]);
            reportMsg("獲利七千以上次數 : " + profitRange[6]);
            reportMsg("獲利八千以上次數 : " + profitRange[7]);
            reportMsg("獲利九千以上次數 : " + profitRange[8]);
            reportMsg("獲利一萬以上次數 : " + profitRange[9]);
            reportMsg("獲利兩萬以上次數 : " + profitRange[10]);
            reportMsg("----------------------------------------------------------------------------------------------");
            reportMsg("賠錢兩千以下次數 : " + profitRange[20]);
            reportMsg("賠錢兩千以上次數 : " + profitRange[21]);
            reportMsg("賠錢三千以上次數 : " + profitRange[22]);
            reportMsg("賠錢四千以上次數 : " + profitRange[23]);
            reportMsg("賠錢五千以上次數 : " + profitRange[24]);
            reportMsg("賠錢六千以上次數 : " + profitRange[25]);
            reportMsg("賠錢七千以上次數 : " + profitRange[26]);
            reportMsg("賠錢八千以上次數 : " + profitRange[27]);
            reportMsg("賠錢九千以上次數 : " + profitRange[28]);
            reportMsg("賠錢一萬以上次數 : " + profitRange[29]);
            reportMsg("賠錢兩萬以上次數 : " + profitRange[30]);

            reportMsg("----------------------------------------------------------------------------------------------");
            for (int i = 1; i <= winLine.Count; i++)
            {
                reportMsg("測試規則WIN   00" + i + ":" + winLine[i]);
            }

            reportMsg("----------------------------------------------------------------------------------------------");

            for (int i = 1; i <= winLine.Count; i++)
            {
                reportMsg("測試規則LOSE  00" + i + ":" + loseLine[i]);
            }

            reportMsg("----------------------------------------------------------------------------------------------");
            reportMsg("----------------------------------------------------------------------------------------------");
            reportMsg("----------------------------------------------------------------------------------------------");
        }



        private void reportMsg(String msg)
        {
            try
            {

                Console.WriteLine(msg);

                if (isReport)
                {
                    this.testReportFile.writeLine(msg);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.Message);
            }

        }

        public void stop()
        {

            try
            {
                conclusionReport.close();
                testReportFile.close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.Message);

            }

        }


    }//end class TestManager
}
