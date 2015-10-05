using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradeUtility;

namespace AutoTrade
{
    class TradeMaster
    {

        //Const 常數區
        //-------------------------------------------------------------------------------------------------------------
        //        

        const Boolean DEBUG = true;

        const Boolean END_TRADE = true;

        const Boolean NEXT_TRADE = false;

        const int Random_Seed = 888;//隨機參數種子

        const int SELL = 2;//交易方式:賣

        const int BUY = 1;//交易方式:買

        const int MaxTradeCount = 100;//最大交易次數        

        const double MaxProfitLoss = -5000;//最大虧損水平線

        enum TradeType : int { SELL, BUY };

        const String strategyFilePath = "C:/Trader/Strategy.txt";

        //const int SellOrBuyCheckPeriod = 60;//交易買賣方向的檢查時間間隔,60秒前

        const int ActiveProfitStartPeriod = 5;//檢查動態停利啟動條件的時間基準，5秒前

        const int ActiveProfitCheckPeriod = 1;//動態停利的檢查時間間隔,一分鐘後

        const int ActiveProfitPoint = 30;//動態停利的啟動條件

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Boolean變數。
        /// </summary>

        Boolean isStopTrade = false;//是否停止今日交易

        Boolean isStartOrder = false;//是否開始下單

        Boolean isPrevWin = false;

        Boolean isPrevLose = false;

        Boolean isActiveCheckProfit = false;//是否動態停利

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 變數。
        /// </summary>
        ///         
        int number = 0;//超過檢查時間的次數

        double tempOneProfit = 99999;//單筆暫時獲利

        double minTradePoint = 99999;//市場最低價

        double maxTradePoint = 0;//市場最高價

        double offsetTradePoint;//最高與最低價格差異區間値 = maxTradePoint - minTradePoint

        int tradeCount = 0;  //交易次數        

        double oneProfit = 0;//單筆利潤

        double totalProfit = 0;//總利潤

        OriginalRecord record = new OriginalRecord();//檔案的一行紀錄        

        String tradeTime = "";//交易時間點

        double orderPrice = 0;//下單交易價格

        double evenPrice = 0;//平倉交易價格        

        DateTime tradeDateTime;//交易時間點

        //DateTime secondsBeforeTradeTime;//交易前X秒，判斷買或賣

        DateTime[] minuteBeforeTradeTime;//交易前X秒，判斷買或賣

        DateTime secondAfterTradeToActiveCheck;//交易後X秒，例如五秒鐘內利潤擴大30點開始動態停利

        DateTime minuteAfterStartActiveProfit;//開始動態停利檢查，每一分鐘一次

        List<OriginalRecord> recordList = null;//所有交易紀錄

        Dictionary<int, int> loseLine;  //認賠的底線

        Dictionary<int, int> winLine;  //停利的底線

        int nowStrategyCount = 1; //目前使用哪一行的停損停利規則

        int maxStrategyCount = 0; //停損停利規則最大有幾種

        int winCount = 0;//獲利次數

        int loseCount = 0;//賠錢次數

        DateTime isStopTradeTime;//今日交易結束時間

        int[] SellOrBuyCheckPeriod;//交易買賣方向的檢查時間間隔

        int nowTradeType = 0;//交易方式賣或是買

        int prevTradeType = 0;//上一次交易方式賣或是買

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------

        String fileName = "";

        String dir = "C:/Trader/";

        TradeFile allTradeOutputFile;          //當天所有交易紀錄
        TradeFile strategyFile;                        //策略檔
        TradeFile tradeRecordFile;                //自動交易紀錄檔


        String refPrice;                    //參考價 
        String openPrice;                   //開盤價
        String highPrice;                   //最高價
        String lowPrice;                    //最低價
        String upPrice;                     //漲停價
        String downPrice;                   //跌停價
        String matchTime;                   //成交時間
        String matchPrice;                  //"成交價位"
        String matchQuantity;               // "成交數量" 
        String totalMatchQuantity;          //"總成交量" 
        String bestBuyFive;                 //"買五"
        String bestSellFive;                //"賣五"


        String outputLine = "";//寫入RPT檔案的內容

        String tradeRecordFileName = "";//自動交易紀錄

        OriginalRecord befofeRecord;


        

        public TradeMaster()
        {
        }


        public void prepareReady()
        {
            

            SellOrBuyCheckPeriod = new int[5];

            isStopTrade = false;//是否停止今日交易

            isStartOrder = false;//是否開始下單

            isPrevWin = false;//上一次交易是否獲利

            DateTime now = System.DateTime.Now;

            isStopTradeTime = new DateTime(now.Year, now.Month, now.Day, 13, 44, 0);

            fileName = dir + "AllOutput_" + now.Year + "_" + now.Month + "_" + now.Day + ".rpt";

            allTradeOutputFile = new TradeFile(fileName);

            allTradeOutputFile.prepareWriter();

            tradeRecordFileName = dir + "TradeRecord_" + now.Year + "_" + now.Month + "_" + now.Day + ".rpt";

            tradeRecordFile = new TradeFile(tradeRecordFileName);

            tradeRecordFile.prepareWriter();

            loseLine = new Dictionary<int, int>();

            winLine = new Dictionary<int, int>();

            strategyFile = new TradeFile(strategyFilePath);

            strategyFile.prepareReader();

            getStrategyLine();

            recordList = new List<OriginalRecord>();

            befofeRecord = new OriginalRecord();

            RecordScanner.setRecordList(recordList);

            debugMsg(now + "------<<開始交易>>");

        }

        public void stop()
        {
            allTradeOutputFile.close();

            strategyFile.close();

            tradeRecordFile.close();
        }

        public void process(AxYuantaQuoteLib._DYuantaQuoteEvents_OnGetMktAllEvent marketEvent)
        {

            refPrice = marketEvent.refPri;
            openPrice = marketEvent.openPri;
            highPrice = marketEvent.highPri;
            lowPrice = marketEvent.lowPri;
            upPrice = marketEvent.upPri;
            downPrice = marketEvent.dnPri;

            matchTime = marketEvent.matchTime != "" ? (string.Format("{0}:{1}:{2}.{3}", marketEvent.matchTime.Substring(0, 2), marketEvent.matchTime.Substring(2, 2), marketEvent.matchTime.Substring(4, 2), marketEvent.matchTime.Substring(6, 3))) : "";

            matchPrice = marketEvent.matchPri;
            matchQuantity = marketEvent.matchQty;
            totalMatchQuantity = marketEvent.tolMatchQty;
            bestBuyFive = marketEvent.bestBuyPri + marketEvent.bestBuyQty;
            bestSellFive = marketEvent.bestSellPri + marketEvent.bestSellQty;


            try
            {
                outputLine = matchTime + "," + matchPrice + "," + matchQuantity;

                allTradeOutputFile.writeLine(outputLine);

                if (!isStopTrade)
                {

                    isStopTrade = startTrade(matchTime, matchPrice, matchQuantity);

                }

            }
            catch (IOException e)
            {
                throw e;
            }
        }


        private void getStrategyLine()//讀取停損停利規則檔
        {

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
        }

        private double getOffsetPoint()//取得最大最小指數之差額
        {
            if (record.TradePrice < minTradePoint)
            {
                minTradePoint = record.TradePrice;
            }

            if (record.TradePrice > maxTradePoint)
            {
                maxTradePoint = record.TradePrice;
            }

            offsetTradePoint = maxTradePoint - minTradePoint;

            return offsetTradePoint;
        }

        private void dealStrategyCount(int winCount)//依照獲歷次數，來定停損停利範圍
        {
            if (winCount % 2 == 0)
            {

                nowStrategyCount = (winCount / 2) + 1;

                if (winCount > maxStrategyCount)
                {
                    nowStrategyCount = maxStrategyCount;

                    return;
                }

            }
        }


        private void dealStrategyCount(double basePoint)
        //依照目前獲得的利潤，來定停損停利範圍
        //或是，依照最大最小指數的差額，來定停損停利範圍
        {
            nowStrategyCount = 1;

            if (basePoint > loseLine[maxStrategyCount])
            {
                nowStrategyCount = maxStrategyCount;

                return;
            }

            for (int i = 1; i < maxStrategyCount; i++)
            {
                if (basePoint > loseLine[i] && basePoint <= loseLine[i + 1])
                {

                    nowStrategyCount = i;

                    if (nowStrategyCount > maxStrategyCount)
                    {
                        nowStrategyCount = maxStrategyCount;
                    }
                    break;
                }
            }


        }


        public Boolean startTrade(String matchTime, String matchPrice, String matchQuantity)//回傳値表示是否結束本日交易
        {

            try
            {
                record = new OriginalRecord();

                record.TradeTime = matchTime;

                record.TradeMoment = Convert.ToDateTime(matchTime);

                record.TradeHour = record.TradeMoment.Hour;

                record.TradeMinute = record.TradeMoment.Minute;

                record.TradePrice = Convert.ToDouble(matchPrice);

                record.TradeVolumn = Convert.ToInt32(matchQuantity);

                recordList.Add(record);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return coreLogic();

        }




        private Boolean coreLogic()//回傳値表示是否結束本日交易
        {

            if (isStartOrder == false && (isPrevLose == true || isPrevWin == true || Dice.run(Random_Seed)))
            {

                tradeTime = record.TradeTime;

                tradeDateTime = record.TradeMoment;

                if (isPrevLose == true)
                {
                    if (prevTradeType == TradeType.BUY.GetHashCode())
                    {
                        nowTradeType = prevTradeType = TradeType.SELL.GetHashCode();
                    }
                    else
                    {
                        nowTradeType = prevTradeType = TradeType.BUY.GetHashCode();
                    }
                }
                else if (isPrevWin == true)
                {
                    if (prevTradeType == TradeType.BUY.GetHashCode())
                    {
                        nowTradeType = prevTradeType = TradeType.BUY.GetHashCode();
                    }
                    else
                    {
                        nowTradeType = prevTradeType = TradeType.SELL.GetHashCode();
                    }
                }
                else if (!dealSellOrBuy(record, tradeDateTime))
                {
                    return false;
                }

                secondAfterTradeToActiveCheck = tradeDateTime.AddSeconds(ActiveProfitStartPeriod);//5秒內利潤擴大50點

                minuteAfterStartActiveProfit = tradeDateTime.AddMinutes(1);//開始動態停利檢查，每一分鐘一次

                orderPrice = record.TradePrice;

                if (nowTradeType == TradeType.BUY.GetHashCode())
                {
                    debugMsg("交易方式---->" + TradeType.BUY);
                }
                else if (nowTradeType == TradeType.SELL.GetHashCode())
                {
                    debugMsg("交易方式---->" + TradeType.SELL);
                }

                debugMsg("交易金額---->" + orderPrice);

                debugMsg("交易時間---->" + tradeDateTime);

                dealStrategyCount(winCount);//取得停損停利範圍
                //dealStrategyCount(totalProfit);//取得停損停利範圍     

                isStartOrder = true;//下單啦

            }


            if (isStartOrder == true)//已經開始下單
            {

                if (nowTradeType == TradeType.BUY.GetHashCode())
                {

                    oneProfit = record.TradePrice - orderPrice;

                }
                else if (nowTradeType == TradeType.SELL.GetHashCode())
                {

                    oneProfit = orderPrice - record.TradePrice;
                }


                if (record.TradeHour >= 13 && record.TradeMinute >= 44)
                {

                    evenPrice = record.TradePrice;

                    totalProfit += oneProfit;

                    isStartOrder = false;

                    tradeCount++;

                    if (nowTradeType == TradeType.BUY.GetHashCode())
                    {
                        debugMsg("時間到買入平倉");
                    }
                    else if (nowTradeType == TradeType.SELL.GetHashCode())
                    {
                        debugMsg("時間到賣出平倉");
                    }

                    debugMsg("平倉點數---->" + evenPrice);

                    debugMsg("平倉時間---->" + record.TradeMoment);

                    debugMsg("利潤:" + oneProfit * 50);

                    debugMsg("策略:" + loseLine[nowStrategyCount]);

                    debugMsg("----------------------------------------------------------------------------------------------");

                    return END_TRADE;//結束交易

                }//end 交易時間結束
                else if ((isActiveCheckProfit == false) && (record.TradeMoment > secondAfterTradeToActiveCheck))//還沒開始【動態停利】，檢查時間到了，看看是否要啟動動態停利機制
                {

                    if (oneProfit - tempOneProfit > ActiveProfitPoint)
                    {
                        isActiveCheckProfit = true;//開始動態停利

                        debugMsg("開始動態停利---->" + record.TradeMoment + " ---------->Profit:" + oneProfit + "-----------tempOneProfit:" + tempOneProfit);

                        tempOneProfit = oneProfit;

                        isStartOrder = true;//下單啦

                        return NEXT_TRADE;//繼續下一筆交易行情運算
                    }
                    else
                    {
                        secondAfterTradeToActiveCheck = secondAfterTradeToActiveCheck.AddSeconds(ActiveProfitStartPeriod);//繼續跑XX秒

                        tempOneProfit = oneProfit;
                    }



                }// end 檢查是否開始動態停利

                else if ((isActiveCheckProfit == true))//開始動態停利
                {
                    if ((record.TradeMoment > minuteAfterStartActiveProfit))//到了規定的檢查時間
                    {
                        double reverseLitmit = getReverseLitmit(ActiveProfitPoint, number);//動態停利的停利條件

                        if (oneProfit < tempOneProfit - reverseLitmit)//這短時間內增加的獲利少於規定的點數，預測趨勢將反轉，獲利了結
                        {

                            totalProfit += oneProfit;

                            evenPrice = record.TradePrice;

                            debugMsg("******平倉點數009---->" + evenPrice);

                            debugMsg("******平倉時間---->" + record.TradeMoment);

                            debugMsg("利潤:" + oneProfit * 50);

                            debugMsg("動態停利範圍:" + reverseLitmit);

                            debugMsg("----------------------------------------------------------------------------------------------");

                            return winOut();//獲利出場                               

                        }
                        else
                        {

                            number++;//再跑一個間隔

                            if (number <= 1)//第一次檢查
                            {
                                tempOneProfit = oneProfit;
                            }
                            else
                            {
                                if (tempOneProfit < oneProfit)
                                {
                                    tempOneProfit = oneProfit;
                                }

                            }

                            //debugMsg("----------------------------------------------------------------------------------------------");

                            debugMsg("RUN!!" + minuteAfterStartActiveProfit);

                            //debugMsg("----------------------------------------------------------------------------------------------");

                            minuteAfterStartActiveProfit = minuteAfterStartActiveProfit.AddMinutes(ActiveProfitCheckPeriod);//繼續跑XX秒

                            return NEXT_TRADE;//繼續下一筆交易行情運算
                        }
                    }//開始動態停利，但是還不到檢查的時間
                    else
                    {

                        return NEXT_TRADE;//繼續下一筆交易行情運算

                    }

                }//end 執行動態停利檢查

                else if (nowTradeType == TradeType.BUY.GetHashCode() && (orderPrice - record.TradePrice) > loseLine[nowStrategyCount])
                {//賠了XX點，認賠殺出

                    evenPrice = record.TradePrice;

                    oneProfit = evenPrice - orderPrice;

                    totalProfit += oneProfit;

                    debugMsg("認賠殺出");

                    debugMsg("平倉點數001---->" + evenPrice);

                    debugMsg("平倉時間---->" + record.TradeMoment);

                    debugMsg("利潤:" + oneProfit * 50);

                    debugMsg("策略:" + loseLine[nowStrategyCount]);

                    debugMsg("----------------------------------------------------------------------------------------------");

                    return loseOut();

                }
                else if (nowTradeType == TradeType.SELL.GetHashCode() && (record.TradePrice - orderPrice) > loseLine[nowStrategyCount])
                {
                    //賠了XX點，認賠殺出


                    evenPrice = record.TradePrice;

                    oneProfit = orderPrice - evenPrice;

                    totalProfit += oneProfit;

                    debugMsg("認賠殺出");

                    debugMsg("平倉點數002---->" + evenPrice);

                    debugMsg("平倉時間---->" + record.TradeMoment);

                    debugMsg("利潤:" + oneProfit * 50);

                    debugMsg("策略:" + loseLine[nowStrategyCount]);

                    debugMsg("----------------------------------------------------------------------------------------------");

                    return loseOut();

                }

                else if (nowTradeType == TradeType.BUY.GetHashCode() && (record.TradePrice - orderPrice) > winLine[nowStrategyCount])
                {
                    //賺了XX點，停利出場

                    evenPrice = record.TradePrice;

                    oneProfit = evenPrice - orderPrice;

                    totalProfit += oneProfit;

                    debugMsg("停利出場");

                    debugMsg("平倉點數003---->" + evenPrice);

                    debugMsg("平倉時間---->" + record.TradeMoment);

                    debugMsg("利潤:" + oneProfit * 50);

                    debugMsg("策略:" + winLine[nowStrategyCount]);

                    debugMsg("----------------------------------------------------------------------------------------------");

                    return winOut();

                }

                else if (nowTradeType == TradeType.SELL.GetHashCode() && (orderPrice - record.TradePrice) > winLine[nowStrategyCount])
                {
                    //賺了XX點，停利出場


                    evenPrice = record.TradePrice;

                    oneProfit = orderPrice - evenPrice;

                    totalProfit += oneProfit;

                    debugMsg("停利出場");

                    debugMsg("平倉點數004---->" + evenPrice);

                    debugMsg("平倉時間---->" + record.TradeMoment);

                    debugMsg("利潤:" + oneProfit * 50);

                    debugMsg("策略:" + winLine[nowStrategyCount]);

                    debugMsg("----------------------------------------------------------------------------------------------");

                    return winOut();

                }


            }//下單結束

            if (totalProfit * 50 < MaxProfitLoss)  //已達最大虧損水平線
            {

                return loseOut();
            }


            return false;//繼續下一筆行情交易運算

        }


        private Boolean winOut()//獲利出場
        {
            winCount++;

            isPrevWin = true;

            isPrevLose = false;

            return tradeOut();
        }

        private Boolean loseOut()//認賠出場
        {
            loseCount++;

            isPrevWin = false;

            isPrevLose = true;

            return tradeOut();
        }

        private Boolean tradeOut()//結束單筆交易的動作
        {

            isStartOrder = false;

            tradeCount++;

            isActiveCheckProfit = false;//停止動態停利檢查

            tempOneProfit = 99999;

            number = 0;//超過檢查時間的次數

            befofeRecord = null;

            return NEXT_TRADE;//結束此次交易//繼續下一筆交易行情運算
        }


        private double getReverseLitmit(int interval, int number)//取得反轉點數
        //interval <--每次的範圍
        //number <--第幾次取得
        //第一次 
        {

            double litmit = interval;
            for (int i = 0; i < number; i++)
            {

                litmit = litmit * 0.9;

            }
            return litmit;

        }

        private void debugMsg(String msg)
        {
            if (DEBUG)
            {
                Console.WriteLine(msg);

                tradeRecordFile.writeLine(msg);
            }
        }

        private Boolean dealSellOrBuy(OriginalRecord record, DateTime tradeDateTime)//決定買或賣的方向，回傳値代表計算成功或是失敗
        {
            try
            {

                int checkCount = 5;//檢查5個時間點

                double basePrice;

                minuteBeforeTradeTime = new DateTime[checkCount];

                int[] direction = new int[checkCount];


                for (int i = 0; i < checkCount; i++)
                {
                    minuteBeforeTradeTime[i] = tradeDateTime.AddSeconds(0 - SellOrBuyCheckPeriod[i]);

                    debugMsg("minuteBeforeTradeTime[" + i + "]:" + minuteBeforeTradeTime[i]);
                }

                for (int i = 0; i < checkCount; i++)
                {

                    try
                    {
                        befofeRecord = RecordScanner.getRecordMinuteBeforeOrAfter(minuteBeforeTradeTime[i]);//XX鐘前的交易紀錄

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("掃描失敗:" + e.Message + "---" + e.StackTrace + "---" + e.Source);
                    }

                    if (befofeRecord == null)
                    {
                        return false;
                    }

                    basePrice = befofeRecord.TradePrice;//交易基準                  

                    debugMsg("basePrice:" + basePrice);

                    if ((record.TradePrice - basePrice) > 0)//目前交易金額大於XX分鐘前的交易金額
                    {
                        direction[i] = TradeType.SELL.GetHashCode();
                    }
                    else
                    {
                        direction[i] = TradeType.BUY.GetHashCode();
                    }

                }//end for

                for (int i = checkCount - 1; i > 0; i--)
                {
                    if (direction[i] != direction[i - 1]) { return false; }
                }

                nowTradeType = direction[0];

                for (int i = 0; i < checkCount; i++)
                {
                    debugMsg("direction[" + i + "]:" + direction[i]);
                }

                debugMsg("nowTradeType : " + nowTradeType);

                return true;

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

            }

            return false;

        }


    }
}
