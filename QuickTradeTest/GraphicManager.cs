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

        Pen pointPen = new Pen(Color.Black, 1);	//畫筆  

        public GraphicManager(Graphics g)
        {
            m_graphics = g;
        }

        public void setGraphic(Graphics g)
        {
            m_graphics = g;
        }

        public void drawPoint(int x, int y)
        {

            if (m_graphics != null)
            {
                //m_graphics.DrawLine(pointPen, x, y, x, y+3);

                m_graphics.DrawEllipse(pointPen, x, y, 2, 2);
            }

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

        public void drawOrderLine(Pen pen, int x, int y)
        {
            if (m_graphics != null)
            {
                m_graphics.DrawLine(pen, x, y-20, x, y + 20);
            }

        }

    }
}
