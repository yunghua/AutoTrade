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

        const int Stage_New = 1;//初始化
        const int Stage_Order_Login_Start = 2;//登入下單API開始
        const int Stage_Order_Login_Success = 3;//登入下單API成功
        const int Stage_Order_New_Start = 4;//新倉開始下單
        const int Stage_Order_New_Success = 5;//新倉回報下單成功        
        const int Stage_Order_New_Fail = 6;//新倉回報下單失敗  
        const int Stage_Order_Not_Process = 7;//無法下單，可能是尚未登入完成
        const int Stage_Order_Even_Start = 8;//平倉倉開始下單
        const int Stage_Order_Even_Success = 9;//平倉回報下單成功     
        const int Stage_Order_Even_Fail = 10;//平倉回報下單失敗  
        const int Stage_Order_Fail = 11;//回報下單失敗  
        const int Stage_Order_Time_Up = 12;//時間到停止下單
        const int Stage_End = 13;//結束

        private const int Direction_None = 0;//無方向
        private const int Direction_LowToHigh = 1;//由低向高之後反轉
        private const int Direction_HighToLow = 2;//由高向低之後反轉


        public const string Core_Method_1 = "Core_Method_1";//獲利加碼

        public const string Core_Method_2 = "Core_Method_2";//動態停利

        public const string Core_Method_3 = "Core_Method_3";//逆勢動態停利

        const int Array_Begin_Index = 0;



        const int Random_Seed = 8888;//隨機參數種子

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

        Boolean debugEnabled = false;//是否顯示除錯訊息

        Boolean reverseEnabled = true;//是否買賣方向轉向

        Boolean winOutEnabled = false;//是否要停利

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 變數。
        /// </summary>
        ///         


        DateTime maxContinueWinMoneyTime;

        public DateTime MaxContinueWinMoneyTime
        {
            get { return maxContinueWinMoneyTime; }
            set { maxContinueWinMoneyTime = value; }
        }

        DateTime maxContinueLossMoneyTime;

        public DateTime MaxContinueLossMoneyTime
        {
            get { return maxContinueLossMoneyTime; }
            set { maxContinueLossMoneyTime = value; }
        }

        double maxContinueWinMoney = 0.0;//連續最大贏錢

        public double MaxContinueWinMoney
        {
            get { return maxContinueWinMoney; }
            set { maxContinueWinMoney = value; }
        }


        double continueWinMoney = 0.0;//連續賠錢

        double maxContinueLossMoney = 0.0;//連續最大賠錢

        public double MaxContinueLossMoney
        {
            get { return maxContinueLossMoney; }
            set { maxContinueLossMoney = value; }
        }

        double continueLossMoney = 0.0;//連續賠錢

        public double ContinueLossMoney
        {
            get { return continueLossMoney; }
            set { continueLossMoney = value; }
        }

        double maxProfitLoss = -999999;//最大虧損水平線

        string coreMethod = "";//核心方法

        List<OriginalRecord> orderPointList = new List<OriginalRecord>();//動態停利機制中，每達到停利階段就下一次單時，下單的點數

        int activeOrderIndex = 0;//第幾次動態停利下單

        int lotIndex = 0;//交易口數陣列的第幾個

        int[] sellOrBuyCheckPeriod = null;//交易買賣方向的檢查時間間隔

        int number = 0;//超過檢查時間的次數

        double tempOneProfit = 99999;//單筆暫時獲利



        double offsetTradePoint;//最高與最低價格差異區間値 = maxTradePoint - minTradePoint

        int tradeCount = 0;  //交易次數        

        double oneProfit = 0;//單筆利潤

        double totalLoseProfit = 0;//輸的總點數(不包含加碼反轉的)

        double totalProfit = 0;//總利潤

        String nowLine = "";//目前檔案跑到哪一行

        OriginalRecord record = new OriginalRecord();//檔案的一行紀錄        

        String tradeTime = "";//交易時間點

        int orderPrice = 0;//下單交易價格

        int evenPrice = 0;//平倉交易價格

        int nowTradeType = 0;//交易方式賣或是買

        int prevTradeType = 0;//上一次交易方式賣或是買

        DateTime tradeDateTime;//交易時間點

        DateTime[] minuteBeforeTradeTime;//交易前X秒，判斷買或賣

        DateTime secondAfterTradeToActiveCheck;//交易後X秒，例如五秒鐘內利潤擴大30點開始動態停利

        DateTime minuteAfterStartActiveProfit;//開始動態停利檢查，每一分鐘一次

        List<OriginalRecord> recordList = null;//所有交易紀錄

        Dictionary<int, int> loseLine;  //認賠的底線

        Dictionary<int, int> winLine;  //停利的底線

        Dictionary<int, double> reverseLine;  //動態停利反轉的底線

        int nowWinLineIndex = 0; //目前使用哪一行的停利規則

        int nowLoseLineIndex = 0; //目前使用哪一行的停損規則        

        int winCount = 0;//獲利次數

        int loseCount = 0;//賠錢次數

        //TradeFile strategyFile;

        TradeFile sourceFile;

        OriginalRecord befofeRecord = null;

        string[] lotArray;//獲利加碼的設定

        double ratio = 0.9;//動態停利的反轉比率，小於1，越接近1表示要回檔接近停利設定，才會執行停利，也就是越不敏感。

        int valuePerPoint = 50;

        int cost = 68;

        double pureProfit;//純利

        double totalPureProfit;//總純利

        int maxLot = 0;//最大交易口數

        Dictionary<int, int> stopRatio;  //逆勢動態停利的百分比查表，第一個int 是點數間隔，第二個是百分比

        enum LastReversePoint : int { MAX, MIN };//最後一個轉折點是最高點還是最低點        

        //int addTimes = 0;//加碼次數

        List<int> orderPriceList = new List<int>();//下單價位的LIST

        List<int> addList = new List<int>();//加碼的LIST

        int checkCount = 5;//檢查幾個時間間隔，來決定買或是賣

        int lotLimit = 8;//可以下單口數的上限

        public int LotLimit
        {
            get { return lotLimit; }
            set { lotLimit = value; }
        }

        int winVolume = 0;//獲利口數

        public int WinVolume
        {
            get { return winVolume; }
            set { winVolume = value; }
        }

        int loseVolume = 0;//賠錢口數

        public int LoseVolume
        {
            get { return loseVolume; }
            set { loseVolume = value; }
        }

        OriginalRecord maxTradePoint = new OriginalRecord();//本次交易期間最高價

        OriginalRecord minTradePoint = new OriginalRecord();//本次交易期間最低價

        List<TradeFile> sourceFileList = new List<TradeFile>();

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 程式區。
        /// </summary>
        /// 

        public void setCheckCount(int b)
        {
            checkCount = b;
        }

        public void setDebugEnabled(Boolean b)
        {
            debugEnabled = b;
        }

        public void setStopRatio(Dictionary<int, int> stopRatio)
        {
            this.stopRatio = stopRatio;
        }

        public int getMaxLot()
        {
            return maxLot;
        }

        public void setRatio(double ratio)
        {
            this.ratio = ratio;
        }

        public double getRatio()
        {
            return ratio;
        }

        public void setMaxProfitLoss(double maxProfitLoss)//單日最大停損 設定
        {
            this.maxProfitLoss = maxProfitLoss;
        }

        public double getMaxProfitLoss()
        {
            return this.maxProfitLoss;
        }

        public void setCore(string coreMethod)
        {//設定核心
            this.coreMethod = coreMethod;
        }

        public string getCore(string coreMethod)
        {//設定核心
            return this.coreMethod;
        }

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

        public void setReverseLine(Dictionary<int, double> reverse)
        {
            reverseLine = reverse;
        }


        public TradeManager()
        {
            maxTradePoint.TradePrice = 0;

            minTradePoint.TradePrice = 99999;

        }



        private void dealLoseLineIndex(int count)//依照加碼次數次數，來定停利範圍
        {
            if (count <= 0)
            {
                nowLoseLineIndex = 1;
                return;
            }

            //int baseCount = 1;//幾次之後跳下一階

            try
            {
                //if (count % baseCount == 0)
                {

                    //nowStrategyCount = (count / baseCount);

                    nowLoseLineIndex = count;

                    if (count >= loseLine.Count)
                    {
                        nowLoseLineIndex = loseLine.Count;

                        return;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void dealWinLineIndex(int count)//依照加碼次數次數，來定停利範圍
        {
            if (count <= 0)
            {
                nowWinLineIndex = 1;
                return;
            }

            //int baseCount = 1;//幾次之後跳下一階

            try
            {
                //if (count % baseCount == 0)
                {

                    //nowStrategyCount = (count / baseCount);

                    nowWinLineIndex = count;

                    if (count >= winLine.Count)
                    {
                        nowWinLineIndex = winLine.Count;

                        return;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void setSourceFileList(List<TradeFile> fileList)
        {
            this.sourceFileList = fileList;
        }


        public void setSourceFile(TradeFile sourceFile)
        {
            this.sourceFile = sourceFile;

            sourceFile.prepareReader();
        }


        private void prepareRecordList()
        {
            try
            {
                for (int i = 0; i < sourceFileList.Count; i++)
                {

                    sourceFile = sourceFileList[i];

                    sourceFile.prepareReader();

                    while (sourceFile.hasNext())
                    {

                        nowLine = sourceFile.getLine();

                        record = OriginalRecordConverter.getOriginalRecord(nowLine);

                        recordList.Add(record);

                    }//end while

                    sourceFile.close();

                }//end for

                Console.WriteLine("GO");

            }
            catch (Exception e)
            {

                Console.WriteLine(e.StackTrace);
            }

        }

        public double startTrade()
        {

            recordList = new List<OriginalRecord>();

            RecordScanner.setRecordList(recordList);

            if (this.coreMethod.Equals(Core_Method_1))
            {
                prepareRecordList();
                return coreLogic();
            }
            else
            {
                throw new Exception("沒有設定Core Method!!");
            }

        }



        private Boolean dealSellOrBuy(OriginalRecord record, DateTime tradeDateTime)//決定買或賣的方向，回傳値代表計算成功或是失敗
        {
            try
            {

                sellOrBuyCheckPeriod = new int[checkCount];

                int baseTimePeriod = 5;

                int[] basePrice = new int[checkCount];

                minuteBeforeTradeTime = new DateTime[checkCount];

                int[] direction = new int[checkCount];


                for (int i = 0; i < checkCount; i++)
                {
                    direction[i] = -1;

                    sellOrBuyCheckPeriod[i] = baseTimePeriod;

                    baseTimePeriod += 5;

                    minuteBeforeTradeTime[i] = tradeDateTime.AddSeconds(0 - sellOrBuyCheckPeriod[i]);

                    debugMsg("minuteBeforeTradeTime[" + i + "]:" + minuteBeforeTradeTime[i]);
                }

                for (int i = 0; i < checkCount; i++)
                {

                    try
                    {
                        befofeRecord = RecordScanner.getRecordMinuteBeforeOrAfter(minuteBeforeTradeTime[i]);//XX鐘前的交易紀錄

                        if (befofeRecord == null)
                        {
                            return false;
                        }
                        basePrice[i] = befofeRecord.TradePrice;//交易基準

                        debugMsg("basePrice:" + i + ":" + basePrice[i]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("掃描失敗:" + e.Message + "---" + e.StackTrace + "---" + e.Source);
                    }

                }//end for

                for (int i = 0; i < checkCount; i++)
                {


                    if (record.TradePrice > basePrice[i])//目前交易金額大於XX分鐘前的交易金額
                    {
                        direction[i] = TradeType.BUY.GetHashCode();
                    }
                    else if (record.TradePrice < basePrice[i])//目前交易金額小於XX分鐘前的交易金額
                    {
                        direction[i] = TradeType.SELL.GetHashCode();
                    }

                    //if (i == 0)
                    //{
                    //    if (record.TradePrice > basePrice[i])//目前交易金額大於XX分鐘前的交易金額
                    //    {
                    //        direction[i] = TradeType.BUY.GetHashCode();
                    //    }
                    //    else if (record.TradePrice < basePrice[i])//目前交易金額小於XX分鐘前的交易金額
                    //    {
                    //        direction[i] = TradeType.SELL.GetHashCode();
                    //    }
                    //}
                    //else
                    //{
                    //    if (basePrice[i - 1] > basePrice[i])//目前交易金額大於XX分鐘前的交易金額
                    //    {
                    //        direction[i] = TradeType.BUY.GetHashCode();
                    //    }
                    //    else if (basePrice[i - 1] < basePrice[i])//目前交易金額小於XX分鐘前的交易金額
                    //    {
                    //        direction[i] = TradeType.SELL.GetHashCode();
                    //    }
                    //}

                }

                for (int i = checkCount - 1; i > 0; i--)
                {
                    if (direction[i] != direction[i - 1]) { return false; }
                }

                if (direction[0] == -1)//五個時間的價位都一樣
                {
                    return false;
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

        private double calOneProfit(double orderPrice)
        {
            if (nowTradeType == TradeType.BUY.GetHashCode())
            {

                oneProfit = record.TradePrice - orderPrice;

            }
            else if (nowTradeType == TradeType.SELL.GetHashCode())
            {

                oneProfit = orderPrice - record.TradePrice;
            }

            return oneProfit;
        }

        private int getReversePercentage(double offsetPoint)//取得轉折點反轉的百分比
        {
            foreach (KeyValuePair<int, int> item in stopRatio)
            {

                if (offsetPoint <= Convert.ToDouble(item.Key))
                {
                    return item.Value;
                }

            }
            return 0;
        }

        private double coreLogic()//獲利動態加碼
        {
            double stopPeriod = 0;//獲利反轉的間隔

            double stopPrice = 0;//獲利反轉的目標點位

            totalPureProfit = 0;

            lotIndex = Array_Begin_Index;

            //while (sourceFile.hasNext())
            for (int z = 0; z < recordList.Count; z++)
            {

                try
                {


                    //nowLine = sourceFile.getLine();

                    //record = OriginalRecordConverter.getOriginalRecord(nowLine);

                    //recordList.Add(record);

                    record = recordList[z];

                    if (isStartOrder == false)
                    {
                        maxTradePoint.TradePrice = 0;

                        minTradePoint.TradePrice = 99999;

                        if (isPrevLose == true ||
                            isPrevWin == true ||
                            Dice.run(Random_Seed))
                        {
                            tradeTime = record.TradeTime;

                            tradeDateTime = record.TradeMoment;

                            if (isPrevLose == true)
                            {
                                if (prevTradeType == TradeType.BUY.GetHashCode())
                                {
                                    if (reverseEnabled)
                                    {
                                        nowTradeType = prevTradeType = TradeType.SELL.GetHashCode();
                                    }
                                    else
                                    {
                                        nowTradeType = prevTradeType = TradeType.BUY.GetHashCode();
                                    }
                                }
                                else
                                {
                                    if (reverseEnabled)
                                    {
                                        nowTradeType = prevTradeType = TradeType.BUY.GetHashCode();
                                    }
                                    else
                                    {
                                        nowTradeType = prevTradeType = TradeType.SELL.GetHashCode();
                                    }

                                }
                            }
                            else
                                if (isPrevWin == true)
                                {
                                    if (prevTradeType == TradeType.BUY.GetHashCode())
                                    {
                                        if (reverseEnabled)
                                        {
                                            nowTradeType = prevTradeType = TradeType.SELL.GetHashCode();
                                        }
                                        else
                                        {
                                            nowTradeType = prevTradeType = TradeType.BUY.GetHashCode();
                                        }
                                    }
                                    else
                                    {
                                        if (reverseEnabled)
                                        {
                                            nowTradeType = prevTradeType = TradeType.BUY.GetHashCode();
                                        }
                                        else
                                        {
                                            nowTradeType = prevTradeType = TradeType.SELL.GetHashCode();
                                        }
                                    }
                                }
                                else
                                    if (!dealSellOrBuy(record, tradeDateTime))
                                    {
                                        continue;
                                    }


                            //secondAfterTradeToActiveCheck = tradeDateTime.AddSeconds(ActiveProfitStartPeriod);//5秒內利潤擴大50點

                            //minuteAfterStartActiveProfit = tradeDateTime.AddMinutes(1);//開始動態停利檢查，每一分鐘一次

                            orderPrice = record.TradePrice;

                            orderPriceList.Add(orderPrice);

                            orderPointList.Add(record);

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

                            isStartOrder = true;//下單啦

                            addList.Clear();
                        }

                    }


                    else if (isStartOrder == true)//已經開始下單
                    {
                        if (record.TradePrice < minTradePoint.TradePrice)
                        {
                            minTradePoint = record;
                        }

                        if (record.TradePrice > maxTradePoint.TradePrice)
                        {
                            maxTradePoint = record;
                        }





                        if (nowTradeType == TradeType.BUY.GetHashCode())
                        {

                            evenPrice = record.TradePrice;

                            oneProfit = 0;

                            for (int i = 0; i < orderPriceList.Count; i++)
                            {
                                oneProfit += evenPrice - orderPriceList[i];
                            }

                            pureProfit = oneProfit * valuePerPoint - orderPriceList.Count * cost;


                            if (
                                //(orderPriceList.Count == 2 && (maxTradePoint.TradePrice - orderPriceList[orderPriceList.Count - 1]) > 8 && pureProfit <= 100) ||
                                // (orderPriceList.Count == 3 && (maxTradePoint.TradePrice - orderPriceList[orderPriceList.Count - 1]) > 7 && pureProfit <= 100) ||
                                //(orderPriceList.Count >= 8) || 
                                //(orderPriceList.Count == 7 && record.TradePrice < maxTradePoint.TradePrice - 10) ||
                                // (orderPriceList.Count == 6 && record.TradePrice < maxTradePoint.TradePrice - 12) ||
                                //  (orderPriceList.Count == 5 && record.TradePrice < maxTradePoint.TradePrice - 14) ||
                                //   (orderPriceList.Count == 4 && record.TradePrice < maxTradePoint.TradePrice - 16) ||
                                //    (orderPriceList.Count == 3 && record.TradePrice < maxTradePoint.TradePrice - 18) ||
                                //     (orderPriceList.Count == 2 && record.TradePrice < maxTradePoint.TradePrice - 20) ||                                      
                                // (orderPriceList.Count > 3 && orderPriceList.Count < 6 && record.TradePrice <= orderPriceList[orderPriceList.Count - 2]) ||
                                // (orderPriceList.Count >= 2 && orderPriceList.Count <= 3 && record.TradePrice <= orderPriceList[0]) ||
                                //(orderPriceList.Count == 4 && record.TradePrice > orderPriceList[orderPriceList.Count-1] + 5) ||
                                //(orderPriceList.Count >= 3 && pureProfit <= 100) ||
                                //(orderPriceList.Count == 1 && (oneProfit < (0 - loseLine[1]) ) ) ||
                                //totalPureProfit + pureProfit < 

                                (orderPriceList.Count >= 2 && record.TradePrice <= maxTradePoint.TradePrice - winLine[1]) ||
                                (orderPriceList.Count == 1 && (oneProfit < (0 - loseLine[1])))
                                )//超過今日最大停損金額
                            {

                                if (oneProfit < 0 - loseLine[1])
                                {
                                    totalLoseProfit += pureProfit;
                                }

                                totalPureProfit += pureProfit;

                                if (orderPriceList.Count > maxLot) //最大交易口數
                                {
                                    maxLot = orderPriceList.Count;
                                }

                                totalProfit += oneProfit;

                                if (pureProfit > 0)
                                {
                                    continueWinMoney += pureProfit;

                                    continueLossMoney = 0;

                                    if (maxContinueWinMoney < continueWinMoney)
                                    {
                                        maxContinueWinMoney = continueWinMoney;

                                        maxContinueWinMoneyTime = record.TradeMoment;
                                    }
                                }
                                else
                                {
                                    continueWinMoney = 0;

                                    continueLossMoney += pureProfit;

                                    if (maxContinueLossMoney > continueLossMoney)
                                    {
                                        maxContinueLossMoney = continueLossMoney;

                                        maxContinueLossMoneyTime = record.TradeMoment;
                                    }
                                }

                                if (oneProfit > 0)
                                {
                                    winVolume += orderPriceList.Count;

                                    winCount++;
                                }
                                else
                                {
                                    loseVolume += orderPriceList.Count;
                                    if (orderPriceList.Count == 1)
                                    {
                                        loseCount++;
                                    }
                                }

                                debugMsg("認賠殺出");

                                debugMsg(" addTimes---->" + addList.Count);

                                debugMsg(" 平倉口數---->" + orderPriceList.Count);

                                debugMsg("平倉點數001---->" + evenPrice);

                                debugMsg("平倉時間---->" + record.TradeMoment);

                                for (int i = 0; i < orderPriceList.Count; i++)
                                {
                                    debugMsg("加碼價位:" + orderPriceList[i]);

                                    debugMsg("加碼時間:" + orderPointList[i].TradeMoment);

                                }


                                debugMsg("平倉前最低價:" + minTradePoint.TradePrice);

                                debugMsg("平倉前最低價的時間:" + minTradePoint.TradeMoment);

                                debugMsg("平倉前最高價:" + maxTradePoint.TradePrice);

                                debugMsg("平倉前最高價的時間:" + maxTradePoint.TradeMoment);

                                if (addList.Count >= 1)
                                {
                                    reverseEnabled = false;//獲利後，同方向

                                    continueLossMoney = 0;
                                }
                                else
                                {
                                    reverseEnabled = true;//一開始就賠錢，反方向


                                }

                                debugMsg("純利:" + pureProfit);

                                debugMsg("總純利:" + totalPureProfit);

                                //debugMsg("停損策略:" + loseLine[nowLoseLineIndex]);

                                if (addList.Count >= 1)
                                {
                                    debugMsg("停利策略:" + winLine[addList.Count]);

                                    debugMsg("反轉策略:" + reverseLine[addList.Count]);
                                }

                                debugMsg("停損次數:" + loseCount);

                                debugMsg("----------------------------------------------------------------------------------------------");

                                prevTradeType = TradeType.BUY.GetHashCode();

                                loseOut();


                            }
                            else if ((record.TradePrice - orderPrice) > winLine[orderPriceList.Count])
                            {

                                //賺了XX點，加碼

                                //--------------------------------------------------------------------------------------------------------------------------------
                                //  加碼部分
                                //--------------------------------------------------------------------------------------------------------------------------------


                                if (Convert.ToInt16(record.TradePrice - orderPriceList[orderPriceList.Count - 1]) > winLine[orderPriceList.Count])
                                {
                                    if (addList.Count == 0)//還沒加碼過
                                    {
                                        if (orderPriceList.Count < lotLimit)
                                        {

                                            addList.Add(1);//實際加碼的次數

                                            orderPriceList.Add(record.TradePrice);

                                            orderPointList.Add(record);

                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        if (orderPriceList.Count < lotLimit)
                                        {

                                            addList.Add(1);//實際加碼的次數

                                            orderPriceList.Add(record.TradePrice);

                                            orderPointList.Add(record);

                                            continue;
                                        }

                                    }
                                }

                            }
                        }
                        else if (nowTradeType == TradeType.SELL.GetHashCode())
                        {

                            evenPrice = record.TradePrice;

                            oneProfit = 0;

                            for (int i = 0; i < orderPriceList.Count; i++)
                            {
                                oneProfit += orderPriceList[i] - evenPrice;
                            }

                            pureProfit = oneProfit * valuePerPoint - orderPriceList.Count * cost;


                            if (
                                //(orderPriceList.Count == 2 && ((orderPriceList[orderPriceList.Count - 1] - minTradePoint.TradePrice) > 8) && pureProfit <= 100) ||
                                //(orderPriceList.Count == 3 && ((orderPriceList[orderPriceList.Count - 1] - minTradePoint.TradePrice) > 7) && pureProfit <= 100) ||
                                //(orderPriceList.Count >= 8 )|| 
                                //(orderPriceList.Count == 7 && record.TradePrice > (minTradePoint.TradePrice + 10)) ||
                                //(orderPriceList.Count == 6 && record.TradePrice > (minTradePoint.TradePrice + 12)) ||
                                //(orderPriceList.Count == 5 && record.TradePrice > (minTradePoint.TradePrice + 14)) ||
                                //(orderPriceList.Count == 4 && record.TradePrice > (minTradePoint.TradePrice + 16)) ||
                                //(orderPriceList.Count == 3 && record.TradePrice > (minTradePoint.TradePrice + 18)) ||
                                //(orderPriceList.Count == 2 && record.TradePrice > (minTradePoint.TradePrice + 20)) ||
                                //(orderPriceList.Count > 3 && orderPriceList.Count < 6 && record.TradePrice >= orderPriceList[orderPriceList.Count - 2]) ||
                                // (orderPriceList.Count >= 2 && orderPriceList.Count <= 3 && record.TradePrice >= orderPriceList[0]) ||

                               // (orderPriceList.Count == 4 && record.TradePrice < orderPriceList[orderPriceList.Count-1] - 5) ||

                               //(orderPriceList.Count >= 3 && pureProfit <= 100) ||
                                //(orderPriceList.Count == 1 && oneProfit < (0 - loseLine[1]))
                                // || totalPureProfit + pureProfit < maxProfitLoss

                                (orderPriceList.Count >= 2 && record.TradePrice >= minTradePoint.TradePrice + winLine[1]) ||
                                 (orderPriceList.Count == 1 && (oneProfit < (0 - loseLine[1])))
                                )
                            {

                                if (oneProfit < 0 - loseLine[1])
                                {
                                    totalLoseProfit += pureProfit;
                                }

                                totalPureProfit += pureProfit;

                                if (orderPriceList.Count > maxLot) //最大交易口數
                                {
                                    maxLot = orderPriceList.Count;
                                }

                                totalProfit += oneProfit;

                                if (pureProfit > 0)
                                {
                                    continueWinMoney += pureProfit;

                                    continueLossMoney = 0;

                                    if (maxContinueWinMoney < continueWinMoney)
                                    {
                                        maxContinueWinMoney = continueWinMoney;

                                        maxContinueWinMoneyTime = record.TradeMoment;
                                    }
                                }
                                else
                                {
                                    continueWinMoney = 0;

                                    continueLossMoney += pureProfit;

                                    if (maxContinueLossMoney > continueLossMoney)
                                    {
                                        maxContinueLossMoney = continueLossMoney;

                                        maxContinueLossMoneyTime = record.TradeMoment;
                                    }
                                }

                                if (oneProfit > 0)
                                {
                                    winVolume += orderPriceList.Count;

                                    winCount++;


                                }
                                else
                                {
                                    loseVolume += orderPriceList.Count;

                                    if (orderPriceList.Count == 1)
                                    {
                                        loseCount++;
                                    }


                                }


                                debugMsg("認賠殺出");

                                debugMsg(" addTimes---->" + addList.Count);

                                debugMsg(" 平倉口數---->" + orderPriceList.Count);

                                debugMsg("平倉點數002---->" + evenPrice);

                                debugMsg("平倉時間---->" + record.TradeMoment);

                                for (int i = 0; i < orderPriceList.Count; i++)
                                {
                                    debugMsg("加碼價位:" + orderPriceList[i]);

                                    debugMsg("加碼時間:" + orderPointList[i].TradeMoment);
                                }

                                debugMsg("平倉前最低價:" + minTradePoint.TradePrice);

                                debugMsg("平倉前最低價的時間:" + minTradePoint.TradeMoment);

                                debugMsg("平倉前最高價:" + maxTradePoint.TradePrice);

                                debugMsg("平倉前最高價的時間:" + maxTradePoint.TradeMoment);


                                if (addList.Count >= 1)
                                {
                                    reverseEnabled = false;//獲利後，同方向
                                }
                                else
                                {
                                    reverseEnabled = true;//一開始就賠錢，反方向                                    
                                }

                                debugMsg("純利:" + pureProfit);

                                debugMsg("總純利:" + totalPureProfit);

                                // debugMsg("停損策略:" + loseLine[nowLoseLineIndex]);

                                if (addList.Count >= 1)
                                {
                                    debugMsg("停利策略:" + winLine[addList.Count]);

                                    debugMsg("反轉策略:" + reverseLine[addList.Count]);
                                }
                                debugMsg("停損次數:" + loseCount);

                                debugMsg("----------------------------------------------------------------------------------------------");

                                prevTradeType = TradeType.SELL.GetHashCode();

                                loseOut();

                            }
                            else if ((orderPrice - record.TradePrice) > winLine[orderPriceList.Count])
                            {

                                //賺了XX點，加碼

                                //--------------------------------------------------------------------------------------------------------------------------------
                                //  加碼部分
                                //--------------------------------------------------------------------------------------------------------------------------------



                                if (Convert.ToInt16(orderPriceList[orderPriceList.Count - 1] - record.TradePrice) > winLine[orderPriceList.Count])
                                {

                                    if (addList.Count == 0)//還沒加碼過
                                    {
                                        if (orderPriceList.Count < lotLimit)
                                        {

                                            addList.Add(1);//實際加碼的次數

                                            orderPriceList.Add(record.TradePrice);

                                            orderPointList.Add(record);

                                            continue;
                                        }
                                    }

                                    else
                                    {
                                        if (orderPriceList.Count < lotLimit)
                                        {

                                            addList.Add(1);//實際加碼的次數

                                            orderPriceList.Add(record.TradePrice);

                                            orderPointList.Add(record);

                                            continue;
                                        }

                                    }
                                }

                            }

                        }



                    }//下單結束

                    if (totalPureProfit < maxProfitLoss)  //已達最大虧損水平線
                    {
                        return totalProfit;
                    }

                    //if (totalLoseProfit < maxProfitLoss)
                    //{
                    //    return totalProfit;
                    //}


                    //if (record.TradeHour >= 13 && record.TradeMinute >= 30)//交易時間截止
                    //{
                    //    if (isStartOrder == true)
                    //    {
                    //        oneProfit = 0;

                    //        evenPrice = record.TradePrice;

                    //        if (nowTradeType == TradeType.BUY.GetHashCode())
                    //        {
                    //            for (int i = 0; i < orderPriceList.Count; i++)
                    //            {
                    //                oneProfit += evenPrice - orderPriceList[i];
                    //            }
                    //        }
                    //        else if (nowTradeType == TradeType.SELL.GetHashCode())
                    //        {
                    //            for (int i = 0; i < orderPriceList.Count; i++)
                    //            {
                    //                oneProfit += orderPriceList[i] - evenPrice;
                    //            }

                    //        }

                    //        totalProfit += oneProfit;

                    //        isStartOrder = false;

                    //        tradeCount++;

                    //        if (oneProfit > 0)
                    //        {

                    //            winVolume += orderPriceList.Count;

                    //            winCount++;
                    //        }
                    //        else
                    //        {
                    //            loseVolume += orderPriceList.Count;

                    //            loseCount++;

                    //        }

                    //        if (nowTradeType == TradeType.BUY.GetHashCode())
                    //        {
                    //            debugMsg("時間到買入平倉");
                    //        }
                    //        else if (nowTradeType == TradeType.SELL.GetHashCode())
                    //        {
                    //            debugMsg("時間到賣出平倉");
                    //        }

                    //        debugMsg("平倉點數---->" + evenPrice);

                    //        debugMsg("平倉時間---->" + record.TradeMoment);

                    //        debugMsg(" addTimes---->" + addList.Count);

                    //        debugMsg(" 平倉口數---->" + orderPriceList.Count);

                    //        for (int i = 0; i < orderPriceList.Count; i++)
                    //        {
                    //            debugMsg("加碼價位:" + orderPriceList[i]);

                    //            debugMsg("加碼時間:" + orderPointList[i].TradeMoment);
                    //        }

                    //        debugMsg("平倉前最低價:" + minTradePoint.TradePrice);

                    //        debugMsg("平倉前最低價的時間:" + minTradePoint.TradeMoment);

                    //        debugMsg("平倉前最高價:" + maxTradePoint.TradePrice);

                    //        debugMsg("平倉前最高價的時間:" + maxTradePoint.TradeMoment);

                    //        pureProfit = oneProfit * valuePerPoint - (orderPriceList.Count) * cost;

                    //        totalPureProfit += pureProfit;

                    //        debugMsg("純利:" + pureProfit);

                    //        debugMsg("總純利:" + totalPureProfit);

                    //        //debugMsg("停損策略:" + loseLine[nowLoseLineIndex]);

                    //        if (addList.Count >= 1)
                    //        {
                    //            debugMsg("停利策略:" + winLine[addList.Count]);

                    //            debugMsg("反轉策略:" + reverseLine[addList.Count]);
                    //        }
                    //        debugMsg("停利次數:" + winCount);

                    //        debugMsg("----------------------------------------------------------------------------------------------");



                    //        return totalProfit;

                    //    } 

                    //    return totalProfit;
                    //}//end 交易時間截止

                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Source);
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);

                }



            }//end of while


            return totalProfit;


        }

        private void winOut()//獲利出場
        {
            lotIndex++;

            isPrevWin = true;

            isPrevLose = false;

            tradeOut();
        }

        private void loseOut()//認賠出場
        {
            lotIndex = Array_Begin_Index;

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

            addList.Clear();

            orderPriceList.Clear();

            orderPointList.Clear();

        }


        private double getReverseLitmit(int interval, int number, double ratio)//取得反轉點數
        //interval <--每次的範圍
        //number <--第幾次取得
        //第一次 
        {
            try
            {

                double litmit = interval;

                for (int i = 0; i < number; i++)
                {

                    litmit = litmit * ratio;

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
            if (debugEnabled)
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
