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
        Thread thread1 = null;

        GraphicManager graphicManager = null;

        Graphics graphicsForeground = null;
        Graphics graphicsBackground = null;

        Bitmap imageForeground = null;
        Bitmap imageBackground = null;
        Bitmap canvas = null;

        const int Picture_Box_Width = 65535;

        const int Form_Width = 1300;

        int x, y;

        public Form1()
        {
            InitializeComponent();

            // 初始化前景畫板
            imageForeground = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            // 初始化背景畫板
            canvas = new Bitmap(pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height);

            // 設置為背景層
            pictureBox1.BackgroundImage = canvas;

            // 獲取背景層
            imageBackground = (Bitmap)pictureBox1.BackgroundImage;

            // 初始化圖形面板
            graphicsForeground = Graphics.FromImage(imageForeground);
            graphicsBackground = Graphics.FromImage(canvas);

            graphicsForeground.Clear(Color.White);

            graphicManager = new GraphicManager(graphicsForeground);

            //timer1.Enabled = false;

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            this.Show();

             thread1 = new Thread(runTest);

            thread1.Start();

        }

        private void runTest()
        {
            TestManager manager = new TestManager();

            manager.setGraphicManager(graphicManager);

            manager.startTest();

            manager.stop();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            graphicsBackground.DrawImage(imageBackground, 0, 0); // 先繪製背景層
            graphicsBackground.DrawImage(imageForeground, 0, 0); // 再繪製繪畫層

            pictureBox1.Refresh();
            pictureBox1.CreateGraphics().DrawImage(canvas, 0, 0);
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int scrollPosition = e.NewValue;

            int xPictureBoxLocation = 0 - (Picture_Box_Width * scrollPosition / 100);

            pictureBox1.Location = new Point(xPictureBoxLocation, pictureBox1.Location.Y);

        }


        private void Form1_Close(object sender, EventArgs e)
        {

            if (thread1 != null && thread1.IsAlive)
            {                
                thread1.Abort();
            }
         

        }

    }
}
