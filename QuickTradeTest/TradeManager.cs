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

        Boolean debugEnabled = false;//是否顯示除錯訊息

        Boolean reverseEnabled = false;//是否買賣方向轉向

        Boolean winOutEnabled = false;//是否要停利

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 變數。
        /// </summary>
        ///         

        double maxProfitLoss = -999999;//最大虧損水平線

        string coreMethod = "";//核心方法

        List<double> orderPointList = new List<double>();//動態停利機制中，每達到停利階段就下一次單時，下單的點數

        int activeOrderIndex = 0;//第幾次動態停利下單

        int lotIndex = 0;//交易口數陣列的第幾個

        int[] sellOrBuyCheckPeriod = null;//交易買賣方向的檢查時間間隔

        int number = 0;//超過檢查時間的次數

        double tempOneProfit = 99999;//單筆暫時獲利

        int minTradePoint = 99999;//市場最低價

        int maxTradePoint = 0;//市場最高價

        double offsetTradePoint;//最高與最低價格差異區間値 = maxTradePoint - minTradePoint

        int tradeCount = 0;  //交易次數        

        double oneProfit = 0;//單筆利潤

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

        Dictionary<int, int> reverseLine;  //動態停利反轉的底線

        int nowStrategyCount = 1; //目前使用哪一行的停損停利規則

        //int maxStrategyCount = 0; //停損停利規則最大有幾種

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

        int lastReversePoint = -1;//最後一個轉折點

        int addTimes = 0;//加碼次數

        List<int> orderPriceList = new List<int>();//下單價位的LIST

        int checkCount = 5;//檢查幾個時間間隔，來決定買或是賣

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

        public void setReverseLine(Dictionary<int, int> reverse)
        {
            reverseLine = reverse;
        }


        public TradeManager()
        {


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



        private void dealStrategyCount(int count)//依照獲利 或是 賠錢，或是加碼次數次數，來定停損停利範圍
        {
            if (count <= 0)
            {
                nowStrategyCount = 1;
                return;
            }

            //int baseCount = 1;//幾次之後跳下一階

            try
            {
                //if (count % baseCount == 0)
                {

                    //nowStrategyCount = (count / baseCount);

                    nowStrategyCount = count;

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

            if (this.coreMethod.Equals(Core_Method_1))
            {
                return coreLogic();
            }
            else if (this.coreMethod.Equals(Core_Method_2))
            {
                return coreLogic2();
            }
            else if (this.coreMethod.Equals(Core_Method_3))
            {
                return coreLogic3();
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

        private double coreLogic3()//逆勢動態停利
        {

            const int Trade_Period_Need = 10;//最高點、最低點都要離原始點至少要10點間隔才能新倉
            int stage = Stage_New;
            double originalPoint = 0;// 今日交易起始點
            double maxHighPoint = 0;//轉折點高點
            double maxLowPoint = 0;//轉折點低點

            double offsetPoint;//高低差
            double orderPriceTarget;//下單點位目標

            int reversePercentage;//停利或是停損，反轉的百分比

            double tradePeriod;//交易點位距離轉折點的距離

            int direction = 0;//波段的方向，例如由低向高之後反轉，或是由高向低之後反轉

            tradeCount = 0;

            while (sourceFile.hasNext())
            {

                try
                {

                    nowLine = sourceFile.getLine();

                    record = OriginalRecordConverter.getOriginalRecord(nowLine);

                    recordList.Add(record);

                    if (recordList.Count == 1)//第一筆資料進來
                    {
                        originalPoint = record.TradePrice;
                        maxHighPoint = record.TradePrice;
                        maxLowPoint = record.TradePrice;
                    }

                    if (record.TradePrice > maxHighPoint)//取得相對高點
                    {
                        maxHighPoint = record.TradePrice;

                        lastReversePoint = LastReversePoint.MAX.GetHashCode();
                    }
                    else if (record.TradePrice < maxLowPoint)//取得相對低點
                    {
                        maxLowPoint = record.TradePrice;

                        lastReversePoint = LastReversePoint.MIN.GetHashCode();
                    }

                    offsetPoint = maxHighPoint - maxLowPoint;

                    if (tradeCount == 0)
                    {

                        if ((maxHighPoint - originalPoint) > Trade_Period_Need)
                        {
                            //最高點大於原始點Trade_Period_Need這麼多點，可以開始交易
                        }
                        else if ((originalPoint - maxLowPoint) > Trade_Period_Need)
                        {
                            //原始點大於最低點Trade_Period_Need這麼多點，可以開始交易
                        }
                        else
                        {
                            continue;
                            //跳過，繼續計算
                        }
                    }

                    reversePercentage = getReversePercentage(offsetPoint);//查表，取得反轉百分比

                    tradePeriod = offsetPoint * reversePercentage / 100;

                    if (isStartOrder == false)//還沒新倉
                    {

                        {
                            if (tradeCount == 0)//第一次交易開始之前
                            {
                                if (lastReversePoint == LastReversePoint.MAX.GetHashCode())//低至高之後反轉，賣出
                                {
                                    direction = Direction_LowToHigh;
                                }
                                else if (lastReversePoint == LastReversePoint.MIN.GetHashCode())//高至低之後反轉，買入
                                {
                                    direction = Direction_HighToLow;
                                }
                            }

                            if (direction == Direction_HighToLow)
                            {
                                orderPriceTarget = maxLowPoint + tradePeriod;

                                if (record.TradePrice >= orderPriceTarget)
                                {
                                    debugMsg("MAX---->" + maxHighPoint);
                                    debugMsg("MIN---->" + maxLowPoint);
                                    debugMsg("Original---->" + originalPoint);

                                    stage = Stage_Order_New_Success;
                                    orderPrice = record.TradePrice;//實際交易點位
                                    isStartOrder = true;
                                    nowTradeType = TradeType.BUY.GetHashCode();
                                    maxHighPoint = orderPrice;//把上一個相對高點拿掉


                                    debugMsg("MAX---->" + maxHighPoint);
                                    debugMsg("MIN---->" + maxLowPoint);
                                    debugMsg("Original---->" + originalPoint);

                                    debugMsg("交易方式---->" + TradeType.BUY);

                                    debugMsg("交易金額---->" + orderPrice);

                                    debugMsg("交易時間---->" + record.TradeMoment);
                                }
                            }
                            else if (direction == Direction_LowToHigh)
                            {
                                orderPriceTarget = maxHighPoint - tradePeriod;
                                if (record.TradePrice <= orderPriceTarget)
                                {
                                    debugMsg("MAX---->" + maxHighPoint);
                                    debugMsg("MIN---->" + maxLowPoint);
                                    debugMsg("Original---->" + originalPoint);

                                    stage = Stage_Order_New_Success;
                                    orderPrice = record.TradePrice;//實際交易點位
                                    isStartOrder = true;
                                    nowTradeType = TradeType.SELL.GetHashCode();
                                    maxLowPoint = orderPrice;//把上一個相對低點拿掉

                                    debugMsg("MAX---->" + maxHighPoint);
                                    debugMsg("MIN---->" + maxLowPoint);
                                    debugMsg("Original---->" + originalPoint);

                                    debugMsg("交易方式---->" + TradeType.SELL);

                                    debugMsg("交易金額---->" + orderPrice);

                                    debugMsg("交易時間---->" + record.TradeMoment);
                                }
                            }

                        }
                    }
                    else if (isStartOrder == true)//已經新倉，準備平倉
                    {
                        if (record.TradeHour >= 13 && record.TradeMinute >= 44)//交易時間截止
                        {
                            evenPrice = record.TradePrice;

                            oneProfit = calOneProfit(orderPrice);

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

                            debugMsg("純利:" + (oneProfit * valuePerPoint - cost));

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
                        }
                        else
                        {

                            if (direction == Direction_HighToLow)
                            {
                                orderPriceTarget = maxHighPoint - tradePeriod;
                                if (record.TradePrice <= orderPriceTarget)
                                {
                                    stage = Stage_Order_Even_Success;
                                    oneProfit = calOneProfit(orderPrice);
                                    totalProfit += oneProfit;
                                    isStartOrder = false;
                                    tradeCount++;

                                    debugMsg("---------------------------------------------------------------------------->");

                                    debugMsg("MAX---->" + maxHighPoint);
                                    debugMsg("MIN---->" + maxLowPoint);
                                    debugMsg("Original---->" + originalPoint);


                                    if (oneProfit > 0)
                                    {
                                        winCount++;
                                        debugMsg("買入停利出場");
                                        debugMsg("停利次數:" + winCount);

                                        maxLowPoint = record.TradePrice;//把上一個相對低點拿掉

                                        debugMsg("MAX---->" + maxHighPoint);
                                        debugMsg("MIN---->" + maxLowPoint);
                                    }
                                    else
                                    {
                                        loseCount++;
                                        debugMsg("買入認賠殺出");
                                        debugMsg("停損次數:" + loseCount);

                                        maxHighPoint = orderPrice;//把上一個相對高點拿掉

                                        debugMsg("MAX---->" + maxHighPoint);
                                        debugMsg("MIN---->" + maxLowPoint);
                                    }


                                    debugMsg("orderPriceTarget---->" + orderPriceTarget);


                                    debugMsg("平倉點數---->" + record.TradePrice);

                                    debugMsg("平倉時間---->" + record.TradeMoment);

                                    pureProfit = oneProfit * valuePerPoint - Convert.ToInt32(lotArray[lotIndex]) * cost;

                                    debugMsg("純利:" + pureProfit);

                                    totalPureProfit += pureProfit;

                                    debugMsg("總純利:" + totalPureProfit);



                                    debugMsg("----------------------------------------------------------------------------------------------");
                                    continue;
                                }
                            }
                            else if (direction == Direction_LowToHigh)
                            {

                                orderPriceTarget = maxLowPoint + tradePeriod;

                                if (record.TradePrice >= orderPriceTarget)
                                {


                                    stage = Stage_Order_Even_Success;
                                    oneProfit = calOneProfit(orderPrice);
                                    totalProfit += oneProfit;
                                    isStartOrder = false;
                                    tradeCount++;

                                    debugMsg("---------------------------------------------------------------------------->");

                                    debugMsg("MAX---->" + maxHighPoint);
                                    debugMsg("MIN---->" + maxLowPoint);
                                    debugMsg("Original---->" + originalPoint);

                                    if (oneProfit > 0)
                                    {
                                        winCount++;
                                        debugMsg("賣出停利出場");
                                        debugMsg("停利次數:" + winCount);

                                        maxHighPoint = record.TradePrice;//把上一個相對高點拿掉

                                        debugMsg("MAX---->" + maxHighPoint);
                                        debugMsg("MIN---->" + maxLowPoint);
                                    }
                                    else
                                    {
                                        loseCount++;
                                        debugMsg("賣出認賠殺出");
                                        debugMsg("停損次數:" + loseCount);

                                        maxLowPoint = orderPrice;//把上一個相對低點拿掉

                                        debugMsg("MAX---->" + maxHighPoint);
                                        debugMsg("MIN---->" + maxLowPoint);
                                    }

                                    debugMsg("orderPriceTarget---->" + orderPriceTarget);

                                    debugMsg("平倉點數---->" + record.TradePrice);

                                    debugMsg("平倉時間---->" + record.TradeMoment);

                                    pureProfit = oneProfit * valuePerPoint - Convert.ToInt32(lotArray[lotIndex]) * cost;

                                    debugMsg("純利:" + pureProfit);

                                    totalPureProfit += pureProfit;

                                    debugMsg("總純利:" + totalPureProfit);




                                    debugMsg("----------------------------------------------------------------------------------------------");
                                    continue;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
            return totalProfit;
        }

        private double coreLogic2()//動態停利
        {
            while (sourceFile.hasNext())
            {

                try
                {

                    nowLine = sourceFile.getLine();

                    record = OriginalRecordConverter.getOriginalRecord(nowLine);


                    recordList.Add(record);
                    //
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


                        //secondAfterTradeToActiveCheck = tradeDateTime.AddSeconds(ActiveProfitStartPeriod);//5秒內利潤擴大50點

                        //minuteAfterStartActiveProfit = tradeDateTime.AddMinutes(1);//開始動態停利檢查，每一分鐘一次

                        orderPrice = record.TradePrice;

                        //orderPointList.Add(orderPrice);

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

                        calOneProfit(this.orderPrice);

                        if (record.TradeHour >= 13 && record.TradeMinute >= 44)//交易時間截止
                        {

                            if (isActiveCheckProfit == true)
                            {
                                double activeProfit = 0;

                                activeProfit += oneProfit;

                                foreach (double point in orderPointList)
                                {

                                    activeProfit += calOneProfit(point);

                                }

                                totalProfit += activeProfit;

                                activeOrderIndex = 0;

                                winCount++;

                                winCount += orderPointList.Count;

                                winOut();//獲利出場 

                                debugMsg("本次動態利潤:" + activeProfit * 50);

                                debugMsg("本次下單口數:" + (orderPointList.Count + 1));

                                debugMsg("本次動態純利潤:" + (activeProfit * 50 - (orderPointList.Count + 1) * 66));


                            }
                            else
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

                                debugMsg("純利:" + (oneProfit * 50 - 66));

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
                            }

                            return totalProfit;

                        }//end 交易時間截止

                        else if (nowTradeType == TradeType.BUY.GetHashCode() && (orderPrice - record.TradePrice) > loseLine[nowStrategyCount])
                        {//賠了XX點，認賠殺出

                            evenPrice = record.TradePrice;

                            oneProfit = evenPrice - orderPrice;

                            //oneProfit *= Convert.ToInt32(lotArray[lotIndex]);

                            lotIndex = Array_Begin_Index;

                            totalProfit += oneProfit;

                            debugMsg("認賠殺出");

                            debugMsg("平倉點數001---->" + evenPrice);

                            debugMsg("平倉時間---->" + record.TradeMoment);

                            debugMsg("純利:" + (oneProfit * 50 - 66));

                            debugMsg("停損策略:" + loseLine[nowStrategyCount]);

                            debugMsg("停利策略:" + winLine[nowStrategyCount]);

                            debugMsg("----------------------------------------------------------------------------------------------");

                            loseCount++;

                            loseOut();

                            prevTradeType = TradeType.BUY.GetHashCode();

                        }
                        else if (nowTradeType == TradeType.SELL.GetHashCode() && (record.TradePrice - orderPrice) > loseLine[nowStrategyCount])
                        {
                            //賠了XX點，認賠殺出


                            evenPrice = record.TradePrice;

                            oneProfit = orderPrice - evenPrice;

                            //oneProfit *= Convert.ToInt32(lotArray[lotIndex]);

                            lotIndex = Array_Begin_Index;

                            totalProfit += oneProfit;

                            debugMsg("認賠殺出");

                            debugMsg("平倉點數002---->" + evenPrice);

                            debugMsg("平倉時間---->" + record.TradeMoment);

                            debugMsg("純利:" + (oneProfit * 50 - 66));

                            debugMsg("停損策略:" + loseLine[nowStrategyCount]);

                            debugMsg("停利策略:" + winLine[nowStrategyCount]);

                            debugMsg("----------------------------------------------------------------------------------------------");

                            loseCount++;

                            loseOut();

                            prevTradeType = TradeType.SELL.GetHashCode();

                        }

                        else if ((isActiveCheckProfit == false) && (oneProfit >= winLine[1]))//還沒開始【動態停利】
                        {
                            //if (record.TradeMoment > secondAfterTradeToActiveCheck)//檢查時間到了，看看是否要啟動動態停利機制
                            {
                                //if (oneProfit - tempOneProfit > ActiveProfitPoint)

                                isActiveCheckProfit = true;//開始動態停利

                                debugMsg("開始動態停利---->" + record.TradeMoment + " ---------->Profit:" + oneProfit + "-----------tempOneProfit:" + tempOneProfit);

                                tempOneProfit = oneProfit;

                                isStartOrder = true;//下單啦

                                continue;

                            }


                        }// end 檢查是否開始動態停利

                        else if ((isActiveCheckProfit == true))//開始動態停利
                        {
                            //if ((record.TradeMoment > minuteAfterStartActiveProfit))//到了規定的檢查時間
                            {
                                //double reverseLitmit = getReverseLitmit(ActiveProfitPoint, number);//動態停利的停利條件

                                double reverseLitmit = getReverseLitmit(winLine[1], activeOrderIndex, ratio);//動態停利的停利條件

                                if (oneProfit < tempOneProfit - reverseLitmit)//這短時間內增加的獲利少於規定的點數，預測趨勢將反轉，獲利了結
                                {

                                    double activeProfit = 0;

                                    activeProfit += oneProfit;

                                    foreach (double point in orderPointList)
                                    {

                                        activeProfit += calOneProfit(point);

                                    }

                                    totalProfit += activeProfit;

                                    activeOrderIndex = 0;

                                    evenPrice = record.TradePrice;

                                    debugMsg("******平倉點數009---->" + evenPrice);

                                    debugMsg("******平倉時間---->" + record.TradeMoment);

                                    debugMsg("本次動態利潤:" + activeProfit * 50);

                                    debugMsg("本次下單口數:" + (orderPointList.Count + 1));

                                    debugMsg("本次動態純利潤:" + (activeProfit * 50 - (orderPointList.Count + 1) * 66));

                                    debugMsg("動態停利範圍:" + reverseLitmit);

                                    debugMsg("----------------------------------------------------------------------------------------------");

                                    winCount++;

                                    winCount += orderPointList.Count;

                                    winOut();//獲利出場 

                                    orderPointList = new List<double>();

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

                                    if (oneProfit > 0 && Convert.ToInt32(oneProfit) / winLine[1] >= 1)//獲利大於停利點1倍以上
                                    {
                                        if (activeOrderIndex < Convert.ToInt32(oneProfit) / winLine[1])
                                        {
                                            activeOrderIndex = Convert.ToInt32(oneProfit) / winLine[1];//第幾階段下單

                                            orderPointList.Add(record.TradePrice);//下單點數

                                        }
                                    }

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

                                    //debugMsg("RUN!!" + minuteAfterStartActiveProfit);

                                    //debugMsg("----------------------------------------------------------------------------------------------");

                                    //minuteAfterStartActiveProfit = minuteAfterStartActiveProfit.AddMinutes(ActiveProfitCheckPeriod);//繼續跑XX秒

                                    continue;
                                }
                            }//開始動態停利，但是還不到檢查的時間
                            //else
                            //{

                            //    continue;

                            //}

                        }//end 執行動態停利檢查



                    }//下單結束

                    if (totalProfit * 50 < maxProfitLoss)  //已達最大虧損水平線
                    {
                        return totalProfit;
                    }





                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Source);
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);

                }

                continue;

            }//end of while


            return totalProfit;


        }

        private double coreLogic()//獲利加碼
        {
            double stopPeriod = 0;//獲利反轉的間隔

            int stopPrice = 0;//獲利反轉的目標點位

            totalPureProfit = 0;

            while (sourceFile.hasNext())
            {

                try
                {

                    nowLine = sourceFile.getLine();

                    record = OriginalRecordConverter.getOriginalRecord(nowLine);

                    recordList.Add(record);

                    if (isStartOrder == false)
                    {
                        maxTradePoint = 0;

                        minTradePoint = 99999;

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
                                else if (!dealSellOrBuy(record, tradeDateTime))
                                {
                                    continue;
                                }


                            //secondAfterTradeToActiveCheck = tradeDateTime.AddSeconds(ActiveProfitStartPeriod);//5秒內利潤擴大50點

                            //minuteAfterStartActiveProfit = tradeDateTime.AddMinutes(1);//開始動態停利檢查，每一分鐘一次

                            orderPrice = record.TradePrice;

                            orderPriceList.Clear();

                            orderPriceList.Add(orderPrice);

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

                            //dealStrategyCount(winCount);//取得停損停利範圍
                            //dealStrategyCount(totalProfit);//取得停損停利範圍     

                            isStartOrder = true;//下單啦
                        }

                    }


                    else if (isStartOrder == true)//已經開始下單
                    {

                        dealStrategyCount(addTimes);//取得停損停利範圍

                        if (minTradePoint > record.TradePrice)
                        {
                            minTradePoint = record.TradePrice;
                        }
                        else if (maxTradePoint < record.TradePrice)
                        {
                            maxTradePoint = record.TradePrice;
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

                            if ((pureProfit + totalPureProfit < maxProfitLoss) || (addTimes >= 1 && orderPrice == record.TradePrice) || (orderPrice - record.TradePrice) > loseLine[nowStrategyCount])
                            {//賠了XX點，認賠殺出

                                totalPureProfit += pureProfit;

                                totalProfit += oneProfit;

                                loseCount += addTimes + 1;

                                debugMsg("認賠殺出");

                                debugMsg("addTimes---->" + addTimes);

                                debugMsg("平倉點數001---->" + evenPrice);

                                debugMsg("平倉時間---->" + record.TradeMoment);

                                
                                if (addTimes >= 1)
                                {
                                    reverseEnabled = true;
                                }
                                else
                                {
                                    reverseEnabled = true;
                                }

                                addTimes = 0;

                                orderPriceList.Clear();

                                debugMsg("純利:" + pureProfit);                                

                                debugMsg("總純利:" + totalPureProfit);

                                debugMsg("停損策略:" + loseLine[nowStrategyCount]);

                                debugMsg("停利策略:" + winLine[nowStrategyCount]);

                                debugMsg("停損次數:" + loseCount);

                                debugMsg("----------------------------------------------------------------------------------------------");

                                prevTradeType = TradeType.BUY.GetHashCode();

                                loseOut();

                                if (totalPureProfit < maxProfitLoss)
                                {
                                    return totalProfit;
                                }


                            }
                            else if ((record.TradePrice - orderPrice) > winLine[nowStrategyCount])
                            {
                                //賺了XX點，加碼

                                int nowAddTimes = Convert.ToInt16((record.TradePrice - orderPrice) / winLine[nowStrategyCount]);//目前應該有的加碼次數



                                //--------------------------------------------------------------------------------------------------------------------------------
                                //  加碼部分
                                //--------------------------------------------------------------------------------------------------------------------------------


                                if (nowAddTimes > addTimes)
                                {
                                    if (addTimes < 10)
                                    {

                                        addTimes = nowAddTimes;//實際要加碼的次數

                                        orderPriceList.Add(record.TradePrice);

                                        continue;
                                    }


                                }


                                //--------------------------------------------------------------------------------------------------------------------------------
                                //  停利部分
                                //--------------------------------------------------------------------------------------------------------------------------------
                                //stopPeriod = getReverseLitmit(maxTradePoint - minTradePoint, addTimes, ratio);

                                stopPeriod = reverseLine[nowStrategyCount];

                                stopPrice = maxTradePoint - Convert.ToInt32(stopPeriod);

                                if (minTradePoint < orderPriceList[addTimes] && record.TradePrice == orderPriceList[addTimes] && addTimes >5)
                                {
                                    winOutEnabled = true;
                                }
                                else
                                {
                                    winOutEnabled = false;
                                }

                                if (winOutEnabled == true || record.TradePrice < stopPrice)
                                {

                                    if (addTimes > maxLot)
                                    {
                                        maxLot = addTimes;
                                    }                                   

                                    evenPrice = record.TradePrice;

                                    oneProfit = 0;

                                    for (int i = 0; i < orderPriceList.Count; i++)
                                    {
                                        oneProfit += evenPrice - orderPriceList[i];
                                    }

                                    totalProfit += oneProfit;

                                    winCount += addTimes + 1;

                                    debugMsg("停利出場");

                                    debugMsg("addTimes---->" + addTimes);

                                    debugMsg("平倉點數003---->" + evenPrice);

                                    debugMsg("平倉時間---->" + record.TradeMoment);

                                    pureProfit = oneProfit * valuePerPoint - addTimes * cost;

                                    if (addTimes >= 1)
                                    {
                                        reverseEnabled = true;
                                    }
                                    else
                                    {
                                        reverseEnabled = false;
                                    }

                                    orderPriceList.Clear();

                                    debugMsg("純利:" + pureProfit);

                                    totalPureProfit += pureProfit;

                                    debugMsg("總純利:" + totalPureProfit);

                                    debugMsg("停損策略:" + loseLine[nowStrategyCount]);

                                    debugMsg("停利策略:" + winLine[nowStrategyCount]);

                                    debugMsg("停利次數:" + winCount);

                                    debugMsg("----------------------------------------------------------------------------------------------");

                                    prevTradeType = TradeType.BUY.GetHashCode();

                                    if (addTimes > 1 && totalPureProfit > 1000)
                                    {
                                        return totalProfit;
                                    }

                                    winOut();

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

                            if ((pureProfit + totalPureProfit < maxProfitLoss) || (addTimes >= 1 && orderPrice == record.TradePrice) || (record.TradePrice - orderPrice) > loseLine[nowStrategyCount])
                            {
                                //賠了XX點，認賠殺出

                                totalPureProfit += pureProfit;
                               
                                totalProfit += oneProfit;

                                loseCount += addTimes + 1;

                                debugMsg("認賠殺出");

                                debugMsg("addTimes---->" + addTimes);

                                debugMsg("平倉點數002---->" + evenPrice);

                                debugMsg("平倉時間---->" + record.TradeMoment);                                

                                if (addTimes >= 1)
                                {
                                    reverseEnabled = true;
                                }
                                else
                                {
                                    reverseEnabled = true;
                                }

                                addTimes = 0;

                                orderPriceList.Clear();

                                debugMsg("純利:" + pureProfit);                                

                                debugMsg("總純利:" + totalPureProfit);

                                debugMsg("停損策略:" + loseLine[nowStrategyCount]);

                                debugMsg("停利策略:" + winLine[nowStrategyCount]);

                                debugMsg("停損次數:" + loseCount);

                                debugMsg("----------------------------------------------------------------------------------------------");

                                prevTradeType = TradeType.SELL.GetHashCode();

                                loseOut();

                                if (totalPureProfit < maxProfitLoss)
                                {
                                    return totalProfit;
                                }

                            }
                            else if ((orderPrice - record.TradePrice) > winLine[nowStrategyCount])
                            {
                                //賺了XX點，加碼

                                int nowAddTimes = (orderPrice - record.TradePrice) / winLine[nowStrategyCount];//目前應該有的加碼次數

                                //--------------------------------------------------------------------------------------------------------------------------------
                                //  加碼部分
                                //--------------------------------------------------------------------------------------------------------------------------------



                                if (nowAddTimes > addTimes)
                                {
                                    if (addTimes < 10)
                                    {
                                        addTimes = nowAddTimes;//實際要加碼的次數

                                        orderPriceList.Add(record.TradePrice);

                                        continue;
                                    }

                                }


                                //--------------------------------------------------------------------------------------------------------------------------------
                                //  停利部分
                                //--------------------------------------------------------------------------------------------------------------------------------
                                //stopPeriod = getReverseLitmit(maxTradePoint - minTradePoint, addTimes, ratio);

                                stopPeriod = reverseLine[nowStrategyCount];

                                stopPrice = minTradePoint + Convert.ToInt32(stopPeriod);

                                //if (stopPrice > orderPrice - winLine[nowStrategyCount])
                                //{
                                //    stopPrice = orderPrice - winLine[nowStrategyCount];
                                //}

                                if (minTradePoint < orderPriceList[addTimes] && record.TradePrice == orderPriceList[addTimes]  && addTimes>5)
                                {
                                    winOutEnabled = true;
                                }
                                else
                                {
                                    winOutEnabled = false;
                                }

                                if (winOutEnabled == true || record.TradePrice > stopPrice)
                                {

                                    if (addTimes > maxLot)
                                    {
                                        maxLot = addTimes;
                                    }

                                    evenPrice = record.TradePrice;

                                    oneProfit = 0;

                                    for (int i = 0; i < orderPriceList.Count; i++)
                                    {
                                        oneProfit += orderPriceList[i] - evenPrice;
                                    }

                                    totalProfit += oneProfit;


                                    winCount += addTimes + 1;

                                    debugMsg("停利出場");

                                    debugMsg("addTimes---->" + addTimes);

                                    debugMsg("平倉點數004---->" + evenPrice);

                                    debugMsg("平倉時間---->" + record.TradeMoment);

                                    pureProfit = oneProfit * valuePerPoint - addTimes * cost;

                                    if (addTimes >= 1)
                                    {
                                        reverseEnabled = true;
                                    }
                                    else
                                    {
                                        reverseEnabled = false;
                                    }



                                    orderPriceList.Clear();

                                    debugMsg("純利:" + pureProfit);

                                    totalPureProfit += pureProfit;

                                    debugMsg("總純利:" + totalPureProfit);

                                    debugMsg("停損策略:" + loseLine[nowStrategyCount]);

                                    debugMsg("停利策略:" + winLine[nowStrategyCount]);

                                    debugMsg("停利次數:" + winCount);

                                    debugMsg("停損次數:" + loseCount);

                                    debugMsg("----------------------------------------------------------------------------------------------");



                                    prevTradeType = TradeType.SELL.GetHashCode();

                                    if (addTimes > 1 && totalPureProfit > 1000)
                                    {
                                        return totalProfit;
                                    }


                                    winOut();


                                }

                            }

                        }



                        //if (totalPureProfit < maxProfitLoss / 2)  //已達最大虧損水平線的一半
                        //{
                        //    if ((isPrevLose == true) && (isStartOrder == false))
                        //    {
                        //        reverseEnabled = true;
                        //    }
                        //}


                    }//下單結束

                    if (totalPureProfit < maxProfitLoss)  //已達最大虧損水平線
                    {

                        return totalProfit;
                    }

                    if (record.TradeHour >= 13 && record.TradeMinute >= 30)//交易時間截止
                    {
                        if (isStartOrder == true)
                        {
                            oneProfit = 0;

                            evenPrice = record.TradePrice;

                            if (nowTradeType == TradeType.BUY.GetHashCode())
                            {

                                for (int i = 0; i < orderPriceList.Count; i++)
                                {
                                    oneProfit += evenPrice - orderPriceList[i];
                                }

                            }
                            else if (nowTradeType == TradeType.SELL.GetHashCode())
                            {
                                for (int i = 0; i < orderPriceList.Count; i++)
                                {
                                    oneProfit += orderPriceList[i] - evenPrice;
                                }
                            }

                            totalProfit += oneProfit;

                            isStartOrder = false;

                            tradeCount++;

                            if (oneProfit > 0)
                            {
                                winCount += addTimes + 1;
                            }
                            else
                            {
                                loseCount += addTimes + 1;
                            }

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

                            debugMsg("addTimes---->" + addTimes);


                            double pureProfit = oneProfit * valuePerPoint - addTimes * cost;

                            debugMsg("純利:" + pureProfit);

                            debugMsg("停損策略:" + loseLine[nowStrategyCount]);

                            debugMsg("停利策略:" + winLine[nowStrategyCount]);

                            debugMsg("停利次數:" + winCount);

                            debugMsg("----------------------------------------------------------------------------------------------");



                            return totalProfit;

                        }//end 交易時間截止

                        return totalProfit;
                    }

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


            isPrevWin = true;

            isPrevLose = false;

            tradeOut();
        }

        private void loseOut()//認賠出場
        {


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

            addTimes = 0;
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
