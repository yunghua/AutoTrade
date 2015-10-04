using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeUtility
{
    public class KBar
    {

        public enum PriceType { OPEN, HIGH, LOW, CLOSE };

        private int startX = 0;//panel起始X坐標  
        private int xPosition = 0;//bar X軸的座標。
        private int xInterval = 15;//bar之間的間隔。
        private int yHeight = 0;//畫布y軸的高度。

        enum Type { UpBar, DownBar, CrossBar };

        public KBar()
        {
        }

        public KBar(int startX,int barNumber,int yHeight)
        {
            this.barNumber = barNumber;
            this.xPosition = startX + barNumber * xInterval;
            this.yHeight = yHeight;
        }


        public int StartX
        {
            get { return startX; }
            set { startX = value; }
        }
        

        public int YHeight
        {
            get { return yHeight; }
            set { yHeight = value; }
        }

        

        public int XInterval
        {
            get { return xInterval; }
            set { xInterval = value; }
        }

        

        public int XPosition
        {
            get { return xPosition; }
            set { xPosition = value; }
        }

        public int getXPosition()
        {
            this.xPosition = StartX + barNumber * xInterval;
            return xPosition;
        }       

        private int barType = -1;

        private int open = 0;

        public int Open
        {
            get { return open; }
            set { open = value; }
        }
        private int close = 0;

        public int Close
        {
            get { return close; }
            set { close = value; }
        }
        private int high = 0;

        public int High
        {
            get { return high; }
            set { high = value; }
        }
        private int low = 0;

        public int Low
        {
            get { return low; }
            set { low = value; }
        }
        private int barNumber = 1;

        public int BarNumber
        {
            get { return barNumber; }
            set { barNumber = value; }
        }

        

        public Boolean isUpBar()
        {
            if (open > close)
            {
                return false;
            }
            return true;
        }

        public Boolean isDownBar()
        {
            if (open > close)
            {
                return true;
            }
            return false;
        }

        public Boolean isCrossBar()
        {
            if (open == close)
            {
                return true;
            }
            return false;
        }

        public int getBarType()
        {
            if (open == close)
            {

                barType = (int)Type.CrossBar;

            }
            else if (open > close)
            {

                barType = (int)Type.DownBar;

            }
            else if (open < close)
            {

                barType = (int)Type.UpBar;

            }

            return barType;
        }

        

        private int openYPosition()
        {
            return this.YHeight - open;
        }

        private int closeYPosition()
        {
            return this.YHeight - close;
        }

        private int highYPosition() 
        {
            return this.YHeight - high;
        }

        private int lowYPosition()
        {
            return this.YHeight - low;
        }        

        private String tradeYearMonth = "";

        public String TradeYearMonth
        {
            get { return tradeYearMonth; }
            set { tradeYearMonth = value; }
        }

        private String tradeTime = "";

        public String TradeTime
        {
            get { return tradeTime; }
            set { tradeTime = value; }
        }

        private String tradeDate = "";

        public String TradeDate
        {
            get { return tradeDate; }
            set { tradeDate = value; }
        }

        private int tradeAmount = 0;

        public int TradeAmount
        {
            get { return tradeAmount; }
            set { tradeAmount = value; }
        }

    }
}
