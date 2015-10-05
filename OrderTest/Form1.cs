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

namespace OrderTest
{
    public partial class Form1 : Form
    {
        const string configFilePath = "C:/Trader/TraderConfig.txt";
        

        const string Trade_Code = "MXFJ5";//代碼，小台指期貨MXF，十月=Ｊ，2015年=5

        const string Function_Code_01 = "01";//委託
        const string Function_Code_02 = "02";//減量
        const string Function_Code_03 = "03";//刪單
        const string Function_Code_04 = "04";//改價

        const string Type_Code_0 = "0";//期貨
        const string Type_Code_1 = "1";//選擇權

        

        const string Sub_Account = "";//子帳號

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

        string tradePrice = "";
        string tradeCode = "";//商品代碼

         string branchCode = "";//分公司

         string accountCode = "";//帳號

         string id = "";

         string password = "";

         string lots = "";//交易筆數


        ConfigFile configFile;//設定檔
        


        public Form1()
        {
            InitializeComponent();
            configFile = new ConfigFile(configFilePath);
            configFile.prepareReader();

            

        }

        private YuantaOrdLib.YuantaOrdClass m_yuanta_ord = null;

        

        

       

        private void Form1_Load(object sender, EventArgs e)
        {
            m_yuanta_ord = new YuantaOrdClass();
            this.button_Buy.Enabled = false;
            this.button_Buy.Visible = false;

            this.button_Sell.Enabled = false;
            this.button_Sell.Visible = false;


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

            //login();

            tradeCode = configFile.readConfig("Trade_Code");

            branchCode = configFile.readConfig("Branch_Code");

            accountCode = configFile.readConfig("Account_Code");

            id = configFile.readConfig("ID");

            password = configFile.readConfig("Password");

            lots = configFile.readConfig("Lots");
            
            try
            {
                tradeCode = TradeUtility.TradeUtility.getInstance().dealTradeCode(tradeCode);
                textBox_Status.Text = tradeCode;
            }
            catch (Exception ex)
            {
                textBox_Status.Text = ex.Message;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_yuanta_ord.DoLogout();

            configFile.close();
            
        }


        // API 登入
        private void login()
        {

            int ret_code = m_yuanta_ord.SetFutOrdConnection(id, password, "api.yuantafutures.com.tw", 80);
            textBox_Status.Text = ret_code.ToString();

            //LogMessage(String.Format("SetFutOrdConnection() = {0}", ret_code));

            // 回傳 2 表示已經在 "已登入" 連線狀態  
            if (ret_code == 2)
            {
                textBox_Status.Text += "已登入";
            }


        }//end of login

        // TLinkStatus: 回傳連線狀態, AccList: 回傳帳號, Casq: 憑證序號, Cast: 憑證狀態
        void m_yuanta_ord_OnLogonS(int TLinkStatus, string AccList, string Casq, string Cast)
        {
            //LogMessage(String.Format("OnLogonS: {0},{1},{2},{3}", TLinkStatus, AccList, Casq, Cast));

            if (TLinkStatus == 2)
            {
                this.button_Buy.Enabled = true;
                this.button_Buy.Visible = true;

                this.button_Sell.Enabled = true;
                this.button_Sell.Visible = true;

                textBox_Status.Text += "登入成功!\n";

                return;
            }

            textBox_Status.Text += TLinkStatus.ToString();
        }

        private void button_Buy_Click(object sender, EventArgs e)
        {
            m_yuanta_ord.SetWaitOrdResult(1);//等待委託單回報

            string ret_no = string.Empty;

            ret_no = m_yuanta_ord.SendOrderF(Function_Code_01, Type_Code_0, branchCode, accountCode, Sub_Account, "", BS_Type_B, Trade_Code, tradePrice, lots, Trade_Type_AUTO, Price_Type_M, Order_Cond_I, "", "");

            textBox_Status.Text += "下單結果:" + ret_no + "'\n";

            button_Sell.Visible = false;

            button_Sell.Enabled = false;

        }

        private void button_Sell_Click(object sender, EventArgs e)
        {
            m_yuanta_ord.SetWaitOrdResult(1);//等待委託單回報

            string ret_no = string.Empty;

            ret_no = m_yuanta_ord.SendOrderF(Function_Code_01, Type_Code_0, branchCode, accountCode, Sub_Account, "", BS_Type_S, Trade_Code, tradePrice, lots, Trade_Type_AUTO, Price_Type_M, Order_Cond_I, "", "");

            textBox_Status.Text += "下單結果:" + ret_no + "'\n";

            button_Buy.Visible = false;

            button_Buy.Enabled = false;
        }


        // 即時成交回報
        void m_yuanta_ord_OnOrdMatF(string Omkt, string Buys, string Cmbf, string Bhno, string AcNo, string Suba, string Symb, string Scnam, string O_Kind, string S_Buys, string O_Prc, string A_Prc, string O_Qty, string Deal_Qty, string T_Date, string D_Time, string Order_No, string O_Src, string O_Lin, string Oseq_No)
        {
            string mat_str = String.Format("Omkt={0},Buys={1},Cmbf={2},Bhno={3},Acno={4},Suba={5},Symb={6},Scnam={7},O_Kind={8},S_Buys={9},O_Prc={10},A_Prc={11},O_Qty={12},Deal_Qty={13},T_Date={14},D_Time={15},Order_No={16},O_Src={17},O_Lin={18},Oseq_No={19}"
                                    , Omkt.Trim(), Buys.Trim(), Cmbf.Trim(), Bhno.Trim(), AcNo.Trim(), Suba.Trim(), Symb.Trim(), Scnam.Trim(), O_Kind.Trim(), S_Buys.Trim(), O_Prc.Trim(), A_Prc.Trim(), O_Qty.Trim(), Deal_Qty.Trim(), T_Date.Trim(), D_Time.Trim(), Order_No.Trim(), O_Src.Trim(), O_Lin.Trim(), Oseq_No.Trim());

            textBox_Status.Text = "OnOrdMatF(" + mat_str + ")\n";

            button_Sell.Visible = true;

            button_Buy.Visible = true;

            button_Sell.Enabled = true;

            button_Buy.Enabled = true;

        }

        // 即時委託回報
        void m_yuanta_ord_OnOrdRptF(string Omkt, string Mktt, string Cmbf, string Statusc, string Ts_Code, string Ts_Msg, string Bhno, string AcNo, string Suba, string Symb, string Scnam, string O_Kind, string O_Type, string Buys, string S_Buys, string O_Prc, string O_Qty, string Work_Qty, string Kill_Qty, string Deal_Qty, string Order_No, string T_Date, string O_Date, string O_Time, string O_Src, string O_Lin, string A_Prc, string Oseq_No, string Err_Code, string Err_Msg, string R_Time, string D_Flag)
        {
            string rpt_str = String.Format("Omkt={0},Mktt={1},Cmbf={2},Statusc={3},Ts_Code={4},Ts_Msg={5},Bhno={6},Acno={7},Suba={8},Symb={9},Scnam={10},O_Kind={11},O_Type={12},Buys={13},S_Buys={14},O_Prc={15},O_Qty={16},Work_Qty={17},Kill_Qty={18},Deal_Qty={19},Order_No={20},T_Date={21},O_Date={22},O_Time={23},O_Src={24},O_Lin={25},A_Prc={26},Oseq_No={27},Err_Code={28},Err_Msg={29},R_Time={30},D_Flag={31}"
                                    , Omkt.Trim(), Mktt.Trim(), Cmbf.Trim(), Statusc.Trim(), Ts_Code.Trim(), Ts_Msg.Trim(), Bhno.Trim(), AcNo.Trim(), Suba.Trim(), Symb.Trim(), Scnam.Trim(), O_Kind.Trim(), O_Type.Trim(), Buys.Trim(), S_Buys.Trim(), O_Prc.Trim(), O_Qty.Trim(), Work_Qty.Trim(), Kill_Qty.Trim(), Deal_Qty.Trim(), Order_No.Trim(), T_Date.Trim(), O_Date.Trim(), O_Time.Trim(), O_Src.Trim(), O_Lin.Trim(), A_Prc.Trim(), Oseq_No.Trim(), Err_Code.Trim(), Err_Msg.Trim(), R_Time.Trim(), D_Flag.Trim());

            textBox_Status.Text = "OnOrdRptF(" + rpt_str + ")\n";
        }



    }
}
