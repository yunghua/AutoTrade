using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradeUtility;


namespace AutoTrade
{
    public partial class Form1 : Form
    {
        const String configFilePath = "./Config/TradeConfig.txt";

        ConfigFile configFile;

        TradeMaster master;

        string ip = "";//IP
        string port = "";//port=80,數字

        string id = "";
        string password = "";

        string tradeCode = "";//市場別代碼，小台指=MXF，大台指=TXF

        public Form1()
        {
            InitializeComponent();


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



        private void LoginFn()
        {
            try
            {
                axYuantaQuote1.SetMktLogon(id, password, ip, port);
                //axYuantaQuote1.SetMktLogon(textBox_id.Text.Trim(), textBox_pass.Text.Trim(), textBox_ip.Text.Trim(), textBox_port.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show("SetMktConnection失敗：" + ex.Message);

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

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            label_Version.Text = TradeUtility.TradeUtility.version;


            configFile = new ConfigFile(configFilePath);
            try
            {
                configFile.prepareReader();
            }
            catch (Exception ex)
            {
                MessageBox.Show("讀取  設定檔  失敗。原因 : " + ex.Message);
            }

            tradeCode = configFile.readConfig("Trade_Code");

            tradeCode = TradeUtility.TradeUtility.getInstance().dealTradeCode(tradeCode);

            id = configFile.readConfig("ID");

            password = configFile.readConfig("Password");

            ip = configFile.readConfig("IP");

            port = configFile.readConfig("Port");

            master = new TradeMaster();
           
            try
            {

                master.prepareReady();

            }
            catch (Exception ez)
            {
                MessageBox.Show(ez.Message+"--"+ez.StackTrace);

                return;
            }


            LoginFn();

            

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
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            LoginFn();
        }




        private void textBox_sym_Click(object sender, EventArgs e)
        {
            textBox_sym.SelectAll();
        }



    }
}
