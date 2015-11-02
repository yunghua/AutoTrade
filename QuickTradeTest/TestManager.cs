using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeUtility;

namespace QuickTradeTest
{
    class TestManager
    {



        const Boolean DEBUG = false;

        //const string Core_Method = TradeManager.Core_Method_2; //1=獲利後下次加碼，2=動態停利

        public const string Core_Method_1 = "Core_Method_1";//獲利加碼

        public const string Core_Method_2 = "Core_Method_2";//動態停利

        public const string Core_Method_3 = "Core_Method_3";//逆勢動態停利

        const double cost = 68;//手續費成本

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

        Dictionary<int, double> reverseLine;  //反轉的底線

        DateTime now = System.DateTime.Now;

        string[] lotArray;//獲利加碼的設定

        string conclusionDir;//

        string reportDir;//

        string coreMethod;//核心方法

        int ruleCountWin;//停利跑幾種規則
        int ruleCountLose;//停損跑幾種規則
        int ruleCountReverse;//反轉動態停利跑幾種規則

        int runCount;//每種規則跑幾次測試
        int rulePeriod;//每次規則增加幅度

        string sourceDir = "";//來源檔子目錄名，通常是History

        double oneDayPureProfit;//;當日純利

        string maxLoss = "";//單日最大停損



        double ratio;//動態停利的反轉比率，小於1，越接近1表示要回檔接近停利設定，才會執行停利，也就是越不敏感。

        string lots;

        int maxLot;//最大交易口數

        string maxLosePureProfitFileName;//最大賠錢是哪一天

        string maxWinPureProfitFileName;//最大獲利是哪一天

        Dictionary<int, int> stopRatio;  //逆勢動態停利的百分比查表，第一個int 是點數間隔，第二個是百分比

        int checkCount = 5;//檢查幾個時間間隔，來決定買或是賣

        //Boolean isPrepared = false;

        double winDayRate = 0;//獲利日數的比率

        string testVersion = "";//程式版本

        string appDir = "";//主程式的目錄

        public TestManager()
        {

        }

        public string getAppDir()
        {
            return appDir;
        }

        public string getVersion()
        {
            return testVersion;
        }

        public string getWinLine()
        {
            //if (winLine != null && winLine[1]!= null)
            return Convert.ToString(winLine[1]);
        }

        public string getLoseLine()
        {
            //if (loseLine != null && loseLine[1] != null)
            return Convert.ToString(loseLine[1]);
        }

        public string getReverseLine()
        {
            //if (reverseLine != null && reverseLine[1] != null)
            return Convert.ToString(reverseLine[1]);
        }

        public Boolean prepareTest()
        {

            appDir = System.IO.Directory.GetCurrentDirectory(); //主程式所在目錄

            appDir = System.Windows.Forms.Application.StartupPath;

            string configFilePath = appDir + "\\" + Config_Dir + "\\" + Config_File_Name;

            ConfigFile configFile = new ConfigFile(configFilePath);
            try
            {
                configFile.prepareReader();

                coreMethod = configFile.readConfig("Core_Method");
                ruleCountWin = Convert.ToInt32(configFile.readConfig("Rule_Count_Win"));
                ruleCountLose = Convert.ToInt32(configFile.readConfig("Rule_Count_Lose"));
                ruleCountReverse = Convert.ToInt32(configFile.readConfig("Rule_Count_Reverse"));
                runCount = Convert.ToInt32(configFile.readConfig("Run_Count"));
                rulePeriod = Convert.ToInt32(configFile.readConfig("Rule_Period"));
                maxLoss = configFile.readConfig("Max_Loss");
                sourceDir = configFile.readConfig("Source_Dir");
                ratio = Convert.ToDouble(configFile.readConfig("Ratio"));

                checkCount = Convert.ToInt32(configFile.readConfig("Check_Count"));

                testVersion = configFile.readConfig("Version");

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

                lots = configFile.readConfig("Lots");

                lotArray = lots.Split(',');

            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.StackTrace);
            }

            //-----------------------------------------------------------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------------------------------------------------------

            StrategyFile strategyInstance = StrategyFile.getInstance();



            Boolean isRuleReady = false;

            if (TradeManager.Core_Method_1.Equals(coreMethod) || TradeManager.Core_Method_2.Equals(coreMethod))
            {

                isRuleReady = strategyInstance.dealStrategyRule(appDir, "TestStrategy.txt");

                this.winLine = strategyInstance.getWinLine();

                this.loseLine = strategyInstance.getLoseLine();

                this.reverseLine = strategyInstance.getReverseLine();

            }
            else if (TradeManager.Core_Method_3.Equals(coreMethod))
            {
                isRuleReady = strategyInstance.dealStopRatioRule(appDir, "TestStrategy.txt");

                this.stopRatio = strategyInstance.getStopRatio();
            }

            if (!isRuleReady)
            {
                return false;
            }


            //-----------------------------------------------------------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------------------------------------------------------

            reportDir = appDir + "\\" + Report_Dir + "\\";

            conclusionDir = appDir + "\\" + Conclusion_Dir + "\\";

            sourceFileDir = appDir + "\\" + sourceDir + "\\";




            conclusionReportFileName = conclusionDir + now.Year + "_" + now.Month + "_" + now.Day + "_" + now.Hour + "_" + now.Minute + "_" + now.Second + "_Conclusion.rpt";

            conclusionReport = new TradeFile(conclusionReportFileName);

            conclusionReport.prepareWriter();

            conclusionMsg("使用核心:" + coreMethod);

            conclusionMsg("單日設定最大停損" + maxLoss);

            conclusionMsg("動態停利反轉比率:" + ratio);

            conclusionMsg("下單口數:" + lots);

            conclusionMsg("測試次數:" + runCount);

            conclusionMsg("確定買賣方向的檢查機制，時間間隔次數:" + checkCount);

            conclusionMsg("----------------------------------------------------------------------------------------------");


            if (stopRatio != null)
            {
                conclusionMsg("逆勢動態停利規則 : " + stopRatio.ToString());
            }

            return true;

        }



        public void startTest()
        {

            //if (!prepareTest())
            //{
            //    conclusionMsg("規則檔案讀取失敗!");

            //    return;
            //}

            if (coreMethod.Equals(Core_Method_3))
            {
                startTest(1001);
            }
            else
            {

                try
                {



                    int j = 0;



                    Dictionary<int, int> initialLoseLine = new Dictionary<int, int>();

                    for (int xx = 1; xx <= loseLine.Count; xx++)
                    {
                        initialLoseLine[xx] = loseLine[xx];
                    }

                    Dictionary<int, int> initialWinLine = new Dictionary<int, int>();

                    for (int xx = 1; xx <= winLine.Count; xx++)
                    {
                        initialWinLine[xx] = winLine[xx];
                    }

                    //---------------------------------------------------------------------------

                    //for (int qq = 1; qq <= ruleCountReverse; qq++)
                    {

                        //j = 0;

                        //double tmpReverse = 0;

                        //for (j = 1; j <= reverseLine.Count; j++)
                        //{
                        //    tmpReverse = reverseLine[j] +  rulePeriod;

                        //    reverseLine[j] = tmpReverse;
                        //}    // end 

                        for (int k = 1; k <= ruleCountWin; k++)
                        {
                            j = 0;

                            int tmpWin = 0;

                            for (j = 1; j <= winLine.Count; j++)
                            {
                                tmpWin = winLine[j] +  rulePeriod;

                                winLine[j] = tmpWin;
                            }    // end winLine




                            for (int i = 1; i <= ruleCountLose; i++)
                            {

                                j = 0;

                                int tmpLose = 0;

                                for (j = 1; j <= loseLine.Count; j++)
                                {
                                    tmpLose = loseLine[j] +  rulePeriod;

                                    loseLine[j] = tmpLose;
                                }

                                startTest( k * 1000 + i);


                                if (winLine != null && loseLine != null)
                                {

                                    for (int z = 1; z <= winLine.Count; z++)
                                    {
                                        reportMsg("測試規則LOSE  00" + z + ":" + loseLine[z]);
                                    }

                                    for (int s = 1; s <= winLine.Count; s++)
                                    {
                                        reportMsg("測試規則WIN   00" + s + ":" + winLine[s]);
                                    }

                                }

                                if (reverseLine != null)
                                {
                                    for (int y = 1; y <= reverseLine.Count; y++)
                                    {
                                        reportMsg("測試規則REVERSE  00" + y + ":" + reverseLine[y]);
                                    }
                                }

                            }//end for i

                            for (int i = 1; i <= loseLine.Count; i++)
                            {
                                loseLine[i] = initialLoseLine[i];
                            }

                        }//end for k

                        for (int i = 1; i <= winLine.Count; i++)
                        {
                            winLine[i] = initialWinLine[i];
                        }

                    }//end for qq

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Source);
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine(e.Message);
                    return;
                }

            }

            stop();

        }

        public void startTest(int guid)
        {

            this.guid = guid;

            try
            {


                testReportFileName = reportDir + now.Year + "_" + now.Month + "_" + now.Day + "_" + now.Hour + "_" + now.Minute + "_" + now.Second + "_";
                if (loseLine != null)
                {
                    testReportFileName += loseLine[1] + "_";
                }
                if (winLine != null)
                {
                    testReportFileName += winLine[1] + "_";
                }

                if (reverseLine != null)
                {
                    testReportFileName += reverseLine[1] + "_";
                }

                testReportFileName += guid + ".rpt";


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


            int[] profitRange = new int[45];//獲利的範圍

            double maxWinPureProfit = 0;//單日最大獲利

            double maxLosePureProfit = 0;//單日最大賠錢




            for (int i = 0; i < 45; i++)
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

                    if (checkCount > 0)
                    {
                        manager.setCheckCount(checkCount);
                    }

                    manager.setDebugEnabled(DEBUG);

                    manager.setStopRatio(stopRatio);

                    manager.setRatio(ratio);

                    manager.setCore(coreMethod);

                    manager.setLots(lotArray);

                    manager.setWinLine(winLine);

                    manager.setLoseLine(loseLine);

                    manager.setReverseLine(reverseLine);

                    manager.setSourceFile(oFileList[j]);

                    oneDayProfit = manager.startTrade();

                    int tmpMaxLot = manager.getMaxLot();

                    if (tmpMaxLot > maxLot)
                    {
                        maxLot = tmpMaxLot;
                    }

                    winCountInOneDayTradeRunManyTimes += manager.getWinCount();

                    loseCountInOneDayTradeRunManyTimes += manager.getLoseCount();

                    oneDayPureProfit = oneDayProfit * valuePerPoint - (manager.getWinCount() + manager.getLoseCount()) * cost;

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

                        maxWinPureProfitFileName = oFileList[j].getFileName();
                    }

                    if (oneDayPureProfit < maxLosePureProfit)
                    {
                        maxLosePureProfit = oneDayPureProfit;

                        maxLosePureProfitFileName = oFileList[j].getFileName();
                    }

                    if (oneDayPureProfit > 0 && oneDayPureProfit < 2000)
                    {

                        profitRange[0]++;
                    }
                    else if (oneDayPureProfit > 100000)
                    {
                        profitRange[38]++;
                    }
                    else if (oneDayPureProfit > 90000)
                    {
                        profitRange[37]++;
                    }
                    else if (oneDayPureProfit > 80000)
                    {
                        profitRange[36]++;
                    }
                    else if (oneDayPureProfit > 70000)
                    {
                        profitRange[35]++;
                    }
                    else if (oneDayPureProfit > 60000)
                    {
                        profitRange[34]++;
                    }
                    else if (oneDayPureProfit > 50000)
                    {
                        profitRange[33]++;
                    }
                    else if (oneDayPureProfit > 40000)
                    {
                        profitRange[32]++;
                    }
                    else if (oneDayPureProfit > 30000)
                    {
                        profitRange[31]++;
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
                    else if (oneDayPureProfit < -50000)
                    {
                        profitRange[41]++;
                    }
                    else if (oneDayPureProfit < -40000)
                    {
                        profitRange[40]++;
                    }
                    else if (oneDayPureProfit < -30000)
                    {
                        profitRange[39]++;
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

                reportMsg(oFileList[j].getFullPath() + "交易結束，單日獲利口數 : " + winCountInOneDayTradeRunManyTimes);

                reportMsg(oFileList[j].getFullPath() + "交易結束，單日賠錢口數 : " + loseCountInOneDayTradeRunManyTimes);

                reportMsg(oFileList[j].getFullPath() + "交易結束，單日獲利口數的總比率 : " + Convert.ToDouble(winCountInOneDayTradeRunManyTimes) / ((Convert.ToDouble(winCountInOneDayTradeRunManyTimes) + Convert.ToDouble(loseCountInOneDayTradeRunManyTimes))) * 100 + " %");

                reportMsg("----------------------------------------------------------------------------------------------");
                reportMsg("----------------------------------------------------------------------------------------------");
                reportMsg("----------------------------------------------------------------------------------------------");

            }//end for fileList


            reportMsg(" 測試編號 : " + guid);
            reportMsg(" 每個交易日的測試次數 : " + runCount);

            reportMsg("獲利口數 : " + totalWinCountRumManyTimes);
            reportMsg("賠錢口數 : " + totalLoseCountRunManyTimes);
            reportMsg("交易結束，獲利口數的總比率 : " + Convert.ToDouble(totalWinCountRumManyTimes) / ((Convert.ToDouble(totalWinCountRumManyTimes) + Convert.ToDouble(totalLoseCountRunManyTimes))) * 100 + " %");


            reportMsg("獲利日數 : " + winDayCount);
            reportMsg("賠錢日數" + loseDayCount);

            winDayRate = Convert.ToDouble(winDayCount) / ((Convert.ToDouble(winDayCount) + Convert.ToDouble(loseDayCount)));

            reportMsg("交易結束，獲利日數的總比率 : " + winDayRate * 100 + " %");


            reportMsg("單日最大獲利 : " + maxWinPureProfit);
            reportMsg("最大獲利 是哪一天: " + maxWinPureProfitFileName);
            reportMsg("單日最大賠錢 : " + maxLosePureProfit);
            reportMsg("最大賠錢 是哪一天: " + maxLosePureProfitFileName);

            reportMsg("單日設定最大停損" + maxLoss);


            reportMsg("曾經最大交易口數 : " + maxLot);

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
            reportMsg("獲利三萬以上次數 : " + profitRange[31]);
            reportMsg("獲利四萬以上次數 : " + profitRange[32]);
            reportMsg("獲利五萬以上次數 : " + profitRange[33]);
            reportMsg("獲利六萬以上次數 : " + profitRange[34]);
            reportMsg("獲利七萬以上次數 : " + profitRange[35]);
            reportMsg("獲利八萬以上次數 : " + profitRange[36]);
            reportMsg("獲利九萬以上次數 : " + profitRange[37]);
            reportMsg("獲利十萬以上次數 : " + profitRange[38]);
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
            reportMsg("賠錢三萬以上次數 : " + profitRange[39]);
            reportMsg("賠錢四萬以上次數 : " + profitRange[40]);
            reportMsg("賠錢五萬以上次數 : " + profitRange[41]);

            reportMsg("----------------------------------------------------------------------------------------------");

            reportMsg("----------------------------------------------------------------------------------------------");



            if (pureProfit > 500 || winDayRate > 0.5)
            {
                conclusionMsg("----------------------------------------------------------------------------------------------");

                conclusionMsg("程式版本 :" + testVersion);

                conclusionMsg("交易結束，獲利口數的總比率 : " + Convert.ToDouble(totalWinCountRumManyTimes) / ((Convert.ToDouble(totalWinCountRumManyTimes) + Convert.ToDouble(totalLoseCountRunManyTimes))) * 100 + " %");

                conclusionMsg("交易結束，獲利日數的總比率 : " + Convert.ToDouble(winDayCount) / ((Convert.ToDouble(winDayCount) + Convert.ToDouble(loseDayCount))) * 100 + " %");

                if (loseLine != null)
                {
                    for (int i = 1; i <= winLine.Count; i++)
                    {
                        conclusionMsg("測試規則LOSE  00" + i + ":" + loseLine[i]);
                    }

                }

                if (winLine != null)
                {
                    for (int i = 1; i <= winLine.Count; i++)
                    {
                        conclusionMsg("測試規則WIN   00" + i + ":" + winLine[i]);
                    }


                }

                if (reverseLine != null)
                {
                    for (int i = 1; i <= winLine.Count; i++)
                    {
                        conclusionMsg("測試規則REVERSE  00" + i + ":" + reverseLine[i]);
                    }

                }

                conclusionMsg(runCount * oFileList.Count + "次，扣除手續費後，總平均利潤 : " + pureProfit);
                conclusionMsg("----------------------------------------------------------------------------------------------");
            }
        }//end startTrade(guid)



        private void showMsg(TradeFile file, string msg)
        {
            try
            {
                if (DEBUG)
                {
                    Console.WriteLine(msg);
                }

                if (isReport)
                {
                    file.writeLine(msg);
                }
            }
            catch (Exception e)
            {
                throw e;
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
