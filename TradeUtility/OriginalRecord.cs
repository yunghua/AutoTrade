using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeUtility
{


    public class OriginalRecord : IComparable
    {


        private int tradeYear;

        public int TradeYear
        {
            get { return tradeYear; }
            set { tradeYear = value; }
        }

        private int tradeMonth;

        public int TradeMonth
        {
            get { return tradeMonth; }
            set { tradeMonth = value; }
        }

        private int tradeDay;

        public int TradeDay
        {
            get { return tradeDay; }
            set { tradeDay = value; }
        }

        private int tradeHour;

        public int TradeHour
        {
            get { return tradeHour; }
            set { tradeHour = value; }
        }

        private int tradeMinute;

        public int TradeMinute
        {
            get { return tradeMinute; }
            set { tradeMinute = value; }
        }

        private int tradeSecond;

        public int TradeSecond
        {
            get { return tradeSecond; }
            set { tradeSecond = value; }
        }



        private String tradeDate = "";

        public String TradeDate
        {
            get { return tradeDate; }
            set { tradeDate = value; }
        }
        private String tradeTime = "";

        public String TradeTime
        {
            get { return tradeTime; }
            set { tradeTime = value; }
        }
        private int tradePrice = 0;

        public int TradePrice
        {
            get { return tradePrice; }
            set { tradePrice = value; }
        }
        private int tradeVolumn = 0;

        public int TradeVolumn
        {
            get { return tradeVolumn; }
            set { tradeVolumn = value; }
        }

        DateTime tradeMoment;

        public DateTime TradeMoment
        {
            get { return tradeMoment; }
            set { tradeMoment = value; }
        }

        private String tradeCode = "";

        public String TradeCode
        {
            get { return tradeCode; }
            set { tradeCode = value; }
        }
        private String tradeYearMonth = "";

        public String TradeYearMonth
        {
            get { return tradeYearMonth; }
            set { tradeYearMonth = value; }
        }



        #region IComparable Members

        public int CompareTo(object obj)
        {
            OriginalRecord recordToBeCompared = (OriginalRecord)obj;

            return this.TradeTime.CompareTo(recordToBeCompared.TradeTime);
        }

        #endregion
    }
}
