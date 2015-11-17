using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradeUtility;
using YuantaOrdLib;

namespace AutoTrade
{
    public partial class Form1 : Form
    {
        List<ConfigFile> trackFileList;//軌跡檔案列表

        Boolean isOrderAPIReady = false;

        string tradeMasterMessage = "";//來自TradeMaster的訊息

        string appDir = "";//應用程式所在目錄

        const string Config_Dir = "Config";//設定檔目錄

        const string Track_Dir = "Track";//往日交易檔目錄        

        const string Config_File_Name = "TradeConfig.txt";//設定檔案名

        const string Month_File_Name = "TradeMonth.txt";//交易月份代碼對照表

        string configFilePath = "";//完整的設定檔案目錄

        ConfigFile configFile;

        TradeMaster master;

        string ipAPI = "api.yuantafutures.com.tw";//下單伺服器的網址
        string ipQuote = "quote.yuantafutures.com.tw";//行情伺服器的網址

        string port = "";//port=80,數字

        string id = "";
        string password = "";

        string tradeCode = "";//元大期貨下單，市場別代碼，小台指=MXF，大台指=TXF

        string maxLoss = "";//單日最大停損

        string lots = "";//交易筆數

        string branchCode = "";//分公司代碼

        string account = "";//帳號

        private YuantaOrdLib.YuantaOrdClass yuantaOrderAPI;

        string futuresCode = "";//期貨代碼，大台指TX，小台指MTX

        string lotLimit = "";//最大加碼限制

        string trackFileDir = "";//往日交易檔案的完整目錄路徑

        string tradeCodeLastDay = "";//上一個交易日的交易月份 

        public Form1()
        {
            InitializeComponent();


        }


        public void setTradeMasterMessage(string msg)
        {
            this.tradeMasterMessage = msg;

            textBox_status2.Text = msg;
        }

        public string getTradeMasterMessage()
        {
            return tradeMasterMessage;
        }


        //連線Event OnMktStatusChange (int Status, char* Msg)	與行情發送端連線的狀態,回傳LinkStatus 
        private void axYuantaQuote1_OnMktStatusChange(object sender, AxYuantaQuoteLib._DYuantaQuoteEvents_OnMktStatusChangeEvent e)
        {
            textBox_status.Text = DateTime.Now.ToString("HH:mm:ss.fff ") + e.msg.ToString();
            if (e.msg.ToString().IndexOf("行情連線結束") >= 0)
            {
                //隔幾秒再連線

                textBox_status.Text = DateTime.Now.ToString("HH:mm:ss.fff ") + "行情連線結束，隔5秒重新連線";
                timer1.Enabled = true;
            }
            else if (e.msg.ToString().IndexOf("行情連線失敗") >= 0)
            {
                //隔幾秒再連線
                //可能網路不通
                textBox_status.Text = DateTime.Now.ToString("HH:mm:ss.fff ") + "行情連線失敗，隔5秒重新連線";
                timer1.Enabled = true;
            }
            else
            {

                register();

            }
        }

        private void axYuantaQuote1_OnGetMktAll(object sender, AxYuantaQuoteLib._DYuantaQuoteEvents_OnGetMktAllEvent e)
        {
            master.process(e);
        }




        private void loginQuote()
        {
            try
            {
                axYuantaQuote1.SetMktLogon(id, password, ipQuote, port);
                //axYuantaQuote1.SetMktLogon(textBox_id.Text.Trim(), textBox_pass.Text.Trim(), textBox_ip.Text.Trim(), textBox_port.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show("SetMktLogon失敗：" + ex.Message);

            }
        }

        // Order API 登入
        private void loginOrder()
        {
            try
            {
                int ret_code = yuantaOrderAPI.SetFutOrdConnection(id, password, ipAPI, 80);

                // 回傳 2 表示已經在 "已經登入" 連線狀態  
                if (ret_code == 2)
                {

                    textBox_status_order.Text = "已經登入";


                    timer2.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("LoginOrder失敗：" + ex.Message);

            }
        }

        // TLinkStatus: 回傳連線狀態, AccList: 回傳帳號, Casq: 憑證序號, Cast: 憑證狀態
        void yuantaOrderAPI_OnLogonS(int TLinkStatus, string AccList, string Casq, string Cast)
        {

            if (TLinkStatus == 2)//登入成功
            {

                textBox_status_order.Text = DateTime.Now.ToString("HH:mm:ss.fff ") + "交易API登入成功: ";

                isOrderAPIReady = true;

                timer2.Enabled = false;

                try
                {
                    master.setIsOrderAPIReady(isOrderAPIReady);

                    textBox_status_ready.Text = "isOrderAPIReady :" + isOrderAPIReady;
                }
                catch (Exception e)
                {

                    MessageBox.Show("yuantaOrderAPI_OnLogonS:" + e.StackTrace);
                }
            }
            else
            {
                textBox_status_order.Text = DateTime.Now.ToString("HH:mm:ss.fff ") + "交易API連線失敗，隔5秒重新連線";
                timer2.Enabled = true;
            }
        }

        private void register()
        {
            try
            {
                int RegErrCode = axYuantaQuote1.AddMktReg(tradeCode, "4");
                //int RegErrCode = axYuantaQuote1.AddMktReg(textBox_sym.Text.Trim(), comboBox_UpdateMode.Text.Substring(0, 1));

                textBox_status2.Text = DateTime.Now.ToString("HH:mm:ss.fff ") + RegErrCode.ToString();

                textBox_sym.Text = tradeCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show("AddMktReg失敗：" + ex.Message);
            }
        }



        private void axYuantaQuote1_OnRegError(object sender, AxYuantaQuoteLib._DYuantaQuoteEvents_OnRegErrorEvent e)
        {
            textBox_status2.Text = e.errCode.ToString();
        }


        private void readTrackFile(TradeMaster master)
        {
            if (trackFileList != null && trackFileList.Count > 0)
            {

                ConfigFile trackFile = trackFileList[trackFileList.Count - 1];

                List<string> contextList = new List<string>();

                if (trackFile != null)
                {

                    tradeCodeLastDay = trackFile.readConfig("TradeCode");

                    textBox_tradeCodeLastDay.Text = tradeCodeLastDay;

                    if (tradeCode != null && tradeCodeLastDay != null && !tradeCodeLastDay.Trim().Equals(""))
                    {
                        if (!tradeCode.Trim().Equals(tradeCodeLastDay))//交易月份不同
                        {
                            return;
                        }
                        else//交易月份相同
                        {
                            master.Stage = TradeMaster.Stage_Last_Day;
                        }
                    }

                    
                    while(trackFile.hasNext()){//把上一個交易日的軌跡檔寫進今天的軌跡檔內

                        master.trackMsg(trackFile.getLine().Trim());

                    }


                    contextList = trackFile.readMultiLineConfig("NewTrade", "EndTrade");

                    if (contextList != null && contextList.Count >= 1)
                    {
                        master.IsStartOrderLastDay = true;

                        contextList.Clear();
                    }



                    contextList = trackFile.readMultiLineConfig("OrderPrice", "EndTrade");

                    if (contextList != null && contextList.Count >= 1)
                    {
                        for (int i = 0; i < contextList.Count; i++)
                        {
                            //orderPriceList.Add(Convert.ToDouble(contexList[i]));
                            if (i == 0)
                            {
                                master.OrderPrice = Convert.ToInt16(contextList[0]);
                            }
                            else
                            {
                                master.AddList.Add(Convert.ToInt16(contextList[i]));
                            }

                            master.OrderNewPriceList.Add(Convert.ToInt16(contextList[i]));

                            master.OrderNewPrice = Convert.ToInt16(contextList[i]);

                        }

                        contextList.Clear();
                    }

                    contextList = trackFile.readMultiLineConfig("BuyOrSell", "EndTrade");

                    if (contextList != null && contextList.Count >= 1)
                    {
                        master.OrderDircetion = contextList[contextList.Count - 1];


                        master.NowTradeType = contextList[contextList.Count - 1];


                        contextList.Clear();
                    }

                    contextList = trackFile.readMultiLineConfig("MaxPrice", "EndTrade");

                    if (contextList != null && contextList.Count >= 1)
                    {
                        master.MaxTradePointLastDay = Convert.ToInt16(contextList[contextList.Count - 1]);

                        contextList.Clear();
                    }

                    contextList = trackFile.readMultiLineConfig("MinPrice", "EndTrade");

                    if (contextList != null && contextList.Count >= 1)
                    {
                        master.MinTradePointLastDay = Convert.ToInt16(contextList[contextList.Count - 1]);

                        contextList.Clear();
                    }


                    trackFile.close();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {


            this.yuantaOrderAPI = new YuantaOrdClass();

            try
            {
                yuantaOrderAPI.OnLogonS += new _DYuantaOrdEvents_OnLogonSEventHandler(yuantaOrderAPI_OnLogonS);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            timer1.Enabled = false;

            label_Version.Text = TradeUtility.TradeUtility.version;

            appDir = System.Windows.Forms.Application.StartupPath;

            trackFileDir = appDir + "\\" + Track_Dir + "\\";

            FileManager fm = new FileManager();

            trackFileList = fm.getConfigFileList(trackFileDir);

            configFilePath = appDir + "\\" + Config_Dir + "\\" + Config_File_Name;

            configFile = new ConfigFile(configFilePath);
            try
            {
                configFile.prepareReader();
            }
            catch (Exception ex)
            {
                MessageBox.Show("讀取  設定檔  失敗。原因 : " + ex.Message);

                return;
            }

            tradeCode = configFile.readConfig("Trade_Code");

            string tradeMonthFilePath = appDir + "\\" + Config_Dir + "\\" + Month_File_Name;

            try
            {

                tradeCode = TradeUtility.TradeUtility.getInstance().dealTradeCode(tradeMonthFilePath, tradeCode);

            }
            catch (Exception ex)
            {

                MessageBox.Show("讀取  月份代碼檔  失敗。原因 : " + ex.Message);

                return;
            }

            ipQuote = configFile.readConfig("IP_Quote");

            id = configFile.readConfig("ID");

            password = configFile.readConfig("Password");

            ipAPI = configFile.readConfig("IP_API");

            port = configFile.readConfig("Port");

            lots = configFile.readConfig("Lots");

            maxLoss = configFile.readConfig("Max_Loss");

            branchCode = configFile.readConfig("Branch_Code");

            account = configFile.readConfig("Account_Code");

            futuresCode = configFile.readConfig("Futures_Code");

            lotLimit = configFile.readConfig("Lot_Limit");

            try
            {

                master = new TradeMaster(this);

                master.setLotLimit(Convert.ToInt16(lotLimit));

                master.setFuturesCode(futuresCode);

                master.setMaxLoss(Convert.ToInt32(maxLoss));

                master.setID(id);

                master.setPassword(password);

                master.setBranchCode(branchCode);

                master.setAccount(account);

                master.setLots(lots);

                master.setIpAPI(ipAPI);

                master.setOrderAPI(yuantaOrderAPI);

                master.setTradeCode(tradeCode);

                master.prepareReady();

                master.prepareTrackFile();

                readTrackFile(master);

                master.prepareDataFromLastTradeDay();                

            }
            catch (Exception ez)
            {
                MessageBox.Show(ez.Message + "--" + ez.StackTrace);

                return;
            }

            textBox_loseLine.Text = Convert.ToString(master.getLoseLine()[1]);

            textBox1_winLine.Text = Convert.ToString(master.getWinLine()[1]);

            textBox_reverseLine.Text = Convert.ToString(master.getReverseLine()[1]);

            textBox_B_S.Text = master.OrderDircetion;

            textBox_MaxPrice.Text = Convert.ToString(master.MaxTradePointLastDay);

            textBox_MinPrice.Text = Convert.ToString(master.MinTradePointLastDay);

            textBox_OrderPrice.Text = Convert.ToString(master.OrderPrice);

            textBox_OrderStart.Text = Convert.ToString(master.IsStartOrder);

            if (master.OrderNewPriceList != null)
            {
                for (int i = 0; i < master.OrderNewPriceList.Count; i++)
                {
                    textBox_OrderNewPriceList.Text += master.OrderNewPriceList[i] + Environment.NewLine;
                }
            }



            loginQuote();

            loginOrder();

        }

        private void Form1_Close(object sender, EventArgs e)
        {
            if (configFile != null)
            {
                configFile.close();
            }

            if (master != null)
            {
                master.stop();
            }

            if (yuantaOrderAPI != null)
            {
                this.yuantaOrderAPI.DoLogout();
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            loginQuote();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Enabled = false;
            loginOrder();
        }


        private void textBox_sym_Click(object sender, EventArgs e)
        {
            textBox_sym.SelectAll();
        }



        private void checkBox_enableTrade_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_enableTrade.Checked)
            {
                master.setEnableTrade(true);
            }
            else
            {
                master.setEnableTrade(false);
            }
        }



    }
}
