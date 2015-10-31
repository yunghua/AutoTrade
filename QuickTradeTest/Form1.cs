using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradeUtility;
using System.Threading;

namespace QuickTradeTest
{
    public partial class Form1 : Form
    {

        Thread workerThread = null;

        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            this.Show();

            TestManager manager = new TestManager();

            manager.prepareTest();
           
            workerThread = new Thread(manager.startTest);            

            // Start the worker thread.
            workerThread.Start();
           
            label_Version.Text = manager.getVersion();

            textBox_lose.Text = manager.getLoseLine();

            textBox_win.Text = manager.getWinLine();

            textBox_reverse.Text = manager.getReverseLine();

            this.Text = manager.getAppDir();

        }

        private void Form1_Close(object sender, EventArgs e)
        {
            workerThread.Abort();

            Console.WriteLine("worker thread: terminated");
        }

    }
}
