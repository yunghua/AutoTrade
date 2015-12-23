using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradeUtility;
using YuantaOrdLib;

namespace AutoTrade
{
    class TradeMaster
    {

        //Const 常數區
        //-------------------------------------------------------------------------------------------------------------
        //        
        const double cost = 68;//手續費成本

        const double valuePerPoint = 50;//每點價值，小台50元/點，大台200元/點        

        public const int Stage_Last_Day = 0;//承接上一個交易日
        public const int Stage_New = 1;//初始化
        public const int Stage_Order_Login_Start = 2;//登入下單API開始
        public const int Stage_Order_Login_Success = 3;//登入下單API成功
        public const int Stage_Order_New_Start = 4;//新倉開始下單
        public const int Stage_Order_New_Success = 5;//新倉回報下單成功        
        public const int Stage_Order_New_Fail = 6;//新倉回報下單失敗  
        public const int Stage_Order_Not_Process = 7;//無法下單，可能是尚未登入完成
        public const int Stage_Order_Even_Start = 8;//平倉倉開始下單
        public const int Stage_Order_Even_Success = 9;//平倉回報下單成功
        public const int Stage_Order_Even_Partial = 10;//平倉回報部分下單成功，部分失敗             
        public const int Stage_Order_Even_Fail = 11;//平倉回報下單失敗  
        public const int Stage_Order_Fail = 12;//回報下單失敗  
        public const int Stage_Order_Time_Up = 13;//時間到停止下單
        public const int Stage_End = 14;//結束

        public const int Out_Not_Out = 0;//尚未平倉出場
        public const int Out_Active_Profit = 1;//動態停利出場
        public const int Out_Win_Sell = 2;//賣出停利
        public const int Out_Win_Buy = 3;//買入停利
        public const int Out_Loss_Sell = 4;//賣出停損
        public const int Out_Loss_Buy = 5;//買入停損
        public const int Out_Time_Up = 6;//時間到出場

        const int Array_Begin_Index = 0;//獲利加碼的口數陣列初始值

        const String Output_Dir = "Output";

        const String Strategy_File_Name = "Strategy.txt";//策略檔案名

        const Boolean DEBUG = true;

        const Boolean END_TRADE = true;

        const Boolean NEXT_TRADE = false;

        const int Random_Seed = 888;//隨機參數種子

        const int SELL = 2;//交易方式:賣

        const int BUY = 1;//交易方式:買

        const int MaxTradeCount = 100;//最大交易次數        

        const double MaxProfitLoss = -5000;//最大虧損水平線

        //public enum TradeType : int { SELL, BUY };

        const int ActiveProfitStartPeriod = 5;//檢查動態停利啟動條件的時間基準，5秒前

        const int ActiveProfitCheckPeriod = 1;//動態停利的檢查時間間隔,一分鐘後

        const int ActiveProfitPoint = 30;//動態停利的啟動條件

        const string Track_Dir = "Track";//往日交易檔目錄        

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Boolean變數。
        /// </summary>      
        /// 


        Boolean isWinReverse = false;//獲利後，買賣方向是否反轉

        public Boolean IsWinReverse
        {
            get { return isWinReverse; }
            set { isWinReverse = value; }
        }

        Boolean isLoseReverse = false;//賠錢後，買賣方向是否反轉

        public Boolean IsLoseReverse
        {
            get { return isLoseReverse; }
            set { isLoseReverse = value; }
        }

        Boolean hasWriteMaxAndMinPrice = false;

        Boolean isStartOrder = false;//是否開始下單

        private Boolean isStartOrderLastDay = false;

        public Boolean IsStartOrderLastDay
        {
            get { return isStartOrderLastDay; }
            set { isStartOrderLastDay = value; }
        }

        public Boolean IsStartOrder
        {
            get { return isStartOrder; }
            set { isStartOrder = value; }
        }

        Boolean isPrevWin = false;

        Boolean isPrevLose = false;

        Boolean isStopTodayTrade = false; //是否停止今日交易

        Boolean enableTrade = false;//是否可以交易

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 變數。
        /// </summary>
        ///                         

        int prevStopPrice = 0;//上一個反轉點位

        public int PrevStopPrice
        {
            get { return prevStopPrice; }
            set { prevStopPrice = value; }
        }

        int stopPrice = 0;//反轉點位

        public int StopPrice
        {
            get { return stopPrice; }
            set { stopPrice = value; }
        }

        int continueLoseTimes = 0;//連續反轉的次數

        public int ContinueLoseTimes
        {
            get { return continueLoseTimes; }
            set { continueLoseTimes = value; }
        }

        int minTradePoint = 99999;//市場最低價

        int maxTradePoint = 0;//市場最高價            

        double oneProfitPoint = 0;//單筆利潤

        double totalProfitPoint = 0;//總利潤

        double oneNetProfit = 0;//單筆純利

        double totalNetProfit = 0;//總純利

        string tradeTime = "";//交易時間點

        int orderPrice = 0;//下單交易價格

        public int OrderPrice
        {
            get { return orderPrice; }
            set { orderPrice = value; }
        }

        double evenPrice = 0;//平倉交易價格             

        int winCount = 0;//獲利次數

        int loseCount = 0;//賠錢次數        

        string nowTradeType = "";//交易方式賣或是買

        public string NowTradeType
        {
            get { return nowTradeType; }
            set { nowTradeType = value; }
        }

        string prevTradeType = "";//上一次交易方式賣或是買                

        string tradeCode = "";//商品代碼

        string outputLine = "";//寫入RPT檔案的內容

        string tradeRecordFileName = "";//自動交易紀錄

        string fileName = "";

        int maxLoss;//單日最大停損

        string futuresCode = "";//期貨代碼，大台指TX，小台指MTX                

        string nowDay = "";//今天日期

        public string NowDay
        {
            get { return nowDay; }
            set { nowDay = value; }
        }

        string nowMonth = "";//今天月份

        string trackFileName = "";  ////跨天數交易軌跡檔檔名

        //-------------------------------------------------------------------------------------------------------------
        //          物件區
        //-------------------------------------------------------------------------------------------------------------    

        Form1 parentForm;//UI介面

        TradeFile allTradeOutputFile;          //當天所有交易紀錄
        StrategyFile strategyFile;                        //策略檔
        TradeFile tradeRecordFile;                //自動交易紀錄檔
        TradeFile trackFile;                            //跨天數交易軌跡檔

        OriginalRecord befofeRecord;

        OriginalRecord record = new OriginalRecord();//檔案的一行紀錄     

        DateTime now = System.DateTime.Now;

        public DateTime Now
        {
            get { return now; }
            set { now = value; }
        }

        DateTime[] minuteBeforeTradeTime;//交易前X秒，判斷買或賣       

        List<OriginalRecord> recordList = null;//所有交易紀錄

        DateTime tradeDateTime;//交易時間點

        Dictionary<int, int> loseLine;  //認賠的底線
        Dictionary<int, int> winLine;  //停利的底線
        Dictionary<int, double> reverseLine;  //動態停利反轉的底線

        //DateTime stopTradeTime;//今日交易結束時間

        //-------------------------------------------------------------------------------------------------------------        
        //        下單區
        //-------------------------------------------------------------------------------------------------------------

        private YuantaOrdLib.YuantaOrdClass yuantaOrderAPI;//元大下單API

        Boolean isOrderAPIReady = false;//Order API 登入成功與否

        const string Function_Code_01 = "01";//委託
        const string Function_Code_02 = "02";//減量
        const string Function_Code_03 = "03";//刪單
        const string Function_Code_04 = "04";//改價

        const string Type_Code_0 = "0";//期貨
        const string Type_Code_1 = "1";//選擇權

        const string Sub_Account = "";//子帳號

        const string Order_No = "";//委託書號

        public const string BS_Type_B = "B";//買
        public const string BS_Type_S = "S";//賣

        const string Lots = "1";//交易數量

        const string Trade_Type_NEW = "0";             //新倉
        const string Trade_Type_EVEN = "1";            //平倉
        const string Trade_Type_ONEDAY = "2";     //當沖
        const string Trade_Type_AUTO = "";            //自動

        const string Price_Type_M = "M";//市價
        const string Price_Type_L = "L";//限價

        const string Order_Cond_R = "R";//ROD
        const string Order_Cond_F = "F";//FOK
        const string Order_Cond_I = "I";//IOC

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


        int dealQty; //已成交量                     

        string ipAPI = "api.yuantafutures.com.tw";//下單伺服器的網址

        string branchCode = "";//分公司

        string account = "";//帳號

        string id = "";

        string password = "";

        string lots = "";//交易筆數

        string[] lotArray;//獲利加碼的設定

        int lotIndex = 0;//交易口數陣列的第幾個

        int stage = Stage_New;//下單階段

        public int Stage
        {
            get { return stage; }
            set { stage = value; }
        }

        string orderDircetion;//下單方向，買或賣

        public string OrderDircetion
        {
            get { return orderDircetion; }
            set { orderDircetion = value; }
        }

        int orderNewPrice;//新倉成交均價

        public int OrderNewPrice
        {
            get { return orderNewPrice; }
            set { orderNewPrice = value; }
        }
        int orderEvenPrice;//平倉成交均價

        public int OrderEvenPrice
        {
            get { return orderEvenPrice; }
            set { orderEvenPrice = value; }
        }

        string orderEvenTime;//平倉時間

        int outStyle = Out_Not_Out;//平倉出場方式，有動態停利、賣出/買入停利、賣出/買入停損等        

        List<int> orderNewPriceList = new List<int>();//下單價位的LIST

        public List<int> OrderNewPriceList
        {
            get { return orderNewPriceList; }
            set { orderNewPriceList = value; }
        }

        int lotLimit = 3;//最大加碼口數        

        //List<int> addList = new List<int>();//加碼的LIST

        //public List<int> AddList
        //{
        //    get { return addList; }
        //    set { addList = value; }
        //}

        int[] sellOrBuyCheckPeriod = null;//交易買賣方向的檢查時間間隔

        int checkCount = 5;//檢查幾個時間間隔，來決定買或是賣

        string initialDirection = "";//初始買賣方向

        public string InitialDirection
        {
            get { return initialDirection; }
            set { initialDirection = value; }
        }

        //-------------------------------------------------------------------------------------------------------------
        // 程式區
        //-------------------------------------------------------------------------------------------------------------

        public TradeMaster()
        {
        }

        public TradeMaster(Form1 parentForm)
        {
            this.parentForm = parentForm;
        }

        public void setLotLimit(int limit)
        {
            if (limit == 0)
            {
                return;
            }
            lotLimit = limit;
        }

        public void setEnableTrade(Boolean enable)
        {
            this.enableTrade = enable;
        }

        public void setFuturesCode(string code)
        {
            this.futuresCode = code;
        }

        public void setTradeCode(string code)
        {
            this.tradeCode = code;
        }

        public void setIsOrderAPIReady(Boolean ready)
        {
            this.isOrderAPIReady = ready;
        }

        public void setOrderAPI(YuantaOrdClass api)
        {
            this.yuantaOrderAPI = api;
        }

        public YuantaOrdClass getOrderAPI()
        {
            return yuantaOrderAPI;
        }

        public void setIpAPI(string ipAPI)
        {
            this.ipAPI = ipAPI;
        }

        public string getIpAPI()
        {
            return ipAPI;
        }

        public void setBranchCode(string branchCode)
        {
            this.branchCode = branchCode;
        }

        public string getBranchCode()
        {
            return branchCode;
        }

        public void setAccount(string account)
        {
            this.account = account;
        }

        public string getAccount()
        {
            return account;
        }

        public void setPassword(string password)
        {
            this.password = password;
        }

        public string getPassword()
        {
            return password;
        }

        public void setID(string id)
        {
            this.id = id;
        }

        public string getID()
        {
            return id;
        }

        public void setMaxLoss(int maxLoss)
        {
            this.maxLoss = maxLoss;
        }

        public int getMaxLoss()
        {
            return maxLoss;
        }

        public void setLots(string lots)
        {
            this.lots = lots;
        }

        public string getLots()
        {
            return lots;
        }

        public Dictionary<int, int> getWinLine()
        {
            return winLine;
        }

        public Dictionary<int, int> getLoseLine()
        {
            return loseLine;
        }

        public Dictionary<int, double> getReverseLine()
        {
            return reverseLine;
        }


        private void showParameters()
        {
            debugMsg("Max_Loss:" + maxLoss);
            debugMsg("Lots:" + lots);
            //debugMsg("Branch_Code:" + branchCode);
            //debugMsg("Account_Code:" + account);
            //debugMsg("ID:" + id);
            //debugMsg("Password:" + password);

        }

        public void prepareDataFromLastTradeDay()//承接上一個交易日
        {
            if (stage == Stage_Last_Day)
            {

                maxTradePoint = maxTradePointLastDay;

                minTradePoint = minTradePointLastDay;

                isStartOrder = isStartOrderLastDay;

                parentForm.textBox_OrderStart.Text = Convert.ToString(isStartOrder);

                parentForm.textBox_B_S.Text = orderDircetion;

                parentForm.textBox_MaxPrice.Text = Convert.ToString(maxTradePoint);

                parentForm.textBox_MinPrice.Text = Convert.ToString(minTradePoint);

                parentForm.textBox_OrderPrice.Text = Convert.ToString(orderPrice);

                parentForm.textBox_Stage.Text = Convert.ToString(stage);

                parentForm.textBox_NowTradeType.Text = Convert.ToString(nowTradeType);

                parentForm.textBox_StopPrice.Text = Convert.ToString(stopPrice);

                parentForm.textBox_PrevStopPrice.Text = Convert.ToString(prevStopPrice);

                parentForm.textBox_ContinueLoseTimes.Text = Convert.ToString(continueLoseTimes);

            }
        }

        public void prepareTrackFile()
        {
            string appDir = System.Windows.Forms.Application.StartupPath;

            trackFileName = appDir + "\\" + Track_Dir + "\\" + "Track_" + now.Year + "_" + now.Month + "_" + nowDay + ".txt";

            trackFile = new TradeFile(trackFileName);

            trackFile.prepareWriter();

            trackMsg("TradeCode = " + tradeCode);

        }

        public void prepareFirst()
        {
            if (now.Day <= 9)
            {
                nowDay = Convert.ToString("0" + now.Day);
            }
            else
            {
                nowDay = Convert.ToString(now.Day);
            }

            if (now.Month <= 9)
            {
                nowMonth = Convert.ToString("0" + now.Month);
            }
            else
            {
                nowMonth = Convert.ToString(now.Month);
            }


            lotArray = lots.Split(',');

            string appDir = System.Windows.Forms.Application.StartupPath;

            loseLine = new Dictionary<int, int>();

            winLine = new Dictionary<int, int>();

            reverseLine = new Dictionary<int, double>();

            strategyFile = StrategyFile.getInstance();

            strategyFile.dealStrategyRule(appDir);

            this.winLine = strategyFile.getWinLine();

            this.loseLine = strategyFile.getLoseLine();

            this.reverseLine = strategyFile.getReverseLine();

            isPrevWin = false;//上一次交易是否獲利

            //stopTradeTime = new DateTime(now.Year, now.Month, now.Day, 13, 44, 0);


            fileName = appDir + "\\" + Output_Dir + "\\" + "NEW_Daily_" + now.Year + "_" + now.Month + "_" + nowDay + ".rpt";

            allTradeOutputFile = new TradeFile(fileName);

            allTradeOutputFile.prepareWriter();

            tradeRecordFileName = appDir + "\\" + Output_Dir + "\\" + "TradeRecord_" + now.Year + "_" + now.Month + "_" + nowDay + ".rpt";

            tradeRecordFile = new TradeFile(tradeRecordFileName);

            tradeRecordFile.prepareWriter();

            recordList = new List<OriginalRecord>();

            befofeRecord = new OriginalRecord();

            RecordScanner.setRecordList(recordList);

            debugMsg(now + "------<<開始交易>>");

            showParameters();

            prepareOrderAPI();



        }

        public void stop()
        {
            try
            {
                if (trackFile != null)
                {
                    trackFile.close();
                }
                if (allTradeOutputFile != null)
                {
                    allTradeOutputFile.close();
                }
                if (tradeRecordFile != null)
                {
                    tradeRecordFile.close();
                }
                if (strategyFile != null)
                {
                    strategyFile.close();
                }
            }
            catch (Exception)
            {
            }
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

                outputLine = Convert.ToString(now.Year) + nowMonth + nowDay + "," + futuresCode + "     ," + TradeUtility.TradeUtility.getInstance().dealYearMonth() + "     ," + matchTime.Substring(0, 2) + matchTime.Substring(3, 2) + matchTime.Substring(6, 2) + "," + matchPrice + "," + matchQuantity + ",-,-,";

                allTradeOutputFile.writeLine(outputLine);


                if (!isStopTodayTrade && enableTrade)
                {
                    startTrade(matchTime, matchPrice, matchQuantity);//執行交易程式
                }

            }
            catch (IOException e)
            {
                throw e;
            }
        }


        private void prepareOrderAPI()
        {
            try
            {
                yuantaOrderAPI.OnOrdMatF += new _DYuantaOrdEvents_OnOrdMatFEventHandler(yuantaOrderAPI_OnOrdMatF);
                yuantaOrderAPI.OnOrdRptF += new _DYuantaOrdEvents_OnOrdRptFEventHandler(yuantaOrderAPI_OnOrdRptF);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // 即時成交回報
        void yuantaOrderAPI_OnOrdMatF(string Omkt, string Buys, string Cmbf, string Bhno, string AcNo, string Suba, string Symb, string Scnam, string O_Kind, string S_Buys, string O_Prc, string A_Prc, string O_Qty, string Deal_Qty, string T_Date, string D_Time, string Order_No, string O_Src, string O_Lin, string Oseq_No)
        {
            string mat_str = String.Format("Omkt={0},Buys={1},Cmbf={2},Bhno={3},Acno={4},Suba={5},Symb={6},Scnam={7},O_Kind={8},S_Buys={9},O_Prc={10},A_Prc={11},O_Qty={12},Deal_Qty={13},T_Date={14},D_Time={15},Order_No={16},O_Src={17},O_Lin={18},Oseq_No={19}"
                                    , Omkt.Trim(), Buys.Trim(), Cmbf.Trim(), Bhno.Trim(), AcNo.Trim(), Suba.Trim(), Symb.Trim(), Scnam.Trim(), O_Kind.Trim(), S_Buys.Trim(), O_Prc.Trim(), A_Prc.Trim(), O_Qty.Trim(), Deal_Qty.Trim(), T_Date.Trim(), D_Time.Trim(), Order_No.Trim(), O_Src.Trim(), O_Lin.Trim(), Oseq_No.Trim());

            debugMsg("即時成交回報" + mat_str);

            int dealPrice;

            dealPrice = Convert.ToInt32(A_Prc.Trim());//成交均價            

            dealQty = Convert.ToInt32(Deal_Qty.Trim());//已成交量

            if (stage == Stage_Order_New_Start)//新倉
            {
                stage = Stage_Order_New_Success;

                orderNewPrice = dealPrice;

                orderNewPriceList.Add(orderNewPrice);

                parentForm.textBox_OrderNewPriceList.Text = "";

                for (int i = 0; i < orderNewPriceList.Count; i++)
                {
                    parentForm.textBox_OrderNewPriceList.Text += orderNewPriceList[i] + Environment.NewLine;
                }

                debugMsg("orderNewPrice:" + orderNewPrice);

                trackMsg("OrderPrice = " + orderNewPrice);

                trackMsg("");

                prevStopPrice = stopPrice;

                debugMsg("PrevStopPrice:" + prevStopPrice);

                trackMsg("PrevStopPrice = " + prevStopPrice);

                trackMsg("");

                stopPrice = dealPrice;

                parentForm.textBox_PrevStopPrice.Text = Convert.ToString(prevStopPrice);

                debugMsg("StopPrice = " + stopPrice);

                trackMsg("StopPrice = " + stopPrice);

                trackMsg("");

                parentForm.textBox_ContinueLoseTimes.Text = Convert.ToString(continueLoseTimes);

                debugMsg("ContinueLoseTimes = " + continueLoseTimes);

                trackMsg("ContinueLoseTimes = " + continueLoseTimes);

                trackMsg("");

                parentForm.textBox_StopPrice.Text = Convert.ToString(stopPrice);

                trackMsg("NewTrade");

                trackMsg("");

                lotIndex++;

                if (lotIndex >= lotArray.Length)
                {
                    lotIndex = lotArray.Length - 1;
                }

                isStartOrder = true;//下單啦    

            }
            else if (stage == Stage_Order_Even_Start)//平倉
            {
                stage = Stage_Order_Even_Success;

                orderEvenPrice = dealPrice;

                debugMsg("orderEvenPrice:" + orderEvenPrice);

                trackMsg("OrderPrice = " + orderEvenPrice);

                trackMsg("");

                trackMsg("MaxPrice = " + maxTradePoint);

                trackMsg("");

                trackMsg("MinPrice = " + minTradePoint);

                trackMsg("");

                trackMsg("EndTrade");

                trackMsg("");

                orderEvenTime = D_Time.Trim();//成交時間

                isStartOrder = false;//此次交易結束，準備下一次交易                 

                dealOut();//處理交易結束後，寫紀錄檔，以及相關參數

                if ((outStyle == Out_Loss_Buy) || (outStyle == Out_Loss_Sell))//只有一筆留倉，賠錢
                {
                    prevStopPrice = stopPrice = 0;
                }

                if ((outStyle == Out_Win_Buy) || (outStyle == Out_Win_Sell))//兩筆以上留倉，趨勢反轉
                {
                    prevStopPrice = stopPrice;

                    stopPrice = dealPrice;
                }

                if (orderNewPriceList != null && orderNewPriceList.Count == lotLimit)
                {
                    isWinReverse = true;
                }
            }

        }

        // 即時委託回報
        void yuantaOrderAPI_OnOrdRptF(string Omkt, string Mktt, string Cmbf, string Statusc, string Ts_Code, string Ts_Msg, string Bhno, string AcNo, string Suba, string Symb, string Scnam, string O_Kind, string O_Type, string Buys, string S_Buys, string O_Prc, string O_Qty, string Work_Qty, string Kill_Qty, string Deal_Qty, string Order_No, string T_Date, string O_Date, string O_Time, string O_Src, string O_Lin, string A_Prc, string Oseq_No, string Err_Code, string Err_Msg, string R_Time, string D_Flag)
        {
            string rpt_str = String.Format("Omkt={0},Mktt={1},Cmbf={2},Statusc={3},Ts_Code={4},Ts_Msg={5},Bhno={6},Acno={7},Suba={8},Symb={9},Scnam={10},O_Kind={11},O_Type={12},Buys={13},S_Buys={14},O_Prc={15},O_Qty={16},Work_Qty={17},Kill_Qty={18},Deal_Qty={19},Order_No={20},T_Date={21},O_Date={22},O_Time={23},O_Src={24},O_Lin={25},A_Prc={26},Oseq_No={27},Err_Code={28},Err_Msg={29},R_Time={30},D_Flag={31}"
                                    , Omkt.Trim(), Mktt.Trim(), Cmbf.Trim(), Statusc.Trim(), Ts_Code.Trim(), Ts_Msg.Trim(), Bhno.Trim(), AcNo.Trim(), Suba.Trim(), Symb.Trim(), Scnam.Trim(), O_Kind.Trim(), O_Type.Trim(), Buys.Trim(), S_Buys.Trim(), O_Prc.Trim(), O_Qty.Trim(), Work_Qty.Trim(), Kill_Qty.Trim(), Deal_Qty.Trim(), Order_No.Trim(), T_Date.Trim(), O_Date.Trim(), O_Time.Trim(), O_Src.Trim(), O_Lin.Trim(), A_Prc.Trim(), Oseq_No.Trim(), Err_Code.Trim(), Err_Msg.Trim(), R_Time.Trim(), D_Flag.Trim());

            debugMsg("即時委託回報" + rpt_str);

            if (Ts_Code.Trim().Equals("05") || Ts_Code.Trim().Equals("07") || Ts_Code.Trim().Equals("11"))
            {
                if (stage == Stage_Order_New_Start)
                {
                    stage = Stage_Order_New_Fail;

                    debugMsg("stage = Stage_Order_New_Fail");
                }
                else if (stage == Stage_Order_Even_Start)
                {
                    stage = Stage_Order_Even_Fail;

                    debugMsg("stage = Stage_Order_Even_Fail");

                    if (record.TradeHour >= 13 && record.TradeMinute >= 30)
                    {
                        stage = Stage_End;

                        debugMsg("stage = Stage_End");
                    }
                }
                else
                {
                    stage = Stage_Order_Fail;

                    debugMsg("stage = Stage_Order_Fail");

                    if (record.TradeHour >= 13 && record.TradeMinute >= 30)
                    {
                        stage = Stage_End;

                        debugMsg("stage = Stage_End");
                    }

                }

            }

        }

        private int dealOrderNew(string tradeCode, string orderPrice, string orderLot, string orderDirection)//用來下單建立新倉，回傳值:stage
        {
            try
            {
                orderPrice = "";//市價下單

                dealOrder(tradeCode, orderPrice, orderLot, orderDirection, Trade_Type_NEW);

                trackMsg("BuyOrSell = " + orderDirection);

                trackMsg("");
            }
            catch (Exception e)
            {
                throw e;
            }

            debugMsg("dealOrderNew-->stage:" + Stage_Order_New_Start);

            return Stage_Order_New_Start;
        }

        private int dealOrderEven(string tradeCode, string orderPrice, string orderLot, string orderDirection)//用來下單平倉，回傳值:stage
        {

            try
            {
                orderPrice = "";//市價下單

                dealOrder(tradeCode, orderPrice, orderLot, orderDirection, Trade_Type_EVEN);

                trackMsg("BuyOrSell = " + orderDirection);

                trackMsg("");
            }
            catch (Exception e)
            {

                throw e;
            }

            debugMsg("dealOrderEven-->stage:" + Stage_Order_Even_Start);

            return Stage_Order_Even_Start;
        }


        //tradeCode 期貨商品代碼，如MXFJ5
        //orderPrice   價位
        //orderLot 口數
        //orderDirection 買或賣
        //tradeType 新倉或是平倉        
        private void dealOrder(string tradeCode, string orderPrice, string orderLot, string orderDirection, string tradeType)//用來下單，回傳值:stage
        {

            if (!isOrderAPIReady)
            {
                stage = Stage_Order_Not_Process;

                throw new Exception("下單API尚未登入成功!");
            }

            yuantaOrderAPI.SetWaitOrdResult(1);//等待委託單回報
            //yuantaOrderAPI.SetWaitOrdResult(0);//不等待委託單回報

            string ret_no = string.Empty;

            ret_no = yuantaOrderAPI.SendOrderF(Function_Code_01, Type_Code_0, branchCode, account, Sub_Account, Order_No, orderDirection, tradeCode, orderPrice, orderLot, tradeType, Price_Type_M, Order_Cond_I, "", "");

            string orderResult = "下單結果:" + ret_no + "'\n";

            debugMsg(orderResult);


        }

        public void startTrade(String matchTime, String matchPrice, String matchQuantity)//回傳値表示是否結束本日交易
        {

            try
            {
                record = new OriginalRecord();

                record.TradeTime = matchTime;

                record.TradeMoment = Convert.ToDateTime(matchTime);

                record.TradeHour = record.TradeMoment.Hour;

                record.TradeMinute = record.TradeMoment.Minute;

                record.TradePrice = Convert.ToInt32(matchPrice);

                record.TradeVolumn = Convert.ToInt32(matchQuantity);

                recordList.Add(record);

            }
            catch (Exception e)
            {
                debugMsg(e.StackTrace);
            }

            if (isOrderAPIReady)
            {
                coreLogic();
            }

        }

        int maxTradePointLastDay = 0;//上一個交易日，最後一次交易的最高點

        public int MaxTradePointLastDay
        {
            get { return maxTradePointLastDay; }
            set { maxTradePointLastDay = value; }
        }

        int minTradePointLastDay = 0;

        public int MinTradePointLastDay
        {
            get { return minTradePointLastDay; }
            set { minTradePointLastDay = value; }
        }

        private void coreLogic()
        {

            if (stage == Stage_Order_New_Fail)//下單失敗
            {
                isStartOrder = false;
            }

            parentForm.textBox_OrderStart.Text = Convert.ToString(isStartOrder);

            parentForm.textBox_B_S.Text = orderDircetion;

            parentForm.textBox_MaxPrice.Text = Convert.ToString(maxTradePoint);

            parentForm.textBox_MinPrice.Text = Convert.ToString(minTradePoint);

            parentForm.textBox_OrderPriceNew.Text = Convert.ToString(orderNewPrice);

            parentForm.textBox_Stage.Text = Convert.ToString(stage);

            parentForm.textBox_OrderStart.Text = Convert.ToString(isStartOrder);

            parentForm.textBox_NowTradeType.Text = Convert.ToString(nowTradeType);

            parentForm.textBox_TradePrice.Text = Convert.ToString(record.TradePrice);

            if (record.TradeMoment.Hour >= 13 && record.TradeMoment.Minute >= 44 && record.TradeMoment.Second >= 59 && hasWriteMaxAndMinPrice == false)
            {
                trackMsg("MaxPrice = " + maxTradePoint);

                trackMsg("");

                trackMsg("MinPrice = " + minTradePoint);

                trackMsg("");

                hasWriteMaxAndMinPrice = true;
            }

            if (isStartOrder == false && stage != Stage_Order_New_Start && stage != Stage_Order_Time_Up && stage != Stage_End && (isPrevLose == true || isPrevWin == true || Dice.run(Random_Seed))) //下單版本
            //if (isStartOrder == false && (isPrevLose == true || isPrevWin == true || Dice.run(Random_Seed))) //測試版本
            {

                maxTradePoint = 0;

                minTradePoint = 99999;

                tradeTime = record.TradeTime;

                tradeDateTime = record.TradeMoment;

                if (isPrevLose == true)
                {
                    if (prevTradeType == BS_Type_B)
                    {
                        if (isLoseReverse)
                        {
                            nowTradeType = prevTradeType = BS_Type_S;
                        }
                        else
                        {
                            nowTradeType = prevTradeType = BS_Type_B;
                        }
                    }
                    else
                    {
                        if (isLoseReverse)
                        {
                            nowTradeType = prevTradeType = BS_Type_B;
                        }
                        else
                        {
                            nowTradeType = prevTradeType = BS_Type_S;
                        }
                    }
                }
                else if (isPrevWin == true)
                {
                    if (prevTradeType == BS_Type_B)
                    {
                        if (isWinReverse)
                        {
                            nowTradeType = prevTradeType = BS_Type_S;
                        }
                        else
                        {
                            nowTradeType = prevTradeType = BS_Type_B;
                        }
                    }
                    else
                    {
                        if (isWinReverse)
                        {
                            nowTradeType = prevTradeType = BS_Type_B;
                        }
                        else
                        {
                            nowTradeType = prevTradeType = BS_Type_S;
                        }
                    }
                }
                else  //還沒有交易過
                {
                    if (initialDirection.Trim().Equals(BS_Type_B))
                    {
                        nowTradeType = BS_Type_B;
                    }
                    else if (initialDirection.Trim().Equals(BS_Type_S))
                    {
                        nowTradeType = BS_Type_S;
                    }
                    else if (!dealSellOrBuy(record, tradeDateTime))
                    {
                        return;//五個時間的的方向不同，下次繼續
                    }
                }


                orderPrice = record.TradePrice;

                if (nowTradeType == BS_Type_B)
                {
                    debugMsg("交易方式---->" + BS_Type_B);

                    orderDircetion = BS_Type_B;
                }
                else if (nowTradeType == BS_Type_S)
                {
                    debugMsg("交易方式---->" + BS_Type_S);

                    orderDircetion = BS_Type_S;
                }

                debugMsg("第一筆交易金額---->" + orderPrice);

                debugMsg("交易時間---->" + tradeDateTime);

                try
                {
                    stage = this.dealOrderNew(tradeCode, Convert.ToString(orderPrice), lotArray[lotIndex], orderDircetion);
                }
                catch (Exception e)
                {
                    debugMsg(e.StackTrace);
                    debugMsg(e.Message);
                }

            }


            if (isStartOrder == true && (stage == Stage_Last_Day || stage == Stage_Order_New_Success || stage == Stage_Order_Even_Success || stage == Stage_Order_Even_Fail))//已經開始下單，而且下單成功，或是平倉失敗            
            {
                if (minTradePoint > record.TradePrice)//取得新倉後市場最低價
                {
                    minTradePoint = record.TradePrice;
                }
                else if (maxTradePoint < record.TradePrice)//取得新倉後市場最高價
                {
                    maxTradePoint = record.TradePrice;
                }

                if (nowTradeType == BS_Type_B)
                {


                    if (
                               (orderNewPriceList.Count > 1 && record.TradePrice <= stopPrice - reverseLine[orderNewPriceList.Count]) ||
                               (orderNewPriceList.Count == 1 && record.TradePrice <= stopPrice - loseLine[orderNewPriceList.Count])
                               )
                    {//反轉

                        stopPrice = record.TradePrice;

                        if (stopPrice > prevStopPrice)//反轉後繼續向上加碼
                        {
                            continueLoseTimes = 0;
                        }

                        prevStopPrice = stopPrice;

                        orderDircetion = BS_Type_S;

                        evenPrice = record.TradePrice;

                        if (stage == Stage_Order_New_Success || stage == Stage_Last_Day)
                        {

                            int orderLots = 0;

                            if (continueLoseTimes >= 2)//全部平倉
                            {
                                orderLots = orderNewPriceList.Count;

                                continueLoseTimes = 0;
                            }
                            else
                            {
                                orderLots = Convert.ToInt32(lotArray[lotIndex]);

                                continueLoseTimes++;
                            }

                            stage = this.dealOrderEven(tradeCode, Convert.ToString(evenPrice), Convert.ToString(orderLots), orderDircetion);
                        }

                        if (orderNewPriceList.Count == 1 && record.TradePrice <= stopPrice - loseLine[orderNewPriceList.Count])
                        {
                            debugMsg("outStyle = Out_Loss_Buy");

                            outStyle = Out_Loss_Buy;
                        }
                        else if (orderNewPriceList.Count > 1 && record.TradePrice <= stopPrice - reverseLine[orderNewPriceList.Count])
                        {
                            debugMsg("outStyle = Out_Win_Buy");

                            outStyle = Out_Win_Buy;
                        }
                    }
                    else if ((record.TradePrice - orderNewPriceList[orderNewPriceList.Count - 1]) >= winLine[orderNewPriceList.Count])  //加碼
                    {

                        //--------------------------------------------------------------------------------------------------------------------------------
                        //  加碼部分
                        //--------------------------------------------------------------------------------------------------------------------------------

                        //賺了XX點，加碼

                        if (orderNewPriceList.Count < lotLimit)
                        {

                            orderDircetion = BS_Type_B;//繼續買

                            if (stage == Stage_Order_New_Success || stage == Stage_Last_Day)
                            {
                                stage = this.dealOrderNew(tradeCode, Convert.ToString(record.TradePrice), lotArray[lotIndex], orderDircetion);
                            }

                            stopPrice = record.TradePrice;

                            return;
                        }

                    }

                }
                else if (nowTradeType == BS_Type_S)
                {

                    //if (orderNewPriceList.Count > 1)
                    //{
                    //    stopPeriod = reverseLine[orderNewPriceList.Count - 1];

                    //    stopPrice = orderNewPriceList[orderNewPriceList.Count - 1] + Convert.ToInt16(stopPeriod);
                    //}

                    if (
                                  (orderNewPriceList.Count > 1 && record.TradePrice >= stopPrice + reverseLine[orderNewPriceList.Count]) ||
                                  (orderNewPriceList.Count == 1 && record.TradePrice >= stopPrice + loseLine[orderNewPriceList.Count])
                     )
                    //反轉
                    {

                        stopPrice = record.TradePrice;

                        if (stopPrice < prevStopPrice)//反轉後繼續向下加碼
                        {
                            continueLoseTimes = 0;
                        }

                        prevStopPrice = stopPrice;

                        orderDircetion = BS_Type_B;

                        evenPrice = record.TradePrice;

                        if (stage == Stage_Order_New_Success || stage == Stage_Last_Day)
                        {

                            int orderLots = 0;

                            if (continueLoseTimes >= 2)//全部平倉
                            {
                                orderLots = orderNewPriceList.Count;

                                continueLoseTimes = 0;
                            }
                            else
                            {
                                orderLots = Convert.ToInt32(lotArray[lotIndex]);

                                continueLoseTimes++;
                            }

                            stage = this.dealOrderEven(tradeCode, Convert.ToString(evenPrice), Convert.ToString(orderLots), orderDircetion);
                        }

                        if (orderNewPriceList.Count == 1 && record.TradePrice >= stopPrice + loseLine[orderNewPriceList.Count])
                        {
                            debugMsg("outStyle = Out_Loss_Sell");

                            outStyle = Out_Loss_Sell;
                        }
                        else if (orderNewPriceList.Count > 1 && record.TradePrice >= stopPrice + reverseLine[orderNewPriceList.Count])
                        {
                            debugMsg("outStyle = Out_Win_Sell");

                            outStyle = Out_Win_Sell;
                        }
                    }
                    else if ((record.TradePrice - orderNewPriceList[orderNewPriceList.Count - 1]) >= winLine[orderNewPriceList.Count])
                    {

                        //賺了XX點，加碼

                        //--------------------------------------------------------------------------------------------------------------------------------
                        //  加碼部分
                        //--------------------------------------------------------------------------------------------------------------------------------


                        if (orderNewPriceList.Count < lotLimit)
                        {

                            orderDircetion = BS_Type_S;//繼續賣

                            if (stage == Stage_Order_New_Success || stage == Stage_Last_Day)
                            {
                                stage = this.dealOrderNew(tradeCode, Convert.ToString(record.TradePrice), lotArray[lotIndex], orderDircetion);
                            }

                            stopPrice = record.TradePrice;

                            return;
                        }

                    }
                }

            }//下單結束

            if (maxLoss < 0 && totalNetProfit < maxLoss)  //已達最大虧損水平線
            {
                endTodayTrade();
            }


        }

        private void endTodayTrade()
        {
            loseOut();

            isStopTodayTrade = true;

            stage = Stage_End;
        }

        private void dealOut()
        {
            try
            {
                parentForm.textBox_OrderNewPriceList.Text = "";

                if (outStyle == Out_Win_Sell)
                {
                    dealOutWinSell();
                }
                else if (outStyle == Out_Win_Buy)
                {
                    dealOutWinBuy();
                }
                else if (outStyle == Out_Loss_Sell)
                {
                    dealOutLossSell();
                }
                else if (outStyle == Out_Loss_Buy)
                {
                    dealOutLossBuy();
                }
                else if (outStyle == Out_Time_Up)
                {
                    dealOutTimeUp();
                }
            }
            catch (Exception e)
            {
                debugMsg(e.StackTrace);
            }
        }

        private void calOneProfit()
        {

            try
            {
                if (nowTradeType == BS_Type_B)
                {
                    //oneProfitPoint = orderEvenPrice - orderNewPrice;

                    for (int i = 0; i < orderNewPriceList.Count; i++)
                    {
                        oneProfitPoint += orderEvenPrice - orderNewPriceList[i];
                    }

                }
                else if (nowTradeType == BS_Type_S)
                {
                    //oneProfitPoint = orderNewPrice - orderEvenPrice;

                    for (int i = 0; i < orderNewPriceList.Count; i++)
                    {
                        oneProfitPoint += orderNewPriceList[i] - orderEvenPrice;
                    }
                }

                oneProfitPoint *= Convert.ToInt32(lotArray[lotIndex]);
            }
            catch (Exception e)
            {
                debugMsg(e.StackTrace);
            }
        }

        private void dealOutTimeUp()//時間到出場
        {
            debugMsg("時間到，平倉。");

            dealOutCore();

        }


        private void dealOutCore()//處理平倉後記錄獲利動作的核心
        {
            calOneProfit();

            totalProfitPoint += oneProfitPoint;

            oneNetProfit = oneProfitPoint * valuePerPoint - Convert.ToInt32(lotArray[lotIndex]) * (orderNewPriceList.Count) * cost;

            totalNetProfit += oneNetProfit;

            debugMsg("平倉點數---->" + orderEvenPrice);

            debugMsg("平倉時間---->" + orderEvenTime);

            debugMsg("純利潤 : " + oneNetProfit);

            debugMsg("總純利 : " + totalNetProfit);

            debugMsg("停損策略 : " + loseLine[orderNewPriceList.Count]);

            debugMsg("停利策略 : " + winLine[orderNewPriceList.Count]);

            if (orderNewPriceList.Count > 1)
            {
                debugMsg("反轉策略 : " + reverseLine[orderNewPriceList.Count - 1]);
            }

            debugMsg("----------------------------------------------------------------------------------------------");

            isStartOrder = false;

            befofeRecord = null;

            orderNewPriceList.Clear();//平倉後，把新倉(包括加碼的新倉)列表清空。            

            minTradePoint = 99999;//新倉後最低價

            maxTradePoint = 0;//新倉後最高價

        }

        private void dealOutWinSell()//賣出停利
        {
            debugMsg("賣出停利");

            prevTradeType = BS_Type_S;

            winOut();
        }

        private void dealOutWinBuy()//買入停利
        {
            debugMsg("買入停利");

            prevTradeType = BS_Type_B;

            winOut();
        }

        private void dealOutLossSell()//賣出停損
        {

            debugMsg("賣出停損");

            prevTradeType = BS_Type_S;

            loseOut();

        }

        private void dealOutLossBuy()//買入停損
        {
            debugMsg("買入停損");

            prevTradeType = BS_Type_B;

            loseOut();
        }


        private void winOut()//反轉獲利出場
        {
            dealOutCore();

            continueLoseTimes++;

            winCount++;

            isPrevWin = true;

            isPrevLose = false;


        }

        private void loseOut()//認賠出場
        {
            dealOutCore();

            lotIndex = Array_Begin_Index;

            loseCount++;

            isPrevWin = false;

            isPrevLose = true;

        }

        public void trackMsg(String msg)
        {
            if (DEBUG)
            {

                try
                {
                    trackFile.writeLine(msg);
                }
                catch (Exception e)
                {
                    throw new Exception("寫入軌跡檔失敗，請檢查是否有Track這個目錄。" + e.StackTrace);
                }
            }
        }

        private void debugMsg(String msg)
        {
            if (DEBUG)
            {
                Console.WriteLine(msg);

                try
                {
                    tradeRecordFile.writeLine(msg);
                }
                catch (Exception e)
                {
                    throw new Exception("寫入交易紀錄檔失敗，請檢查是否有Output這個目錄。" + e.StackTrace);
                }
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

                string[] direction = new string[checkCount];


                for (int i = 0; i < checkCount; i++)
                {
                    direction[i] = "NONE";

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
                        direction[i] = BS_Type_B;
                    }
                    else if (record.TradePrice < basePrice[i])//目前交易金額小於XX分鐘前的交易金額
                    {
                        direction[i] = BS_Type_S;
                    }

                }

                for (int i = checkCount - 1; i > 0; i--)
                {
                    if (direction[i] != direction[i - 1]) { return false; }
                }

                if (direction[0].Equals("NONE"))//五個時間的價位都一樣
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


    }
}