using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace TradeUtility
{
    class GraphicManager
    {
        Graphics m_graphics = null;      

        Pen pointPen =  new Pen(Color.Black, 1);	//畫筆

        PictureBox pictureBox1 = null;

        Bitmap b = null;

        public GraphicManager(System.Windows.Forms.PictureBox pictureBox1,Bitmap b)
        {

            //Graphics m_graphics = pictureBox1.CreateGraphics();

            this.pictureBox1 = pictureBox1;

            m_graphics = Graphics.FromImage(b);

            m_graphics.Clear(Color.White);
            
        }

        public void drawPoint(int x, int y)
        {            

            if (m_graphics != null)
            {
                m_graphics.DrawLine(pointPen, x, y, x, y+1);
            }

            pictureBox1.Image = b;

            pictureBox1.Invalidate();
        }

        public void drawBuyLine(int x, int y)
        {
            Pen pen = new Pen(Color.Red, 3);	//畫筆
            drawOrderLine(pen, x, y);
        }

        public void drawSellLine(int x, int y)
        {
            Pen pen = new Pen(Color.Green, 3);	//畫筆
            drawOrderLine(pen, x, y);
        }

        public void drawOrderLine(Pen pen ,int x, int y)
        {
            if (m_graphics != null)
            {                
                m_graphics.DrawLine(pen, x, y, x+1, y + 20);
            }

            pictureBox1.Image = b; 
        }

    }
}
