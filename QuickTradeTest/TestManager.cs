using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeUtility;

namespace QuickTradeTest
{
    class TestManager
    {

        //const string Core_Method = TradeManager.Core_Method_2; //1=獲利後下次加碼，2=動態停利

        const double cost = 66;//手續費成本

        const double valuePerPoint = 50;//每點價值，小台50元/點，大台200元/點        

        const string Config_Dir = "Config";//設定檔目錄

        const string Report_Dir = "Report";//報告檔目錄

        const string Conclusion_Dir = "Conclusion";//總結檔目錄

        const string Source_Dir = "History";//來源檔檔目錄

        const string Config_File_Name = "TradeConfig.txt";//設定檔案名

        const Boolean isReport = true;

        const String testReportFilePath = "";

        string sourceFileDir = "";//來源RPT檔案所在目錄

        TradeFile testReportFile;

        string testReportFileName = "";

        TradeFile conclusionReport;

        String conclusionReportFileName = "";//總報告

        int testDayCount = 0;//測試幾天的歷史資料

        int guid = 0;//測試編號

        Dictionary<int, int> loseLine;  //認賠的底線

        Dictionary<int, int> winLine;  //停利的底線

        DateTime now = System.DateTime.Now;

        string[] lotArray;//獲利加碼的設定

        string conclusionDir;//

        string reportDir;//

        string coreMethod;//核心方法

        int ruleCountWin;//停利跑幾種規則
        int ruleCountLose;//停損跑幾種規則
        int runCount;//每種規則跑幾次測試
        int rulePeriod;//每次規則增加幅度

        string sourceDir = "";//來源檔子目錄名，通常是History

        double oneDayPureProfit;//;當日純利

        string maxLoss = "";//單日最大停損

        //Boolean isPrepared = false;

        public TestManager()
        {

        }

        public Boolean prepareTest()
        {


            StrategyFile strategyInstance = StrategyFile.getInstance();

            string appDir = System.IO.Directory.GetCurrentDirectory(); //主程式所在目錄

            Boolean isRuleReady = strategyInstance.dealStrategyRule(appDir, "TestStrategy.txt");

            if (!isRuleReady)
            {
                return false;
            }

            appDir = System.Windows.Forms.Application.StartupPath;

            string configFilePath = appDir + "\\" + Config_Dir + "\\" + Config_File_Name;

            ConfigFile configFile = new ConfigFile(configFilePath);
            try
            {
                configFile.prepareReader();

                coreMethod = configFile.readConfig("Core_Method");
                ruleCountWin = Convert.ToInt32(configFile.readConfig("Rule_Count_Win"));
                ruleCountLose = Convert.ToInt32(configFile.readConfig("Rule_Count_Lose"));
                runCount = Convert.ToInt32(configFile.readConfig("Run_Count"));
                rulePeriod = Convert.ToInt32(configFile.readConfig("Rule_Period"));
                maxLoss = configFile.readConfig("Max_Loss");
                sourceDir = configFile.readConfig("Source_Dir");

                if (null == sourceDir)
                {
                    sourceDir = Source_Dir;
                }

            }
            catch (Exception)
            {
                return false;
            }

            List<int> lotList = new List<int>();

            try
            {

                string lots = configFile.readConfig("Lots");

                lotArray = lots.Split(',');

            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.StackTrace);
            }

            this.winLine = strategyInstance.getWinLine();

            this.loseLine = strategyInstance.getLoseLine();

            reportDir = appDir + "\\" + Report_Dir + "\\";

            conclusionDir = appDir + "\\" + Conclusion_Dir + "\\";

            sourceFileDir = appDir + "\\" + sourceDir + "\\";


            if (winLine != null && loseLine != null)
            {

                conclusionReportFileName = conclusionDir + now.Year + "_" + now.Month + "_" + now.Day + "_" + now.Hour + "_" + now.Minute + "_" + now.Second + "_Conclusion.rpt";

                conclusionReport = new TradeFile(conclusionReportFileName);

                conclusionReport.prepareWriter();

                conclusionMsg("使用核心:" + coreMethod);

                conclusionMsg("單日設定最大停損" + maxLoss);

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



                for (int k = 1; k <= ruleCountWin; k++)
                {
                    j = 0;

                    int tmpWin = 0;

                    for (j = 1; j <= winLine.Count; j++)
                    {
                        tmpWin = winLine[j] + j * rulePeriod;

                        winLine[j] = tmpWin;
                    }    // end winLine




                    for (int i = 1; i <= ruleCountLose; i++)
                    {

                        j = 0;

                        int tmpLose = 0;

                        for (j = 1; j <= loseLine.Count; j++)
                        {
                            tmpLose = loseLine[j] + j * rulePeriod;

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


                testReportFileName = reportDir + now.Year + "_" + now.Month + "_" + now.Day + "_" + now.Hour + "_" + now.Minute + "_" + now.Second + "_" + loseLine[1] + "_" + winLine[1] + "_" + guid + ".rpt";

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

            double maxWinPureProfit = 0;

            double maxLosePureProfit = 0;

            for (int i = 0; i < 31; i++)
            {
                profitRange[i] = 0;
            }

            FileManager fm = new FileManager();

            List<TradeFile> oFileList = fm.getTradeFileList(sourceFileDir);

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

                double oneDayMaxLossSetting = -9999999;//單日最大停損設定

                for (int i = 0; i < runCount; i++)
                {

                    TradeManager manager = new TradeManager();

                    if (maxLoss != null && !maxLoss.Equals(""))
                    {
                        manager.setMaxProfitLoss(Convert.ToDouble(maxLoss));
                    }

                    manager.setCore(coreMethod);

                    manager.setLots(lotArray);

                    manager.setWinLine(winLine);

                    manager.setLoseLine(loseLine);

                    manager.setSourceFile(oFileList[j]);

                    oneDayProfit = manager.startTrade();

                    winCountInOneDayTradeRunManyTimes += manager.getWinCount();

                    loseCountInOneDayTradeRunManyTimes += manager.getLoseCount();

                    oneDayPureProfit = oneDayProfit * valuePerPoint - (winCountInOneDayTradeRunManyTimes + loseCountInOneDayTradeRunManyTimes) * cost;

                    if (oneDayPureProfit > 0)
                    {
                        winDayCount++;
                    }
                    else
                    {
                        loseDayCount++;
                    }


                    if (oneDayPureProfit > maxWinPureProfit)
                    {
                        maxWinPureProfit = oneDayPureProfit;
                    }

                    if (oneDayPureProfit < maxLosePureProfit)
                    {
                        maxLosePureProfit = oneDayPureProfit;
                    }

                    if (oneDayPureProfit > 0 && oneDayPureProfit < 2000)
                    {

                        profitRange[0]++;
                    }
                    else if (oneDayPureProfit > 20000)
                    {
                        profitRange[10]++;
                    }
                    else if (oneDayPureProfit > 10000)
                    {
                        profitRange[9]++;
                    }
                    else if (oneDayPureProfit > 9000)
                    {
                        profitRange[8]++;
                    }
                    else if (oneDayPureProfit > 8000)
                    {
                        profitRange[7]++;
                    }
                    else if (oneDayPureProfit > 7000)
                    {
                        profitRange[6]++;
                    }
                    else if (oneDayPureProfit > 6000)
                    {
                        profitRange[5]++;
                    }
                    else if (oneDayPureProfit > 5000)
                    {
                        profitRange[4]++;
                    }
                    else if (oneDayPureProfit > 4000)
                    {
                        profitRange[3]++;
                    }
                    else if (oneDayPureProfit > 3000)
                    {
                        profitRange[2]++;
                    }
                    else if (oneDayPureProfit > 2000)
                    {
                        profitRange[1]++;
                    }
                    else if (oneDayPureProfit < -20000)
                    {
                        profitRange[30]++;
                    }
                    else if (oneDayPureProfit < -10000)
                    {
                        profitRange[29]++;
                    }
                    else if (oneDayPureProfit < -9000)
                    {
                        profitRange[28]++;
                    }
                    else if (oneDayPureProfit < -8000)
                    {
                        profitRange[27]++;
                    }
                    else if (oneDayPureProfit < -7000)
                    {
                        profitRange[26]++;
                    }
                    else if (oneDayPureProfit < -6000)
                    {
                        profitRange[25]++;
                    }
                    else if (oneDayPureProfit < -5000)
                    {
                        profitRange[24]++;
                    }
                    else if (oneDayPureProfit < -4000)
                    {
                        profitRange[23]++;
                    }
                    else if (oneDayPureProfit < -3000)
                    {
                        profitRange[22]++;
                    }
                    else if (oneDayPureProfit < -2000)
                    {
                        profitRange[21]++;
                    }
                    if (oneDayPureProfit < 0 && oneDayPureProfit > -2000)
                    {
                        profitRange[20]++;
                    }



                    totalProfit += oneDayProfit;

                    oneDayRunManyTimesTotalProfit += oneDayProfit;

                    oneDayMaxLossSetting = manager.getMaxProfitLoss();

                    //Console.WriteLine("交易結束，單日交易總利潤 : " + oneTimeProfit * valuePerPoint);

                }

                totalWinCountRumManyTimes += winCountInOneDayTradeRunManyTimes;

                totalLoseCountRunManyTimes += loseCountInOneDayTradeRunManyTimes;

                reportMsg("單日設定最大停損" + Convert.ToString(oneDayMaxLossSetting));

                reportMsg(oFileList[j].getFullPath() + "交易結束，單日交易平均利潤 : " + ((oneDayRunManyTimesTotalProfit * valuePerPoint) - (winCountInOneDayTradeRunManyTimes + loseCountInOneDayTradeRunManyTimes) * cost) / runCount);

                reportMsg(oFileList[j].getFullPath() + "交易結束，單日獲利次數 : " + winCountInOneDayTradeRunManyTimes);

                reportMsg(oFileList[j].getFullPath() + "交易結束，單日賠錢次數 : " + loseCountInOneDayTradeRunManyTimes);

                reportMsg(oFileList[j].getFullPath() + "交易結束，單日獲利次數的總比率 : " + Convert.ToDouble(winCountInOneDayTradeRunManyTimes) / ((Convert.ToDouble(winCountInOneDayTradeRunManyTimes) + Convert.ToDouble(loseCountInOneDayTradeRunManyTimes))) * 100 + " %");

            }


            reportMsg(" 測試編號 : " + guid);
            reportMsg(" 每個交易日的測試次數 : " + runCount);

            reportMsg("獲利次數 : " + totalWinCountRumManyTimes);
            reportMsg("賠錢次數 : " + totalLoseCountRunManyTimes);
            reportMsg("交易結束，獲利次數的總比率 : " + Convert.ToDouble(totalWinCountRumManyTimes) / ((Convert.ToDouble(totalWinCountRumManyTimes) + Convert.ToDouble(totalLoseCountRunManyTimes))) * 100 + " %");

            reportMsg("獲利日數 : " + winDayCount);
            reportMsg("賠錢日數" + loseDayCount);
            reportMsg("交易結束，獲利日數的總比率 : " + Convert.ToDouble(winDayCount) / ((Convert.ToDouble(winDayCount) + Convert.ToDouble(loseDayCount))) * 100 + " %");

            reportMsg("單日最大獲利 : " + maxWinPureProfit);
            reportMsg("單日最大賠錢 : " + maxLosePureProfit);
            reportMsg("單日設定最大停損" + maxLoss);


            reportMsg("最大交易口數 : " + lotArray[lotArray.Length - 1]);

            reportMsg("總獲利口數 : " + totalWinCountRumManyTimes);

            reportMsg("總賠錢口數 : " + totalLoseCountRunManyTimes);

            reportMsg("總手續費 : " + (totalWinCountRumManyTimes + totalLoseCountRunManyTimes) * cost);

            reportMsg("平均手續費 : " + ((totalWinCountRumManyTimes + totalLoseCountRunManyTimes) * cost) / (runCount * testDayCount));

            double pureProfit = ((totalProfit * valuePerPoint - (totalWinCountRumManyTimes + totalLoseCountRunManyTimes) * cost)) / (runCount * testDayCount);

            reportMsg(runCount * oFileList.Count + "次，總利潤 : " + totalProfit * valuePerPoint);

            reportMsg(runCount * oFileList.Count + "次，扣除手續費後，總平均利潤 : " + pureProfit);



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

            

            if (pureProfit > 0)
            {
                conclusionMsg("------------loseLine : " + loseLine[1] + "----------------winLine : " + winLine[1] + "-------------PureProfit : " + pureProfit);
            }


        }



        private void showMsg(TradeFile file, string msg)
        {
            try
            {

                Console.WriteLine(msg);

                if (isReport)
                {
                    file.writeLine(msg);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.Message);
            }


        }

        private void conclusionMsg(string msg)
        {
            showMsg(conclusionReport, msg);
        }

        private void reportMsg(string msg)
        {
            showMsg(testReportFile, msg);
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
