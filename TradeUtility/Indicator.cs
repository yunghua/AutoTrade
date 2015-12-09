using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace TradeUtility
{
    public class Indicator
    {
   

        private static int getMaxClosePrice(int kBarCount, int currentKBarNumber, Hashtable htKBar)
        {
            int maxClosePrice = 0;
            int iBase = kBarCount;
            int iKBarStartIndex = 0;

            if (currentKBarNumber < kBarCount)
            {
                iKBarStartIndex = 1;
            }
            else
            {
                iKBarStartIndex = currentKBarNumber - kBarCount + 1;
            }

            for (int index = iKBarStartIndex; index <= currentKBarNumber; index++)
            {
                if (index <= 0)
                {
                    iBase--;
                    continue;
                }
                KBar kBar = (KBar)htKBar[index];
                if (kBar.Close != 0 && kBar.Close > maxClosePrice)
                {
                    maxClosePrice = kBar.Close;
                }

            }

            return maxClosePrice;
        }

        private static int getMiniClosePrice(int kBarCount, int currentKBarNumber, Hashtable htKBar)
        {
            int miniClosePrice = 99999;
            int iBase = kBarCount;
            int iKBarStartIndex = 0;

            if (currentKBarNumber < kBarCount)
            {
                iKBarStartIndex = 1;
            }
            else
            {
                iKBarStartIndex = currentKBarNumber - kBarCount + 1;
            }

            for (int index = iKBarStartIndex; index <= currentKBarNumber; index++)
            {
                if (index <= 0)
                {
                    iBase--;
                    continue;
                }
                KBar kBar = (KBar)htKBar[index];
                if (kBar.Close != 0 && kBar.Close < miniClosePrice)
                {
                    miniClosePrice = kBar.Close;
                }

            }

            return miniClosePrice;
        }

        public static double RSV(int kBarCount, int currentKBarNumber, Hashtable htKBar)
        {
            double rsv = 0;
            KBar kBar = (KBar)htKBar[currentKBarNumber];
            int currentClosePrice = 0;
            currentClosePrice = kBar.Close;

            int miniClosePrice = Indicator.getMiniClosePrice(kBarCount, currentKBarNumber, htKBar);
            int maxClosePrice = Indicator.getMaxClosePrice(kBarCount, currentKBarNumber, htKBar);

            rsv = (((double)(currentClosePrice - miniClosePrice)) / ((double)maxClosePrice - miniClosePrice)) * 100;

            return rsv;
        }

        public static double IndicatorK(int kBarCount, int currentKBarNumber, Hashtable htKBar)
        {
            double previousK = 0;
            double currentK = 0;

            if (currentKBarNumber <= 0)
            {
                return 0;
            }
            else if (currentKBarNumber == 1)
            {
                previousK = 50;
            }
            else if (currentKBarNumber > 1)
            {
                previousK = IndicatorK(kBarCount, currentKBarNumber - 1, htKBar);
            }

            currentK = previousK * 2 / 3 + RSV(kBarCount, currentKBarNumber, htKBar) * 1 / 3;

            return currentK;
        }

        public static double IndicatorD(int kBarCount, int currentKBarNumber, Hashtable htKBar)
        {
            double previousD = 0;
            double currentD = 0;

            if (currentKBarNumber <= 0)
            {
                return 0;
            }
            else if (currentKBarNumber == 1)
            {
                previousD = 50;
            }
            else if (currentKBarNumber > 1)
            {
                previousD = IndicatorD(kBarCount, currentKBarNumber - 1, htKBar);
            }

            currentD = previousD * 2 / 3 + IndicatorK(kBarCount, currentKBarNumber, htKBar) * 1 / 3;

            return currentD;

        }

        public static double MACD(int kBarCount, int priceType, int currentKBarNumber, Hashtable htKBar)
        {
            int iBase = kBarCount;

            int iKBarStartIndex = 0;
            iKBarStartIndex = currentKBarNumber - kBarCount + 1;
            double dResult = 0.0;

            for (int index = iKBarStartIndex; index <= currentKBarNumber; index++)
            {
                if (index <= 0)
                {
                    iBase--;
                    continue;
                }
                KBar kBar = (KBar)htKBar[index];

                if (priceType == (int)KBar.PriceType.OPEN)
                {
                    dResult += kBar.Open;
                }
                else if (priceType == (int)KBar.PriceType.LOW)
                {
                    dResult += kBar.Low;
                }
                else if (priceType == (int)KBar.PriceType.HIGH)
                {
                    dResult += kBar.High;
                }
                else if (priceType == (int)KBar.PriceType.CLOSE)
                {
                    dResult += kBar.Close;
                }
            }

            dResult = dResult / iBase;

            return dResult;

        }


    }
}
