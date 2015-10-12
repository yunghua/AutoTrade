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
        const double cost = 70;//手續費成本

        const double valuePerPoint = 50;//每點價值，小台50元/點，大台200元/點        

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

        const int Out_Not_Out = 0;//尚未平倉出場
        const int Out_Active_Profit = 1;//動態停利出場
        const int Out_Win_Sell = 2;//賣出停利
        const int Out_Win_Buy = 3;//買入停利
        const int Out_Loss_Sell = 4;//賣出停損
        const int Out_Loss_Buy = 5;//買入停損
        const int Out_Time_Up = 6;//時間到出場

        const int Array_Begin_Index = 0;//獲利加碼的口數陣列初始值

        //const String Config_Dir = "Config";

        const String Output_Dir = "Output";

        const String Strategy_File_Name = "Strategy.txt";//策略檔案名

        //const String Config_File_Name = "TradeConfig.txt";//設定檔案名

        //const String Month_File_Name = "TradeMonth.txt";//月份代碼檔

        const Boolean DEBUG = true;

        const Boolean END_TRADE = true;

        const Boolean NEXT_TRADE = false;

        const int Random_Seed = 888;//隨機參數種子

        const int SELL = 2;//交易方式:賣

        const int BUY = 1;//交易方式:買

        const int MaxTradeCount = 100;//最大交易次數        

        const double MaxProfitLoss = -5000;//最大虧損水平線

        enum TradeType : int { SELL, BUY };

        //const int SellOrBuyCheckPeriod = 60;//交易買賣方向的檢查時間間隔,60秒前

        const int ActiveProfitStartPeriod = 5;//檢查動態停利啟動條件的時間基準，5秒前

        const int ActiveProfitCheckPeriod = 1;//動態停利的檢查時間間隔,一分鐘後

        const int ActiveProfitPoint = 30;//動態停利的啟動條件

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Boolean變數。
        /// </summary>      

        Boolean isStartOrder = false;//是否開始下單

        Boolean isPrevWin = false;

        Boolean isPrevLose = false;

        Boolean isActiveCheckProfit = false;//是否動態停利

        Boolean isStopTodayTrade = false; //是否停止今日交易

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 變數。
        /// </summary>
        ///         
        int orderAPIStatus = Stage_New;//下單API是否登入成功

        int number = 0;//超過檢查時間的次數

        double tempOneProfit = 99999;//單筆暫時獲利

        double minTradePoint = 99999;//市場最低價

        double maxTradePoint = 0;//市場最高價

        double offsetTradePoint;//最高與最低價格差異區間値 = maxTradePoint - minTradePoint        

        double oneProfit = 0;//單筆利潤

        double totalProfit = 0;//總利潤

        double onePureProfit = 0;//單筆純利

        double totalPureProfit = 0;//總純利

        string tradeTime = "";//交易時間點

        double orderPrice = 0;//下單交易價格

        double evenPrice = 0;//平倉交易價格        

        int nowStrategyCount = 1; //目前使用哪一行的停損停利規則

        int maxStrategyCount = 0; //停損停利規則最大有幾種

        int winCount = 0;//獲利次數

        int loseCount = 0;//賠錢次數

        int[] SellOrBuyCheckPeriod;//交易買賣方向的檢查時間間隔

        int nowTradeType = 0;//交易方式賣或是買

        int prevTradeType = 0;//上一次交易方式賣或是買        

        string monthFilePath = "";//完整的月份代碼檔案路徑        

        string configFilePath = "";//完整的設定檔案路徑        

        string tradeCode = "";//商品代碼

        string outputLine = "";//寫入RPT檔案的內容

        string tradeRecordFileName = "";//自動交易紀錄

        string fileName = "";

        int maxLoss;//單日最大停損

        //-------------------------------------------------------------------------------------------------------------
        //          物件區
        //-------------------------------------------------------------------------------------------------------------    

        Form1 parentForm;//UI介面

        TradeFile allTradeOutputFile;          //當天所有交易紀錄
        StrategyFile strategyFile;                        //策略檔
        TradeFile tradeRecordFile;                //自動交易紀錄檔
        //ConfigFile configFile;                       //設定檔

        OriginalRecord befofeRecord;

        OriginalRecord record = new OriginalRecord();//檔案的一行紀錄     


        DateTime[] minuteBeforeTradeTime;//交易前X秒，判斷買或賣

        DateTime secondAfterTradeToActiveCheck;//交易後X秒，例如五秒鐘內利潤擴大30點開始動態停利

        DateTime minuteAfterStartActiveProfit;//開始動態停利檢查，每一分鐘一次

        List<OriginalRecord> recordList = null;//所有交易紀錄

        DateTime tradeDateTime;//交易時間點

        //DateTime secondsBeforeTradeTime;//交易前X秒，判斷買或賣

        Dictionary<int, int> loseLine;  //認賠的底線
        Dictionary<int, int> winLine;  //停利的底線

        DateTime stopTradeTime;//今日交易結束時間
        //-------------------------------------------------------------------------------------------------------------        
        //        下單區
        //-------------------------------------------------------------------------------------------------------------

        private YuantaOrdLib.YuantaOrdClass m_yuanta_ord = new YuantaOrdClass();

        const string Function_Code_01 = "01";//委託
        const string Function_Code_02 = "02";//減量
        const string Function_Code_03 = "03";//刪單
        const string Function_Code_04 = "04";//改價

        const string Type_Code_0 = "0";//期貨
        const string Type_Code_1 = "1";//選擇權

        const string Sub_Account = "";//子帳號

        const string Order_No = "";//委託書號

        const string BS_Type_B = "B";//買
        const string BS_Type_S = "S";//賣

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

        string orderDircetion;//下單方向，買或賣

        double orderNewPrice;//新倉成交均價
        double orderEvenPrice;//平倉成交均價

        string orderEvenTime;//平倉時間

        int outStyle = Out_Not_Out;//平倉出場方式，有動態停利、賣出/買入停利、賣出/買入停損等


        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------




        public TradeMaster()
        {
        }

        public TradeMaster(Form1 parentForm)
        {
            this.parentForm = parentForm;
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

        //private void prepareConfig()
        //{
        //    if (configFile != null)
        //    {
        //        tradeCode = configFile.readConfig("Trade_Code");

        //        branchCode = configFile.readConfig("Branch_Code");

        //        accountCode = configFile.readConfig("Account_Code");

        //        id = configFile.readConfig("ID");

        //        password = configFile.readConfig("Password");

        //        lots = configFile.readConfig("Lots");

        //        lotArray = lots.Split(',');
        //    }
        //    else
        //    {
        //        throw new Exception("沒有設定檔!");
        //    }
        //}

        private void showParameters()
        {
            debugMsg("Max_Loss:" + maxLoss);
            debugMsg("Lots:" + lots);
            //debugMsg("Branch_Code:" + branchCode);
            //debugMsg("Account_Code:" + account);
            //debugMsg("ID:" + id);
            //debugMsg("Password:" + password);

        }

        public void prepareReady()
        {


            lotArray = lots.Split(',');

            string appDir = System.Windows.Forms.Application.StartupPath;

            stage = Stage_New;

            loseLine = new Dictionary<int, int>();

            winLine = new Dictionary<int, int>();

            //configFilePath = appDir + "\\" + Config_Dir + "\\" + Config_File_Name;

            //configFile = new ConfigFile(configFilePath);

            //configFile.prepareReader();

            //prepareConfig();//讀取設定檔案內容

            //monthFilePath = appDir + "\\" + Config_Dir + "\\" + Month_File_Name;

            //tradeCode = TradeUtility.TradeUtility.getInstance().dealTradeCode(monthFilePath, tradeCode);//設定好期貨商品代碼

            strategyFile = StrategyFile.getInstance();

            strategyFile.dealStrategyRule(appDir);

            this.winLine = strategyFile.getWinLine();

            this.loseLine = strategyFile.getLoseLine();

            SellOrBuyCheckPeriod = new int[5];

            SellOrBuyCheckPeriod[4] = 25;//交易買賣方向的檢查時間間隔,25秒前

            SellOrBuyCheckPeriod[3] = 20;//交易買賣方向的檢查時間間隔,20秒前     

            SellOrBuyCheckPeriod[2] = 15;//交易買賣方向的檢查時間間隔,15秒前

            SellOrBuyCheckPeriod[1] = 10;//交易買賣方向的檢查時間間隔,10秒前

            SellOrBuyCheckPeriod[0] = 5;//交易買賣方向的檢查時間間隔,5秒前            

            isStartOrder = false;//是否開始下單

            isPrevWin = false;//上一次交易是否獲利

            DateTime now = System.DateTime.Now;

            stopTradeTime = new DateTime(now.Year, now.Month, now.Day, 13, 44, 0);

            appDir = System.Windows.Forms.Application.StartupPath;

            fileName = appDir + "\\" + Output_Dir + "\\" + "AllOutput_" + now.Year + "_" + now.Month + "_" + now.Day + ".rpt";

            allTradeOutputFile = new TradeFile(fileName);

            allTradeOutputFile.prepareWriter();

            tradeRecordFileName = appDir + "\\" + Output_Dir + "\\" + "TradeRecord_" + now.Year + "_" + now.Month + "_" + now.Day + ".rpt";

            tradeRecordFile = new TradeFile(tradeRecordFileName);

            tradeRecordFile.prepareWriter();

            recordList = new List<OriginalRecord>();

            befofeRecord = new OriginalRecord();

            RecordScanner.setRecordList(recordList);

            debugMsg(now + "------<<開始交易>>");

            showParameters();

            prepareOrder();

        }

        public void stop()
        {
            try
            {
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
                outputLine = matchTime + "," + matchPrice + "," + matchQuantity;

                allTradeOutputFile.writeLine(outputLine);


                if (!isStopTodayTrade)
                {
                    startTrade(matchTime, matchPrice, matchQuantity);//執行交易程式
                }



            }
            catch (IOException e)
            {
                throw e;
            }
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


        // Order API 登入
        private void loginOrder()
        {

            int ret_code = m_yuanta_ord.SetFutOrdConnection(id, password, ipAPI, 80);

            // 回傳 2 表示已經在 "已經登入" 連線狀態  
            if (ret_code == 2)
            {
                debugMsg("Order API已經登入!");

                orderAPIStatus = Stage_Order_Login_Success;

                parentForm.setTradeMasterMessage("Order API已經登入!");
            }

        }//end of login

        // TLinkStatus: 回傳連線狀態, AccList: 回傳帳號, Casq: 憑證序號, Cast: 憑證狀態
        void m_yuanta_ord_OnLogonS(int TLinkStatus, string AccList, string Casq, string Cast)
        {
            debugMsg(String.Format("OnLogonS: {0},{1},{2},{3}", TLinkStatus, AccList, Casq, Cast));

            if (TLinkStatus == 2)//登入成功
            {
                debugMsg("Order API登入成功! " + TLinkStatus.ToString());

                orderAPIStatus = Stage_Order_Login_Success;

                parentForm.setTradeMasterMessage("Order API登入成功!");
            }
        }

        private void prepareOrder()
        {

            loginOrder();



            try
            {
                m_yuanta_ord.OnLogonS += new _DYuantaOrdEvents_OnLogonSEventHandler(m_yuanta_ord_OnLogonS);
                m_yuanta_ord.OnOrdMatF += new _DYuantaOrdEvents_OnOrdMatFEventHandler(m_yuanta_ord_OnOrdMatF);
                m_yuanta_ord.OnOrdRptF += new _DYuantaOrdEvents_OnOrdRptFEventHandler(m_yuanta_ord_OnOrdRptF);
                //m_yuanta_ord.OnDealQuery += new _DYuantaOrdEvents_OnDealQueryEventHandler(m_yuanta_ord_OnDealQuery);
                //m_yuanta_ord.OnReportQuery += new _DYuantaOrdEvents_OnReportQueryEventHandler(m_yuanta_ord_OnReportQuery);
                //m_yuanta_ord.OnOrdResult += new _DYuantaOrdEvents_OnOrdResultEventHandler(m_yuanta_ord_OnOrdResult);
                //m_yuanta_ord.OnRfOrdRptRF += new _DYuantaOrdEvents_OnRfOrdRptRFEventHandler(m_yuanta_ord_OnRfOrdRptRF);
                //m_yuanta_ord.OnRfReportQuery += new _DYuantaOrdEvents_OnRfReportQueryEventHandler(m_yuanta_ord_OnRfReportQuery);
                //m_yuanta_ord.OnRfOrdMatRF += new _DYuantaOrdEvents_OnRfOrdMatRFEventHandler(m_yuanta_ord_OnRfOrdMatRF);
                //m_yuanta_ord.OnRfDealQuery += new _DYuantaOrdEvents_OnRfDealQueryEventHandler(m_yuanta_ord_OnRfDealQuery);
                //m_yuanta_ord.OnUserDefinsFuncResult += new _DYuantaOrdEvents_OnUserDefinsFuncResultEventHandler(m_yuanta_ord_OnUserDefinsFuncResult);

            }
            catch (Exception e)
            {
                throw e;
            }
        }




        // 即時成交回報
        void m_yuanta_ord_OnOrdMatF(string Omkt, string Buys, string Cmbf, string Bhno, string AcNo, string Suba, string Symb, string Scnam, string O_Kind, string S_Buys, string O_Prc, string A_Prc, string O_Qty, string Deal_Qty, string T_Date, string D_Time, string Order_No, string O_Src, string O_Lin, string Oseq_No)
        {
            string mat_str = String.Format("Omkt={0},Buys={1},Cmbf={2},Bhno={3},Acno={4},Suba={5},Symb={6},Scnam={7},O_Kind={8},S_Buys={9},O_Prc={10},A_Prc={11},O_Qty={12},Deal_Qty={13},T_Date={14},D_Time={15},Order_No={16},O_Src={17},O_Lin={18},Oseq_No={19}"
                                    , Omkt.Trim(), Buys.Trim(), Cmbf.Trim(), Bhno.Trim(), AcNo.Trim(), Suba.Trim(), Symb.Trim(), Scnam.Trim(), O_Kind.Trim(), S_Buys.Trim(), O_Prc.Trim(), A_Prc.Trim(), O_Qty.Trim(), Deal_Qty.Trim(), T_Date.Trim(), D_Time.Trim(), Order_No.Trim(), O_Src.Trim(), O_Lin.Trim(), Oseq_No.Trim());

            debugMsg("即時成交回報" + mat_str);

            double dealPrice;

            dealPrice = Convert.ToDouble(A_Prc.Trim());//成交均價            

            dealQty = Convert.ToInt32(Deal_Qty.Trim());//已成交量

            orderEvenTime = D_Time.Trim();//成交時間

            if (stage == Stage_Order_New_Start)//新倉
            {
                stage = Stage_Order_New_Success;

                orderNewPrice = dealPrice;

                isStartOrder = true;//下單啦    

            }
            else if (stage == Stage_Order_Even_Start)//平倉
            {
                stage = Stage_Order_Even_Success;

                orderEvenPrice = dealPrice;

                isStartOrder = false;//此次交易結束，準備下一次交易                 

                dealOut();//處理交易結束後，寫紀錄檔，以及相關參數
            }

        }

        // 即時委託回報
        void m_yuanta_ord_OnOrdRptF(string Omkt, string Mktt, string Cmbf, string Statusc, string Ts_Code, string Ts_Msg, string Bhno, string AcNo, string Suba, string Symb, string Scnam, string O_Kind, string O_Type, string Buys, string S_Buys, string O_Prc, string O_Qty, string Work_Qty, string Kill_Qty, string Deal_Qty, string Order_No, string T_Date, string O_Date, string O_Time, string O_Src, string O_Lin, string A_Prc, string Oseq_No, string Err_Code, string Err_Msg, string R_Time, string D_Flag)
        {
            string rpt_str = String.Format("Omkt={0},Mktt={1},Cmbf={2},Statusc={3},Ts_Code={4},Ts_Msg={5},Bhno={6},Acno={7},Suba={8},Symb={9},Scnam={10},O_Kind={11},O_Type={12},Buys={13},S_Buys={14},O_Prc={15},O_Qty={16},Work_Qty={17},Kill_Qty={18},Deal_Qty={19},Order_No={20},T_Date={21},O_Date={22},O_Time={23},O_Src={24},O_Lin={25},A_Prc={26},Oseq_No={27},Err_Code={28},Err_Msg={29},R_Time={30},D_Flag={31}"
                                    , Omkt.Trim(), Mktt.Trim(), Cmbf.Trim(), Statusc.Trim(), Ts_Code.Trim(), Ts_Msg.Trim(), Bhno.Trim(), AcNo.Trim(), Suba.Trim(), Symb.Trim(), Scnam.Trim(), O_Kind.Trim(), O_Type.Trim(), Buys.Trim(), S_Buys.Trim(), O_Prc.Trim(), O_Qty.Trim(), Work_Qty.Trim(), Kill_Qty.Trim(), Deal_Qty.Trim(), Order_No.Trim(), T_Date.Trim(), O_Date.Trim(), O_Time.Trim(), O_Src.Trim(), O_Lin.Trim(), A_Prc.Trim(), Oseq_No.Trim(), Err_Code.Trim(), Err_Msg.Trim(), R_Time.Trim(), D_Flag.Trim());

            debugMsg("即時委託回報" + rpt_str);

            if (Ts_Code.Equals("05") || Ts_Code.Equals("07") || Ts_Code.Equals("11"))
            {
                if (stage == Stage_Order_New_Start)
                {
                    stage = Stage_Order_New_Fail;
                }
                else if (stage == Stage_Order_Even_Start)
                {
                    stage = Stage_Order_Even_Fail;
                }
                else
                {
                    stage = Stage_Order_Fail;
                }

            }

        }

        private int dealOrderNew(string tradeCode, string orderPrice, string orderLot, string orderDirection)//用來下單建立新倉，回傳值:stage
        {
            try
            {
                orderPrice = "";//市價下單

                dealOrder(tradeCode, orderPrice, orderLot, orderDirection, Trade_Type_NEW);
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



            if (orderAPIStatus != Stage_Order_Login_Success)
            {
                stage = Stage_Order_Not_Process;

                throw new Exception("下單API尚未登入成功!");
            }

            m_yuanta_ord.SetWaitOrdResult(1);//等待委託單回報
            //m_yuanta_ord.SetWaitOrdResult(0);//不等待委託單回報

            string ret_no = string.Empty;

            ret_no = m_yuanta_ord.SendOrderF(Function_Code_01, Type_Code_0, branchCode, account, Sub_Account, Order_No, orderDirection, tradeCode, orderPrice, orderLot, tradeType, Price_Type_M, Order_Cond_I, "", "");

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

                record.TradePrice = Convert.ToDouble(matchPrice);

                record.TradeVolumn = Convert.ToInt32(matchQuantity);

                recordList.Add(record);

            }
            catch (Exception e)
            {
                debugMsg(e.StackTrace);
            }

            coreLogic();

        }




        private void coreLogic()//回傳値表示是否結束本日交易
        {
            if (stage == Stage_Order_New_Fail)//下單失敗
            {
                isStartOrder = false;
            }
            if (isStartOrder == false && stage != Stage_Order_New_Start && (isPrevLose == true || isPrevWin == true || Dice.run(Random_Seed))) //下單版本
            //if (isStartOrder == false && (isPrevLose == true || isPrevWin == true || Dice.run(Random_Seed))) //測試版本
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
                    return;//五個時間的的方向不同，下次繼續
                }

                secondAfterTradeToActiveCheck = tradeDateTime.AddSeconds(ActiveProfitStartPeriod);//5秒內利潤擴大50點

                minuteAfterStartActiveProfit = tradeDateTime.AddMinutes(1);//開始動態停利檢查，每一分鐘一次

                orderPrice = record.TradePrice;

                if (nowTradeType == TradeType.BUY.GetHashCode())
                {
                    debugMsg("交易方式---->" + TradeType.BUY);

                    orderDircetion = BS_Type_B;
                }
                else if (nowTradeType == TradeType.SELL.GetHashCode())
                {
                    debugMsg("交易方式---->" + TradeType.SELL);

                    orderDircetion = BS_Type_S;
                }

                debugMsg("交易金額---->" + orderPrice);

                debugMsg("交易時間---->" + tradeDateTime);

                //dealStrategyCount(winCount);//取得停損停利第幾階段，以獲利次數
                //dealStrategyCount(totalProfit);//取得停損停利第幾階段 ，以獲利金額

                try
                {
                    stage = this.dealOrderNew(tradeCode, Convert.ToString(orderPrice), lotArray[lotIndex], orderDircetion);
                }
                catch (Exception e)
                {
                    debugMsg(e.StackTrace);
                }

            }


            if (isStartOrder == true && stage == Stage_Order_New_Success)//已經開始下單，而且下單成功
            //if (isStartOrder == true)//已經開始下單
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


                    if (nowTradeType == TradeType.BUY.GetHashCode())
                    {
                        debugMsg("時間到買入平倉");

                        orderDircetion = BS_Type_S;
                    }
                    else if (nowTradeType == TradeType.SELL.GetHashCode())
                    {
                        debugMsg("時間到賣出平倉");

                        orderDircetion = BS_Type_B;
                    }

                    if (stage == Stage_Order_New_Success)
                    {
                        stage = this.dealOrderEven(tradeCode, Convert.ToString(evenPrice), lotArray[lotIndex], orderDircetion);
                    }

                    outStyle = Out_Time_Up;

                    debugMsg("outStyle = Out_Time_Up");

                }//end 交易時間結束
                else if ((isActiveCheckProfit == false) && (record.TradeMoment > secondAfterTradeToActiveCheck))//還沒開始【動態停利】，檢查時間到了，看看是否要啟動動態停利機制
                {

                    if (oneProfit - tempOneProfit > ActiveProfitPoint)
                    {
                        isActiveCheckProfit = true;//開始動態停利

                        debugMsg("開始動態停利---->" + record.TradeMoment + " ---------->Profit:" + oneProfit + "-----------tempOneProfit:" + tempOneProfit);

                        tempOneProfit = oneProfit;

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
                            debugMsg("動態停利範圍:" + reverseLitmit);

                            if (nowTradeType == TradeType.BUY.GetHashCode())
                            {
                                prevTradeType = TradeType.BUY.GetHashCode();

                                orderDircetion = BS_Type_S;
                            }
                            else
                            {
                                prevTradeType = TradeType.SELL.GetHashCode();

                                orderDircetion = BS_Type_B;
                            }

                            if (stage == Stage_Order_New_Success)
                            {
                                stage = this.dealOrderEven(tradeCode, Convert.ToString(evenPrice), lotArray[lotIndex], orderDircetion);
                            }

                            debugMsg("outStyle = Out_Active_Profit");

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


                        }
                    }// end 開始動態停利，到了檢查的時間


                }//end 執行動態停利檢查

                else if (nowTradeType == TradeType.BUY.GetHashCode() && (orderPrice - record.TradePrice) > loseLine[nowStrategyCount])
                {//賠了XX點，認賠殺出

                    orderDircetion = BS_Type_S;

                    if (stage == Stage_Order_New_Success)
                    {
                        stage = this.dealOrderEven(tradeCode, Convert.ToString(evenPrice), lotArray[lotIndex], orderDircetion);
                    }

                    debugMsg("outStyle = Out_Loss_Buy");

                }
                else if (nowTradeType == TradeType.SELL.GetHashCode() && (record.TradePrice - orderPrice) > loseLine[nowStrategyCount])
                {
                    //賠了XX點，認賠殺出

                    if (stage == Stage_Order_New_Success)
                    {
                        stage = this.dealOrderEven(tradeCode, Convert.ToString(evenPrice), lotArray[lotIndex], orderDircetion);
                    }

                    debugMsg("outStyle = Out_Loss_Sell");

                    outStyle = Out_Loss_Sell;

                }

                else if (nowTradeType == TradeType.BUY.GetHashCode() && (record.TradePrice - orderPrice) > winLine[nowStrategyCount])
                {
                    //賺了XX點，停利出場

                    if (stage == Stage_Order_New_Success)
                    {
                        stage = this.dealOrderEven(tradeCode, Convert.ToString(evenPrice), lotArray[lotIndex], orderDircetion);
                    }

                    debugMsg("outStyle = Out_Win_Buy");

                    outStyle = Out_Win_Buy;

                }

                else if (nowTradeType == TradeType.SELL.GetHashCode() && (orderPrice - record.TradePrice) > winLine[nowStrategyCount])
                {
                    //賺了XX點，停利出場

                    if (stage == Stage_Order_New_Success)
                    {
                        stage = this.dealOrderEven(tradeCode, Convert.ToString(evenPrice), lotArray[lotIndex], orderDircetion);
                    }

                    debugMsg("outStyle = Out_Win_Sell");

                    outStyle = Out_Win_Sell;

                }


            }//下單結束

            if (maxLoss < 0 && totalProfit * valuePerPoint < maxLoss)  //已達最大虧損水平線
            {

                loseOut();

                isStopTodayTrade = true;
            }


        }

        private void dealOut()
        {
            try
            {
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
                if (nowTradeType == TradeType.BUY.GetHashCode())
                {
                    oneProfit = orderEvenPrice - orderNewPrice;
                }
                else if (nowTradeType == TradeType.SELL.GetHashCode())
                {
                    oneProfit = orderNewPrice - orderEvenPrice;
                }

                oneProfit *= Convert.ToInt32(lotArray[lotIndex]);
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

        private void dealOutActiveProfit()//動態停利
        {
            debugMsg("動態停利。");

            dealOutCore();

            winOut();
        }

        private void dealOutCore()//處理平倉後記錄獲利動作的核心
        {
            calOneProfit();

            totalProfit += oneProfit;

            onePureProfit = oneProfit * valuePerPoint - Convert.ToInt32(lotArray[lotIndex]) * cost;

            totalPureProfit += onePureProfit;

            debugMsg("平倉點數---->" + orderEvenPrice);

            debugMsg("平倉時間---->" + orderEvenTime);

            debugMsg("純利潤 : " + onePureProfit);

            debugMsg("停損策略 : " + loseLine[nowStrategyCount]);

            debugMsg("停利策略 : " + winLine[nowStrategyCount]);

            debugMsg("----------------------------------------------------------------------------------------------");

            winOut();

        }

        private void dealOutWinSell()//賣出停利
        {
            debugMsg("賣出停利");

            dealOutCore();

            lotIndex++;

            if (lotIndex >= lotArray.Length)
            {
                lotIndex = lotArray.Length - 1;
            }

            prevTradeType = TradeType.SELL.GetHashCode();
        }

        private void dealOutWinBuy()//買入停利
        {
            debugMsg("買入停利");

            dealOutCore();

            lotIndex++;

            if (lotIndex >= lotArray.Length)
            {
                lotIndex = lotArray.Length - 1;
            }

            prevTradeType = TradeType.BUY.GetHashCode();
        }

        private void dealOutLossSell()//賣出停損
        {

            debugMsg("賣出停損");

            dealOutCore();

            lotIndex = Array_Begin_Index;

            prevTradeType = TradeType.SELL.GetHashCode();

        }

        private void dealOutLossBuy()//買入停損
        {
            debugMsg("買入停損");

            dealOutCore();

            lotIndex = Array_Begin_Index;

            prevTradeType = TradeType.BUY.GetHashCode();
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

            isActiveCheckProfit = false;//停止動態停利檢查

            tempOneProfit = 99999;

            number = 0;//超過檢查時間的次數

            befofeRecord = null;

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
                        direction[i] = TradeType.BUY.GetHashCode();
                    }
                    else
                    {
                        direction[i] = TradeType.SELL.GetHashCode();
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
