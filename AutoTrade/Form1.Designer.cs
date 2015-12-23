namespace AutoTrade
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {



            if (disposing && (components != null))
            {
                Form1_Close();

                components.Dispose();                
            }
            base.Dispose(disposing);

        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.axYuantaQuote1 = new AxYuantaQuoteLib.AxYuantaQuote();
            this.checkBox_enableTrade = new System.Windows.Forms.CheckBox();
            this.textBox_reverseLine = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_loseLine = new System.Windows.Forms.TextBox();
            this.textBox1_winLine = new System.Windows.Forms.TextBox();
            this.label_loseLine = new System.Windows.Forms.Label();
            this.label_winLine = new System.Windows.Forms.Label();
            this.textBox_status = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_status_order = new System.Windows.Forms.TextBox();
            this.label_Version = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_status2 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_sym = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_status_ready = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBox_OrderStart = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBox_tradeCodeLastDay = new System.Windows.Forms.TextBox();
            this.radioButton_lose_not_reverse = new System.Windows.Forms.RadioButton();
            this.radioButton_lose_reverse = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton_win_reverse = new System.Windows.Forms.RadioButton();
            this.radioButton_win_not_reverse = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.key = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RefPri = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OpenPri = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HighPri = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LowPri = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UpPri = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DnPri = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MatchTim = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MatchPri = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MatchQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TolMatchQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_B_S = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox_MaxPrice = new System.Windows.Forms.TextBox();
            this.textBox_MinPrice = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBox_OrderPrice = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox_OrderNewPriceList = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBox_Stage = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.textBox_NowTradeType = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.textBox_OrderPriceNew = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.textBox_TradePrice = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label18 = new System.Windows.Forms.Label();
            this.comboBox_initial_direction = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label19 = new System.Windows.Forms.Label();
            this.textBox_StopPrice = new System.Windows.Forms.TextBox();
            this.textBox_PrevStopPrice = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.textBox_ContinueLoseTimes = new System.Windows.Forms.TextBox();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axYuantaQuote1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.panel2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.AutoSize = true;
            this.groupBox3.Controls.Add(this.axYuantaQuote1);
            this.groupBox3.Controls.Add(this.checkBox_enableTrade);
            this.groupBox3.Controls.Add(this.textBox_reverseLine);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.textBox_loseLine);
            this.groupBox3.Controls.Add(this.textBox1_winLine);
            this.groupBox3.Controls.Add(this.label_loseLine);
            this.groupBox3.Controls.Add(this.label_winLine);
            this.groupBox3.Controls.Add(this.textBox_status);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.textBox_status_order);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(5);
            this.groupBox3.Size = new System.Drawing.Size(418, 270);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Yuanta Quote API 連線資訊";
            // 
            // axYuantaQuote1
            // 
            this.axYuantaQuote1.Enabled = true;
            this.axYuantaQuote1.Location = new System.Drawing.Point(289, 83);
            this.axYuantaQuote1.Name = "axYuantaQuote1";
            this.axYuantaQuote1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axYuantaQuote1.OcxState")));
            this.axYuantaQuote1.Size = new System.Drawing.Size(100, 50);
            this.axYuantaQuote1.TabIndex = 23;
            this.axYuantaQuote1.OnGetMktAll += new AxYuantaQuoteLib._DYuantaQuoteEvents_OnGetMktAllEventHandler(this.axYuantaQuote1_OnGetMktAll);
            this.axYuantaQuote1.OnMktStatusChange += new AxYuantaQuoteLib._DYuantaQuoteEvents_OnMktStatusChangeEventHandler(this.axYuantaQuote1_OnMktStatusChange);
            this.axYuantaQuote1.OnRegError += new AxYuantaQuoteLib._DYuantaQuoteEvents_OnRegErrorEventHandler(this.axYuantaQuote1_OnRegError);
            // 
            // checkBox_enableTrade
            // 
            this.checkBox_enableTrade.AutoSize = true;
            this.checkBox_enableTrade.Location = new System.Drawing.Point(289, 36);
            this.checkBox_enableTrade.Name = "checkBox_enableTrade";
            this.checkBox_enableTrade.Size = new System.Drawing.Size(92, 24);
            this.checkBox_enableTrade.TabIndex = 27;
            this.checkBox_enableTrade.Text = "執行交易";
            this.checkBox_enableTrade.UseVisualStyleBackColor = true;
            this.checkBox_enableTrade.CheckedChanged += new System.EventHandler(this.checkBox_enableTrade_CheckedChanged);
            // 
            // textBox_reverseLine
            // 
            this.textBox_reverseLine.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox_reverseLine.Location = new System.Drawing.Point(175, 104);
            this.textBox_reverseLine.Name = "textBox_reverseLine";
            this.textBox_reverseLine.Size = new System.Drawing.Size(82, 29);
            this.textBox_reverseLine.TabIndex = 26;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(84, 107);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 20);
            this.label3.TabIndex = 25;
            this.label3.Text = "反轉範圍：";
            // 
            // textBox_loseLine
            // 
            this.textBox_loseLine.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox_loseLine.Location = new System.Drawing.Point(175, 31);
            this.textBox_loseLine.Name = "textBox_loseLine";
            this.textBox_loseLine.Size = new System.Drawing.Size(82, 29);
            this.textBox_loseLine.TabIndex = 24;
            // 
            // textBox1_winLine
            // 
            this.textBox1_winLine.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox1_winLine.Location = new System.Drawing.Point(175, 67);
            this.textBox1_winLine.Name = "textBox1_winLine";
            this.textBox1_winLine.Size = new System.Drawing.Size(82, 29);
            this.textBox1_winLine.TabIndex = 23;
            // 
            // label_loseLine
            // 
            this.label_loseLine.AutoSize = true;
            this.label_loseLine.Location = new System.Drawing.Point(84, 34);
            this.label_loseLine.Name = "label_loseLine";
            this.label_loseLine.Size = new System.Drawing.Size(89, 20);
            this.label_loseLine.TabIndex = 22;
            this.label_loseLine.Text = "停損範圍：";
            // 
            // label_winLine
            // 
            this.label_winLine.AutoSize = true;
            this.label_winLine.Location = new System.Drawing.Point(84, 70);
            this.label_winLine.Name = "label_winLine";
            this.label_winLine.Size = new System.Drawing.Size(89, 20);
            this.label_winLine.TabIndex = 21;
            this.label_winLine.Text = "停利範圍：";
            // 
            // textBox_status
            // 
            this.textBox_status.Location = new System.Drawing.Point(114, 168);
            this.textBox_status.Margin = new System.Windows.Forms.Padding(5);
            this.textBox_status.Name = "textBox_status";
            this.textBox_status.Size = new System.Drawing.Size(294, 29);
            this.textBox_status.TabIndex = 19;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 177);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 20);
            this.label4.TabIndex = 18;
            this.label4.Text = "Quote狀態";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 223);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 20);
            this.label1.TabIndex = 25;
            this.label1.Text = "Order狀態";
            // 
            // textBox_status_order
            // 
            this.textBox_status_order.Location = new System.Drawing.Point(114, 214);
            this.textBox_status_order.Margin = new System.Windows.Forms.Padding(5);
            this.textBox_status_order.Name = "textBox_status_order";
            this.textBox_status_order.Size = new System.Drawing.Size(294, 29);
            this.textBox_status_order.TabIndex = 25;
            // 
            // label_Version
            // 
            this.label_Version.AutoSize = true;
            this.label_Version.Location = new System.Drawing.Point(183, 721);
            this.label_Version.Name = "label_Version";
            this.label_Version.Size = new System.Drawing.Size(105, 20);
            this.label_Version.TabIndex = 20;
            this.label_Version.Text = "公用元件版本";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_status2);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.textBox_sym);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox_status_ready);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.textBox_OrderStart);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.textBox_tradeCodeLastDay);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.Location = new System.Drawing.Point(418, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(5);
            this.groupBox1.Size = new System.Drawing.Size(469, 270);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Yuanta Quote API 商品訂閱資訊";
            // 
            // textBox_status2
            // 
            this.textBox_status2.Location = new System.Drawing.Point(126, 166);
            this.textBox_status2.Margin = new System.Windows.Forms.Padding(5);
            this.textBox_status2.Name = "textBox_status2";
            this.textBox_status2.Size = new System.Drawing.Size(310, 29);
            this.textBox_status2.TabIndex = 21;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 171);
            this.label7.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 20);
            this.label7.TabIndex = 20;
            this.label7.Text = "狀態";
            // 
            // textBox_sym
            // 
            this.textBox_sym.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_sym.Location = new System.Drawing.Point(203, 25);
            this.textBox_sym.Margin = new System.Windows.Forms.Padding(5);
            this.textBox_sym.Name = "textBox_sym";
            this.textBox_sym.Size = new System.Drawing.Size(217, 29);
            this.textBox_sym.TabIndex = 6;
            this.textBox_sym.Click += new System.EventHandler(this.textBox_sym_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(120, 34);
            this.label6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(73, 20);
            this.label6.TabIndex = 11;
            this.label6.Text = "商品代碼";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 214);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 20);
            this.label2.TabIndex = 26;
            this.label2.Text = "Ready:";
            // 
            // textBox_status_ready
            // 
            this.textBox_status_ready.Location = new System.Drawing.Point(125, 205);
            this.textBox_status_ready.Margin = new System.Windows.Forms.Padding(5);
            this.textBox_status_ready.Name = "textBox_status_ready";
            this.textBox_status_ready.Size = new System.Drawing.Size(311, 29);
            this.textBox_status_ready.TabIndex = 24;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(100, 113);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(93, 20);
            this.label12.TabIndex = 38;
            this.label12.Text = "Order Start";
            // 
            // textBox_OrderStart
            // 
            this.textBox_OrderStart.Location = new System.Drawing.Point(203, 107);
            this.textBox_OrderStart.Margin = new System.Windows.Forms.Padding(5);
            this.textBox_OrderStart.Name = "textBox_OrderStart";
            this.textBox_OrderStart.Size = new System.Drawing.Size(217, 29);
            this.textBox_OrderStart.TabIndex = 37;
            this.textBox_OrderStart.Text = "false";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 70);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(185, 20);
            this.label13.TabIndex = 39;
            this.label13.Text = "上一個交易日的商品代碼";
            // 
            // textBox_tradeCodeLastDay
            // 
            this.textBox_tradeCodeLastDay.Location = new System.Drawing.Point(203, 67);
            this.textBox_tradeCodeLastDay.Margin = new System.Windows.Forms.Padding(5);
            this.textBox_tradeCodeLastDay.Name = "textBox_tradeCodeLastDay";
            this.textBox_tradeCodeLastDay.Size = new System.Drawing.Size(217, 29);
            this.textBox_tradeCodeLastDay.TabIndex = 40;
            // 
            // radioButton_lose_not_reverse
            // 
            this.radioButton_lose_not_reverse.AutoSize = true;
            this.radioButton_lose_not_reverse.Checked = true;
            this.radioButton_lose_not_reverse.Location = new System.Drawing.Point(138, 28);
            this.radioButton_lose_not_reverse.Name = "radioButton_lose_not_reverse";
            this.radioButton_lose_not_reverse.Size = new System.Drawing.Size(75, 24);
            this.radioButton_lose_not_reverse.TabIndex = 27;
            this.radioButton_lose_not_reverse.TabStop = true;
            this.radioButton_lose_not_reverse.Text = "不反轉";
            this.radioButton_lose_not_reverse.UseVisualStyleBackColor = true;
            this.radioButton_lose_not_reverse.CheckedChanged += new System.EventHandler(this.radioButton_lose_not_reverse_CheckedChanged);
            // 
            // radioButton_lose_reverse
            // 
            this.radioButton_lose_reverse.AutoSize = true;
            this.radioButton_lose_reverse.Location = new System.Drawing.Point(55, 28);
            this.radioButton_lose_reverse.Name = "radioButton_lose_reverse";
            this.radioButton_lose_reverse.Size = new System.Drawing.Size(59, 24);
            this.radioButton_lose_reverse.TabIndex = 25;
            this.radioButton_lose_reverse.Text = "反轉";
            this.radioButton_lose_reverse.UseVisualStyleBackColor = true;
            this.radioButton_lose_reverse.CheckedChanged += new System.EventHandler(this.radioButton_lose_reverse_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton_win_reverse);
            this.groupBox2.Controls.Add(this.radioButton_win_not_reverse);
            this.groupBox2.Location = new System.Drawing.Point(11, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(222, 67);
            this.groupBox2.TabIndex = 28;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "賺錢";
            // 
            // radioButton_win_reverse
            // 
            this.radioButton_win_reverse.AutoSize = true;
            this.radioButton_win_reverse.Location = new System.Drawing.Point(29, 28);
            this.radioButton_win_reverse.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.radioButton_win_reverse.Name = "radioButton_win_reverse";
            this.radioButton_win_reverse.Size = new System.Drawing.Size(59, 24);
            this.radioButton_win_reverse.TabIndex = 24;
            this.radioButton_win_reverse.Text = "反轉";
            this.radioButton_win_reverse.UseVisualStyleBackColor = true;
            this.radioButton_win_reverse.CheckedChanged += new System.EventHandler(this.radioButton_win_reverse_CheckedChanged);
            // 
            // radioButton_win_not_reverse
            // 
            this.radioButton_win_not_reverse.AutoSize = true;
            this.radioButton_win_not_reverse.Checked = true;
            this.radioButton_win_not_reverse.Location = new System.Drawing.Point(108, 28);
            this.radioButton_win_not_reverse.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.radioButton_win_not_reverse.Name = "radioButton_win_not_reverse";
            this.radioButton_win_not_reverse.Size = new System.Drawing.Size(75, 24);
            this.radioButton_win_not_reverse.TabIndex = 26;
            this.radioButton_win_not_reverse.TabStop = true;
            this.radioButton_win_not_reverse.Text = "不反轉";
            this.radioButton_win_not_reverse.UseVisualStyleBackColor = true;
            this.radioButton_win_not_reverse.CheckedChanged += new System.EventHandler(this.radioButton_win_not_reverse_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(891, 270);
            this.panel1.TabIndex = 18;
            // 
            // key
            // 
            this.key.DataPropertyName = "商品代碼";
            this.key.Frozen = true;
            this.key.HeaderText = "商品代碼";
            this.key.Name = "key";
            this.key.ReadOnly = true;
            this.key.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // RefPri
            // 
            this.RefPri.Name = "RefPri";
            // 
            // OpenPri
            // 
            this.OpenPri.Name = "OpenPri";
            // 
            // HighPri
            // 
            this.HighPri.Name = "HighPri";
            // 
            // LowPri
            // 
            this.LowPri.Name = "LowPri";
            // 
            // UpPri
            // 
            this.UpPri.Name = "UpPri";
            // 
            // DnPri
            // 
            this.DnPri.Name = "DnPri";
            // 
            // MatchTim
            // 
            this.MatchTim.Name = "MatchTim";
            // 
            // MatchPri
            // 
            this.MatchPri.Name = "MatchPri";
            // 
            // MatchQty
            // 
            this.MatchQty.Name = "MatchQty";
            // 
            // TolMatchQty
            // 
            this.TolMatchQty.Name = "TolMatchQty";
            // 
            // timer1
            // 
            this.timer1.Interval = 5000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Interval = 5000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label5.Location = new System.Drawing.Point(18, 382);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(153, 20);
            this.label5.TabIndex = 27;
            this.label5.Text = "上一交易日買賣方向";
            // 
            // textBox_B_S
            // 
            this.textBox_B_S.Location = new System.Drawing.Point(200, 379);
            this.textBox_B_S.Margin = new System.Windows.Forms.Padding(5);
            this.textBox_B_S.Name = "textBox_B_S";
            this.textBox_B_S.Size = new System.Drawing.Size(208, 29);
            this.textBox_B_S.TabIndex = 28;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label8.Location = new System.Drawing.Point(21, 471);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(82, 20);
            this.label8.TabIndex = 29;
            this.label8.Text = "Max Price";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label9.Location = new System.Drawing.Point(21, 516);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(79, 20);
            this.label9.TabIndex = 30;
            this.label9.Text = "Min Price";
            // 
            // textBox_MaxPrice
            // 
            this.textBox_MaxPrice.Location = new System.Drawing.Point(115, 468);
            this.textBox_MaxPrice.Margin = new System.Windows.Forms.Padding(5);
            this.textBox_MaxPrice.Name = "textBox_MaxPrice";
            this.textBox_MaxPrice.Size = new System.Drawing.Size(294, 29);
            this.textBox_MaxPrice.TabIndex = 31;
            // 
            // textBox_MinPrice
            // 
            this.textBox_MinPrice.Location = new System.Drawing.Point(114, 513);
            this.textBox_MinPrice.Margin = new System.Windows.Forms.Padding(5);
            this.textBox_MinPrice.Name = "textBox_MinPrice";
            this.textBox_MinPrice.Size = new System.Drawing.Size(294, 29);
            this.textBox_MinPrice.TabIndex = 32;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label10.Location = new System.Drawing.Point(443, 382);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(128, 20);
            this.label10.TabIndex = 33;
            this.label10.Text = "Order Price Last";
            // 
            // textBox_OrderPrice
            // 
            this.textBox_OrderPrice.Location = new System.Drawing.Point(580, 379);
            this.textBox_OrderPrice.Margin = new System.Windows.Forms.Padding(5);
            this.textBox_OrderPrice.Name = "textBox_OrderPrice";
            this.textBox_OrderPrice.Size = new System.Drawing.Size(294, 29);
            this.textBox_OrderPrice.TabIndex = 34;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label11.Location = new System.Drawing.Point(10, 662);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(127, 20);
            this.label11.TabIndex = 35;
            this.label11.Text = "Order Price List ";
            // 
            // textBox_OrderNewPriceList
            // 
            this.textBox_OrderNewPriceList.Location = new System.Drawing.Point(145, 552);
            this.textBox_OrderNewPriceList.Margin = new System.Windows.Forms.Padding(5);
            this.textBox_OrderNewPriceList.Multiline = true;
            this.textBox_OrderNewPriceList.Name = "textBox_OrderNewPriceList";
            this.textBox_OrderNewPriceList.Size = new System.Drawing.Size(263, 164);
            this.textBox_OrderNewPriceList.TabIndex = 36;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(476, 516);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(52, 20);
            this.label14.TabIndex = 41;
            this.label14.Text = "Stage";
            // 
            // textBox_Stage
            // 
            this.textBox_Stage.Location = new System.Drawing.Point(580, 513);
            this.textBox_Stage.Margin = new System.Windows.Forms.Padding(5);
            this.textBox_Stage.Name = "textBox_Stage";
            this.textBox_Stage.Size = new System.Drawing.Size(294, 29);
            this.textBox_Stage.TabIndex = 42;
            this.textBox_Stage.Text = "None";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label15.Location = new System.Drawing.Point(68, 429);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(105, 20);
            this.label15.TabIndex = 43;
            this.label15.Text = "目前買賣方向";
            // 
            // textBox_NowTradeType
            // 
            this.textBox_NowTradeType.Location = new System.Drawing.Point(199, 423);
            this.textBox_NowTradeType.Margin = new System.Windows.Forms.Padding(5);
            this.textBox_NowTradeType.Name = "textBox_NowTradeType";
            this.textBox_NowTradeType.Size = new System.Drawing.Size(209, 29);
            this.textBox_NowTradeType.TabIndex = 44;
            this.textBox_NowTradeType.Text = "None";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label16.Location = new System.Drawing.Point(439, 426);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(132, 20);
            this.label16.TabIndex = 45;
            this.label16.Text = "Order Price New";
            // 
            // textBox_OrderPriceNew
            // 
            this.textBox_OrderPriceNew.Location = new System.Drawing.Point(580, 423);
            this.textBox_OrderPriceNew.Margin = new System.Windows.Forms.Padding(5);
            this.textBox_OrderPriceNew.Name = "textBox_OrderPriceNew";
            this.textBox_OrderPriceNew.Size = new System.Drawing.Size(294, 29);
            this.textBox_OrderPriceNew.TabIndex = 46;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label17.Location = new System.Drawing.Point(465, 471);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(73, 20);
            this.label17.TabIndex = 47;
            this.label17.Text = "目前行情";
            // 
            // textBox_TradePrice
            // 
            this.textBox_TradePrice.ForeColor = System.Drawing.Color.Red;
            this.textBox_TradePrice.Location = new System.Drawing.Point(580, 468);
            this.textBox_TradePrice.Margin = new System.Windows.Forms.Padding(5);
            this.textBox_TradePrice.Name = "textBox_TradePrice";
            this.textBox_TradePrice.Size = new System.Drawing.Size(294, 29);
            this.textBox_TradePrice.TabIndex = 48;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label18);
            this.panel2.Controls.Add(this.comboBox_initial_direction);
            this.panel2.Controls.Add(this.groupBox4);
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Location = new System.Drawing.Point(14, 278);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(877, 87);
            this.panel2.TabIndex = 49;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(562, 31);
            this.label18.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(105, 20);
            this.label18.TabIndex = 31;
            this.label18.Text = "初始買賣方向";
            // 
            // comboBox_initial_direction
            // 
            this.comboBox_initial_direction.FormattingEnabled = true;
            this.comboBox_initial_direction.Items.AddRange(new object[] {
            "不指定",
            "BUY",
            "SELL"});
            this.comboBox_initial_direction.Location = new System.Drawing.Point(698, 31);
            this.comboBox_initial_direction.Name = "comboBox_initial_direction";
            this.comboBox_initial_direction.Size = new System.Drawing.Size(132, 28);
            this.comboBox_initial_direction.TabIndex = 30;
            this.comboBox_initial_direction.SelectedIndexChanged += new System.EventHandler(this.comboBox_initial_direction_SelectedIndexChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioButton_lose_not_reverse);
            this.groupBox4.Controls.Add(this.radioButton_lose_reverse);
            this.groupBox4.Location = new System.Drawing.Point(270, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(234, 67);
            this.groupBox4.TabIndex = 29;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "賠錢";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label19.Location = new System.Drawing.Point(486, 628);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(85, 20);
            this.label19.TabIndex = 50;
            this.label19.Text = "Stop Price";
            // 
            // textBox_StopPrice
            // 
            this.textBox_StopPrice.Location = new System.Drawing.Point(583, 619);
            this.textBox_StopPrice.Margin = new System.Windows.Forms.Padding(5);
            this.textBox_StopPrice.Name = "textBox_StopPrice";
            this.textBox_StopPrice.Size = new System.Drawing.Size(294, 29);
            this.textBox_StopPrice.TabIndex = 51;
            this.textBox_StopPrice.TextChanged += new System.EventHandler(this.textBox_StopPrice_TextChanged);
            // 
            // textBox_PrevStopPrice
            // 
            this.textBox_PrevStopPrice.Location = new System.Drawing.Point(580, 568);
            this.textBox_PrevStopPrice.Margin = new System.Windows.Forms.Padding(5);
            this.textBox_PrevStopPrice.Name = "textBox_PrevStopPrice";
            this.textBox_PrevStopPrice.Size = new System.Drawing.Size(294, 29);
            this.textBox_PrevStopPrice.TabIndex = 52;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label20.Location = new System.Drawing.Point(450, 571);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(122, 20);
            this.label20.TabIndex = 53;
            this.label20.Text = "Prev Stop Price";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label21.Location = new System.Drawing.Point(414, 672);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(157, 20);
            this.label21.TabIndex = 54;
            this.label21.Text = "ContinueLoseTimes";
            // 
            // textBox_ContinueLoseTimes
            // 
            this.textBox_ContinueLoseTimes.Location = new System.Drawing.Point(583, 672);
            this.textBox_ContinueLoseTimes.Margin = new System.Windows.Forms.Padding(5);
            this.textBox_ContinueLoseTimes.Name = "textBox_ContinueLoseTimes";
            this.textBox_ContinueLoseTimes.Size = new System.Drawing.Size(294, 29);
            this.textBox_ContinueLoseTimes.TabIndex = 55;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(891, 750);
            this.Controls.Add(this.textBox_ContinueLoseTimes);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.textBox_PrevStopPrice);
            this.Controls.Add(this.textBox_StopPrice);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.textBox_TradePrice);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.textBox_OrderPriceNew);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.textBox_NowTradeType);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.textBox_Stage);
            this.Controls.Add(this.label_Version);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.textBox_OrderNewPriceList);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.textBox_OrderPrice);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.textBox_MinPrice);
            this.Controls.Add(this.textBox_MaxPrice);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBox_B_S);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft JhengHei", 12F);
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " # # # # # 自動下單機V1.12.6  # # # # #     逐層反轉機制";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axYuantaQuote1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBox_status;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox_status2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox_sym;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel1;
        //private System.Windows.Forms.DataGridView DataGrid;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.DataGridViewTextBoxColumn key;
        private System.Windows.Forms.DataGridViewTextBoxColumn RefPri;
        private System.Windows.Forms.DataGridViewTextBoxColumn OpenPri;
        private System.Windows.Forms.DataGridViewTextBoxColumn HighPri;
        private System.Windows.Forms.DataGridViewTextBoxColumn LowPri;
        private System.Windows.Forms.DataGridViewTextBoxColumn UpPri;
        private System.Windows.Forms.DataGridViewTextBoxColumn DnPri;
        private System.Windows.Forms.DataGridViewTextBoxColumn MatchTim;
        private System.Windows.Forms.DataGridViewTextBoxColumn MatchPri;
        private System.Windows.Forms.DataGridViewTextBoxColumn MatchQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn TolMatchQty;
        private AxYuantaQuoteLib.AxYuantaQuote axYuantaQuote1;        
        private System.Windows.Forms.Label label_Version;
        private System.Windows.Forms.TextBox textBox_loseLine;
        private System.Windows.Forms.TextBox textBox1_winLine;
        private System.Windows.Forms.Label label_loseLine;
        private System.Windows.Forms.Label label_winLine;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_status_order;
        private System.Windows.Forms.TextBox textBox_status_ready;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_reverseLine;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBox_enableTrade;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.TextBox textBox_B_S;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        public System.Windows.Forms.TextBox textBox_MaxPrice;
        public System.Windows.Forms.TextBox textBox_MinPrice;
        private System.Windows.Forms.Label label10;
        public System.Windows.Forms.TextBox textBox_OrderPrice;
        private System.Windows.Forms.Label label11;
        public System.Windows.Forms.TextBox textBox_OrderNewPriceList;
        public System.Windows.Forms.TextBox textBox_OrderStart;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        public System.Windows.Forms.TextBox textBox_tradeCodeLastDay;
        private System.Windows.Forms.Label label14;
        public System.Windows.Forms.TextBox textBox_Stage;
        private System.Windows.Forms.Label label15;
        public System.Windows.Forms.TextBox textBox_NowTradeType;
        private System.Windows.Forms.Label label16;
        public System.Windows.Forms.TextBox textBox_OrderPriceNew;
        private System.Windows.Forms.Label label17;
        public System.Windows.Forms.TextBox textBox_TradePrice;
        private System.Windows.Forms.RadioButton radioButton_win_reverse;
        private System.Windows.Forms.RadioButton radioButton_lose_not_reverse;
        private System.Windows.Forms.RadioButton radioButton_win_not_reverse;
        private System.Windows.Forms.RadioButton radioButton_lose_reverse;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox comboBox_initial_direction;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        public System.Windows.Forms.TextBox textBox_StopPrice;
        public System.Windows.Forms.TextBox textBox_PrevStopPrice;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        public System.Windows.Forms.TextBox textBox_ContinueLoseTimes;
    }
}

