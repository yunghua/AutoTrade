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

        GraphicManager graphic = null;

        const int Picture_Box_Width = 65535;

        const int Form_Width = 1300;

        int x, y;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            b = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            this.Show();

            graphic = new GraphicManager(pictureBox1, b);

            Thread t1 = new Thread(runTest);

            t1.Start();            

        }

        private void runTest()
        {
            TestManager manager = new TestManager();

            manager.setGraphicManager(graphic);

            manager.startTest();

            manager.stop();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            pictureBox1.Image = b;  
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            //graphic.drawPoint(x, y);            
            this.Refresh();
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int  scrollPosition = e.NewValue;

            int xPictureBoxLocation= 0-(Picture_Box_Width * scrollPosition / 100);

            pictureBox1.Location = new Point(xPictureBoxLocation, pictureBox1.Location.Y);
                
        }

       
    }
}
