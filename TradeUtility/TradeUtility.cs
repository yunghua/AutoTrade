using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeUtility
{
    public class TradeUtility
    {
        private static TradeUtility utility = null;

        public static string version = "TradeUtility Version 0.0.0.6";

        static ConfigFile monthFile;//月份代碼設定檔

        private TradeUtility()
        {

        }

        ~TradeUtility()
        {
            monthFile.close();
        }

        public static TradeUtility getInstance()
        {
            try
            {
                if (utility == null)
                {
                    utility = new TradeUtility();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return utility;
        }

        public string dealYearMonth()//計算現在是哪個交易月份
        {
            DateTime now = System.DateTime.Now;

            return Convert.ToString(now.Year) + dealTradMonth(now);
        }

        private string dealTradMonth(DateTime now)//用日期計算出交易月份是哪一個月
        {
            // 本月第一天
            DateTime firstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);

            // 找到第一個星期三
            int wk;//星期幾

            wk = int.Parse(firstDay.DayOfWeek.ToString("d"));

            while (wk != 3)
            {
                firstDay = firstDay.AddDays(1);
                wk = (wk + 1) % 7;
            }

            DateTime thridWeekWendesday = firstDay.AddDays(14);

            if (now.Day > thridWeekWendesday.Day)
            {
                return Convert.ToString(now.Month + 1);
            }
            else
            {
                return Convert.ToString(now.Month);
            }


        }//end dealTradMonth

        private string dealMonthCode(string tradeMonth)//元大期貨交易月份代碼
        {
            return monthFile.readConfig(tradeMonth);
        }

        public string dealTradeCode(string monthFilePath, string code, string month, string year)//計算當天的商品代碼
        {
            try
            {
                monthFile = new ConfigFile(monthFilePath);
            }
            catch (Exception e)
            {
                throw e;
            }

            return dealTradeCode(code, month, year);
        }


        private string dealTradeCode(string code, string month, string year)//計算當天的商品代碼
        {

            try
            {
                DateTime now = System.DateTime.Now;

                string tradeMonth = "";

                if (month == null || month.Trim().Equals(""))
                {

                    tradeMonth = dealTradMonth(now);//目前交易月份

                }
                else
                {
                    tradeMonth = month;
                }

                string monthCode = dealMonthCode(tradeMonth);

                if (monthCode == null || monthCode.Trim().Equals(""))
                {
                    tradeMonth = dealTradMonth(now);//目前交易月份
                    monthCode = dealMonthCode(tradeMonth);
                }

                code += monthCode;

                string yearCode = "";

                if (year == null || year.Trim().Equals(""))
                {
                    yearCode = Convert.ToString(now.Year % 10);
                }
                else
                {
                    yearCode = year.Substring(year.Length - 1);
                }

                code += yearCode;

                return code;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.StackTrace);
                return null;
            }
        }

    }
}
