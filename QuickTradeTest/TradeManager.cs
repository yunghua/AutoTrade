using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeUtility;

namespace QuickTradeTest
{
    class TradeManager
    {




        //Const 常數區
        //-------------------------------------------------------------------------------------------------------------
        //

        const int Array_Begin_Index = 0;

        const Boolean DEBUG = false;

        const int MaxTradeCount = 100;//最大交易次數        

        const double MaxProfitLoss = -50000;//最大虧損水平線

        const int Random_Seed = 888;//隨機參數種子

        enum TradeType : int { SELL, BUY };//交易方式:買或賣



        //const int SellOrBuyCheckPeriod005 = 9;//交易買賣方向的檢查時間間隔,25秒前

        //const int SellOrBuyCheckPeriod004 = 7;//交易買賣方向的檢查時間間隔,20秒前     

        //const int SellOrBuyCheckPeriod003 = 5;//交易買賣方向的檢查時間間隔,15秒前

        //const int SellOrBuyCheckPeriod002 = 4;//交易買賣方向的檢查時間間隔,10秒前

        //const int SellOrBuyCheckPeriod001 = 3;//交易買賣方向的檢查時間間隔,5秒前

        const int ActiveProfitStartPeriod = 5;//檢查動態停利啟動條件的時間基準，5秒前

        const int ActiveProfitCheckPeriod = 1;//動態停利的檢查時間間隔,一分鐘後

        const int ActiveProfitPoint = 30;//動態停利的啟動條件

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Boolean變數。
        /// </summary>

        //Boolean isStopTrade = false;//是否停止今日交易

        Boolean isStartOrder = false;//是否開始下單

        Boolean isPrevWin = false;

        Boolean isPrevLose = false;

        Boolean isActiveCheckProfit = false;//是否動態停利

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 變數。
        /// </summary>
        ///         

        int lotIndex = 0;//交易口數陣列的第幾個

        int[] SellOrBuyCheckPeriod;//交易買賣方向的檢查時間間隔

        int number = 0;//超過檢查時間的次數

        double tempOneProfit = 99999;//單筆暫時獲利

        double minTradePoint = 99999;//市場最低價

        double maxTradePoint = 0;//市場最高價

        double offsetTradePoint;//最高與最低價格差異區間値 = maxTradePoint - minTradePoint

        int tradeCount = 0;  //交易次數        

        double oneProfit = 0;//單筆利潤

        double totalProfit = 0;//總利潤

        String nowLine = "";//目前檔案跑到哪一行

        OriginalRecord record = new OriginalRecord();//檔案的一行紀錄        

        String tradeTime = "";//交易時間點

        double orderPrice = 0;//下單交易價格

        double evenPrice = 0;//平倉交易價格

        int nowTradeType = 0;//交易方式賣或是買

        int prevTradeType = 0;//上一次交易方式賣或是買

        DateTime tradeDateTime;//交易時間點

        DateTime[] minuteBeforeTradeTime;//交易前X秒，判斷買或賣

        DateTime secondAfterTradeToActiveCheck;//交易後X秒，例如五秒鐘內利潤擴大30點開始動態停利

        DateTime minuteAfterStartActiveProfit;//開始動態停利檢查，每一分鐘一次

        List<OriginalRecord> recordList = null;//所有交易紀錄

        Dictionary<int, int> loseLine;  //認賠的底線

        Dictionary<int, int> winLine;  //停利的底線

        int nowStrategyCount = 1; //目前使用哪一行的停損停利規則

        //int maxStrategyCount = 0; //停損停利規則最大有幾種

        int winCount = 0;//獲利次數

        int loseCount = 0;//賠錢次數

        //TradeFile strategyFile;

        TradeFile sourceFile;

        OriginalRecord befofeRecord;


        string[] lotArray;//獲利加碼的設定

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 程式區。
        /// </summary>
        /// 

        public void setLots(string[] lots)
        {
            lotArray = lots;
        }

        public void setLoseLine(Dictionary<int, int> lose)
        {
            loseLine = lose;
        }

        public void setWinLine(Dictionary<int, int> win)
        {
            winLine = win;
        }


        public TradeManager()
        {

            OriginalRecord befofeRecord = new OriginalRecord();

            SellOrBuyCheckPeriod = new int[5];

            SellOrBuyCheckPeriod[4] = 25;//交易買賣方向的檢查時間間隔,25秒前

            SellOrBuyCheckPeriod[3] = 20;//交易買賣方向的檢查時間間隔,20秒前     

            SellOrBuyCheckPeriod[2] = 15;//交易買賣方向的檢查時間間隔,15秒前

            SellOrBuyCheckPeriod[1] = 10;//交易買賣方向的檢查時間間隔,10秒前

            SellOrBuyCheckPeriod[0] = 5;//交易買賣方向的檢查時間間隔,5秒前

        }



        private double getOffsetPoint()//取得最大最小指數之差額
        {

            try
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
            catch (Exception ex)
            {

                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

            }
            return -1;
        }



        private void dealStrategyCount(int count)//依照獲利 或是 賠錢次數，來定停損停利範圍
        {

            int baseCount = 1;//幾次之後跳下一階

            try
            {
                if (count % baseCount == 0)
                {

                    nowStrategyCount = (count / baseCount) + 1;

                    if (count >= winLine.Count)
                    {
                        nowStrategyCount = winLine.Count;

                        return;
                    }

                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

            }
        }


        private void dealStrategyCount(double basePoint)
        //依照目前獲得的利潤，來定停損停利範圍
        //或是，依照最大最小指數的差額，來定停損停利範圍
        {
            try
            {
                nowStrategyCount = 1;

                if (basePoint > loseLine[loseLine.Count])
                {
                    nowStrategyCount = loseLine.Count;

                    return;
                }

                for (int i = 1; i < loseLine.Count; i++)
                {
                    if (basePoint > loseLine[i] && basePoint <= loseLine[i + 1])
                    {

                        nowStrategyCount = i;

                        if (nowStrategyCount > loseLine.Count)
                        {
                            nowStrategyCount = loseLine.Count;
                        }
                        break;
                    }
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

            }

        }


        public void setSourceFile(TradeFile sourceFile)
        {
            this.sourceFile = sourceFile;

            sourceFile.prepareReader();
        }


        public double startTrade()
        {

            recordList = new List<OriginalRecord>();

            RecordScanner.setRecordList(recordList);

            return coreLogic();

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

        private double coreLogic()
        {
            while (sourceFile.hasNext())
            {

                try
                {

                    nowLine = sourceFile.getLine();

                    record = OriginalRecordConverter.getOriginalRecord(nowLine);


                    recordList.Add(record);

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
                            continue;
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


                    else if (isStartOrder == true)//已經開始下單
                    {

                        if (nowTradeType == TradeType.BUY.GetHashCode())
                        {

                            oneProfit = record.TradePrice - orderPrice;

                        }
                        else if (nowTradeType == TradeType.SELL.GetHashCode())
                        {

                            oneProfit = orderPrice - record.TradePrice;
                        }



                        if ((isActiveCheckProfit == false) && (record.TradeMoment > secondAfterTradeToActiveCheck))//還沒開始【動態停利】，檢查時間到了，看看是否要啟動動態停利機制
                        {

                            if (oneProfit - tempOneProfit > ActiveProfitPoint)
                            {
                                isActiveCheckProfit = true;//開始動態停利

                                debugMsg("開始動態停利---->" + record.TradeMoment + " ---------->Profit:" + oneProfit + "-----------tempOneProfit:" + tempOneProfit);

                                tempOneProfit = oneProfit;

                                isStartOrder = true;//下單啦

                                continue;
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

                                    winOut();//獲利出場 

                                    if (nowTradeType == TradeType.BUY.GetHashCode())
                                    {
                                        prevTradeType = TradeType.BUY.GetHashCode();
                                    }
                                    else
                                    {
                                        prevTradeType = TradeType.SELL.GetHashCode();
                                    }

                                    continue;//

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

                                    continue;
                                }
                            }//開始動態停利，但是還不到檢查的時間
                            else
                            {

                                continue;

                            }

                        }//end 執行動態停利檢查

                        else if (nowTradeType == TradeType.BUY.GetHashCode() && (orderPrice - record.TradePrice) > loseLine[nowStrategyCount])
                        {//賠了XX點，認賠殺出

                            evenPrice = record.TradePrice;

                            oneProfit = evenPrice - orderPrice;

                            oneProfit *= Convert.ToInt32(lotArray[lotIndex]);

                            lotIndex = Array_Begin_Index;

                            totalProfit += oneProfit;

                            debugMsg("認賠殺出");

                            debugMsg("平倉點數001---->" + evenPrice);

                            debugMsg("平倉時間---->" + record.TradeMoment);

                            debugMsg("利潤:" + oneProfit * 50);

                            debugMsg("停損策略:" + loseLine[nowStrategyCount]);

                            debugMsg("停利策略:" + winLine[nowStrategyCount]);

                            debugMsg("----------------------------------------------------------------------------------------------");

                            loseOut();

                            prevTradeType = TradeType.BUY.GetHashCode();

                        }
                        else if (nowTradeType == TradeType.SELL.GetHashCode() && (record.TradePrice - orderPrice) > loseLine[nowStrategyCount])
                        {
                            //賠了XX點，認賠殺出


                            evenPrice = record.TradePrice;

                            oneProfit = orderPrice - evenPrice;

                            oneProfit *= Convert.ToInt32(lotArray[lotIndex]);

                            lotIndex = Array_Begin_Index;

                            totalProfit += oneProfit;

                            debugMsg("認賠殺出");

                            debugMsg("平倉點數002---->" + evenPrice);

                            debugMsg("平倉時間---->" + record.TradeMoment);

                            debugMsg("利潤:" + oneProfit * 50);

                            debugMsg("停損策略:" + loseLine[nowStrategyCount]);

                            debugMsg("停利策略:" + winLine[nowStrategyCount]);

                            debugMsg("----------------------------------------------------------------------------------------------");

                            loseOut();

                            prevTradeType = TradeType.SELL.GetHashCode();

                        }

                        else if (nowTradeType == TradeType.BUY.GetHashCode() && (record.TradePrice - orderPrice) > winLine[nowStrategyCount])
                        {
                            //賺了XX點，停利出場

                            evenPrice = record.TradePrice;

                            oneProfit = evenPrice - orderPrice;

                            oneProfit *= Convert.ToInt32(lotArray[lotIndex]);

                            lotIndex++;

                            if (lotIndex >= lotArray.Length)
                            {
                                lotIndex = lotArray.Length-1;
                            }

                            totalProfit += oneProfit;

                            debugMsg("停利出場");

                            debugMsg("平倉點數003---->" + evenPrice);

                            debugMsg("平倉時間---->" + record.TradeMoment);

                            debugMsg("利潤:" + oneProfit * 50);

                            debugMsg("停損策略:" + loseLine[nowStrategyCount]);

                            debugMsg("停利策略:" + winLine[nowStrategyCount]);

                            debugMsg("----------------------------------------------------------------------------------------------");

                            winOut();

                            prevTradeType = TradeType.BUY.GetHashCode();

                        }

                        else if (nowTradeType == TradeType.SELL.GetHashCode() && (orderPrice - record.TradePrice) > winLine[nowStrategyCount])
                        {
                            //賺了XX點，停利出場


                            evenPrice = record.TradePrice;

                            oneProfit = orderPrice - evenPrice;

                            oneProfit *= Convert.ToInt32(lotArray[lotIndex]);

                            lotIndex++;

                            if (lotIndex >= lotArray.Length)
                            {
                                lotIndex = lotArray.Length-1;
                            }

                            totalProfit += oneProfit;

                            debugMsg("停利出場");

                            debugMsg("平倉點數004---->" + evenPrice);

                            debugMsg("平倉時間---->" + record.TradeMoment);

                            debugMsg("利潤:" + oneProfit * 50);

                            debugMsg("停損策略:" + loseLine[nowStrategyCount]);

                            debugMsg("停利策略:" + winLine[nowStrategyCount]);

                            debugMsg("----------------------------------------------------------------------------------------------");

                            winOut();

                            prevTradeType = TradeType.SELL.GetHashCode();

                        }


                    }//下單結束

                    if (totalProfit * 50 < MaxProfitLoss)  //已達最大虧損水平線
                    {

                        break;
                    }

                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Source);
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);

                }

            }//end of while

            //if (record.TradeHour >= 13 && record.TradeMinute >= 44)//交易時間截止
            if (isStartOrder == true)
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

                debugMsg("停損策略:" + loseLine[nowStrategyCount]);

                debugMsg("停利策略:" + winLine[nowStrategyCount]);

                debugMsg("----------------------------------------------------------------------------------------------");

                if (oneProfit > 0)
                {
                    winCount++;
                }
                else
                {
                    loseCount++;
                }

                return totalProfit;

            }//end 交易時間截止

            return totalProfit;

        }

        private void winOut()//獲利出場
        {
            winCount++;

            isPrevWin = true;

            isPrevLose = false;

            tradeOut();
        }

        private void loseOut()//認賠出場
        {
            loseCount++;

            isPrevWin = false;

            isPrevLose = true;

            tradeOut();
        }

        private void tradeOut()//結束單筆交易的動作
        {

            isStartOrder = false;

            tradeCount++;

            isActiveCheckProfit = false;//停止動態停利檢查

            tempOneProfit = 99999;

            number = 0;//超過檢查時間的次數
        }


        private double getReverseLitmit(int interval, int number)//取得反轉點數
        //interval <--每次的範圍
        //number <--第幾次取得
        //第一次 
        {
            try
            {

                double litmit = interval;

                for (int i = 0; i < number; i++)
                {

                    litmit = litmit * 0.9;

                }
                return litmit;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                return 100;

            }

        }

        private void debugMsg(String msg)
        {
            if (DEBUG)
            {
                Console.WriteLine(msg);
            }
        }

        public int getWinCount()
        {
            return winCount;
        }

        public int getLoseCount()
        {
            return loseCount;
        }

    }
}
