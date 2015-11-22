using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeUtility
{
    public class TradeUtility
    {
        private static TradeUtility utility = null;

        public static string version = "TradeUtility.TradeUtility Version 0.0.0.4";        

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

        public string dealTradeCode(string monthFilePath,string code)//自動計算當天的商品代碼
        {
            try
            {
                monthFile = new ConfigFile(monthFilePath);                
            }
            catch (Exception e)
            {
                throw e;
            }

            return dealTradeCode(code);
        }



        private string dealTradeCode(string code)//自動計算當天的商品代碼
        {
            
            try
            {
                DateTime now = System.DateTime.Now;

                string tradeMonth = "";

                tradeMonth = dealTradMonth(now);//交易月份

                string monthCode = dealMonthCode(tradeMonth);
                code += monthCode;

                string yearCode = Convert.ToString(now.Year % 10);

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
