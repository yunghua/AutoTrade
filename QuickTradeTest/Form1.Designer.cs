namespace QuickTradeTest
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
            this.label_Version = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_lose = new System.Windows.Forms.TextBox();
            this.textBox_win = new System.Windows.Forms.TextBox();
            this.textBox_reverse = new System.Windows.Forms.TextBox();
            this.label_comment = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label_Version
            // 
            this.label_Version.AutoSize = true;
            this.label_Version.Font = new System.Drawing.Font("Microsoft JhengHei", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_Version.Location = new System.Drawing.Point(25, 22);
            this.label_Version.Name = "label_Version";
            this.label_Version.Size = new System.Drawing.Size(263, 81);
            this.label_Version.TabIndex = 0;
            this.label_Version.Text = "Version";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft JhengHei", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(28, 103);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(222, 61);
            this.label1.TabIndex = 1;
            this.label1.Text = "LoseLine";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft JhengHei", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(28, 164);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(208, 61);
            this.label2.TabIndex = 2;
            this.label2.Text = "WinLine";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft JhengHei", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(28, 225);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(296, 61);
            this.label3.TabIndex = 3;
            this.label3.Text = "ReverseLine";
            // 
            // textBox_lose
            // 
            this.textBox_lose.Font = new System.Drawing.Font("Microsoft JhengHei", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox_lose.Location = new System.Drawing.Point(363, 115);
            this.textBox_lose.Name = "textBox_lose";
            this.textBox_lose.Size = new System.Drawing.Size(271, 35);
            this.textBox_lose.TabIndex = 4;
            // 
            // textBox_win
            // 
            this.textBox_win.Font = new System.Drawing.Font("Microsoft JhengHei", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox_win.Location = new System.Drawing.Point(363, 164);
            this.textBox_win.Name = "textBox_win";
            this.textBox_win.Size = new System.Drawing.Size(271, 35);
            this.textBox_win.TabIndex = 5;
            // 
            // textBox_reverse
            // 
            this.textBox_reverse.Font = new System.Drawing.Font("Microsoft JhengHei", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox_reverse.Location = new System.Drawing.Point(363, 225);
            this.textBox_reverse.Name = "textBox_reverse";
            this.textBox_reverse.Size = new System.Drawing.Size(271, 35);
            this.textBox_reverse.TabIndex = 6;
            // 
            // label_comment
            // 
            this.label_comment.AutoSize = true;
            this.label_comment.Font = new System.Drawing.Font("Microsoft JhengHei", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_comment.Location = new System.Drawing.Point(42, 301);
            this.label_comment.Name = "label_comment";
            this.label_comment.Size = new System.Drawing.Size(162, 40);
            this.label_comment.TabIndex = 7;
            this.label_comment.Text = "comment";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(687, 429);
            this.Controls.Add(this.label_comment);
            this.Controls.Add(this.textBox_reverse);
            this.Controls.Add(this.textBox_win);
            this.Controls.Add(this.textBox_lose);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label_Version);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);            
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_Close);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_Version;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_lose;
        private System.Windows.Forms.TextBox textBox_win;
        private System.Windows.Forms.TextBox textBox_reverse;
        private System.Windows.Forms.Label label_comment;

    }
}

