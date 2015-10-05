namespace OrderTest
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_Buy = new System.Windows.Forms.Button();
            this.button_Sell = new System.Windows.Forms.Button();
            this.textBox_Status = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button_Buy
            // 
            this.button_Buy.BackColor = System.Drawing.Color.Red;
            this.button_Buy.Font = new System.Drawing.Font("Microsoft JhengHei", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button_Buy.Location = new System.Drawing.Point(138, 201);
            this.button_Buy.Name = "button_Buy";
            this.button_Buy.Size = new System.Drawing.Size(135, 80);
            this.button_Buy.TabIndex = 0;
            this.button_Buy.Text = "Buy";
            this.button_Buy.UseVisualStyleBackColor = false;
            this.button_Buy.Click += new System.EventHandler(this.button_Buy_Click);
            // 
            // button_Sell
            // 
            this.button_Sell.BackColor = System.Drawing.Color.Lime;
            this.button_Sell.Font = new System.Drawing.Font("Microsoft JhengHei", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button_Sell.Location = new System.Drawing.Point(453, 201);
            this.button_Sell.Name = "button_Sell";
            this.button_Sell.Size = new System.Drawing.Size(135, 80);
            this.button_Sell.TabIndex = 1;
            this.button_Sell.Text = "Sell";
            this.button_Sell.UseVisualStyleBackColor = false;
            this.button_Sell.Click += new System.EventHandler(this.button_Sell_Click);
            // 
            // textBox_Status
            // 
            this.textBox_Status.Font = new System.Drawing.Font("Microsoft JhengHei", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox_Status.Location = new System.Drawing.Point(138, 41);
            this.textBox_Status.Multiline = true;
            this.textBox_Status.Name = "textBox_Status";
            this.textBox_Status.Size = new System.Drawing.Size(450, 107);
            this.textBox_Status.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 358);
            this.Controls.Add(this.textBox_Status);
            this.Controls.Add(this.button_Sell);
            this.Controls.Add(this.button_Buy);
            this.Name = "Form1";
            this.Text = "Order Test";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Buy;
        private System.Windows.Forms.Button button_Sell;
        private System.Windows.Forms.TextBox textBox_Status;
    }
}

