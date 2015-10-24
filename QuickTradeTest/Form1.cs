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
        const Boolean isGoPaint = false;

        Thread thread1 = null;

        GraphicManager graphicManager = null;

        Graphics graphicsForeground = null;
        Graphics graphicsBackground = null;

        Bitmap imageForeground = null;
        Bitmap imageBackground = null;
        Bitmap canvas = null;

        const int Picture_Box_Width = 65535;

        const int Picture_Box_Height = 1000;

        const int Form_Width = 1300;

        int x, y;

        public Form1()
        {
            InitializeComponent();

            this.imageBackground = new Bitmap(pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height);
            this.graphicsBackground = Graphics.FromImage(imageBackground);

            graphicManager = new GraphicManager(graphicsBackground);

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

            manager.setGoPaint(isGoPaint);

            manager.startTest();

            manager.stop();

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isGoPaint)
            {
                pictureBox1.CreateGraphics().DrawImageUnscaled(this.imageBackground, 0, 0);
            }
            else
            {
                timer1.Enabled = false;
            }
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

  

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int scrollPosition = e.NewValue;

            int yPictureBoxLocation = 0 - (Picture_Box_Height * scrollPosition / 100);

            pictureBox1.Location = new Point(pictureBox1.Location.X, yPictureBoxLocation);
        }

    }
}
