using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeUtility;

namespace QuickTradeTest
{

    static class OriginalRecordConverter
    {




        public static OriginalRecord getOriginalRecord(String currentLine)
        {

            try
            {

                OriginalRecord record = new OriginalRecord();

                String currentPrice = "";

                int tradeCurrentVolumn = 0;

                String[] tmpData = new String[10];

                tmpData = currentLine.Split(',');


                if (tmpData == null || tmpData[1] == null)
                {
                    return null;
                }


                record.TradeDate = tmpData[0].Trim();

                record.TradeTime = tmpData[3].Trim();

                record.TradeYearMonth = tmpData[2].Trim(); ;

                if (record.TradeYearMonth.Contains('/'))
                {
                    currentPrice = tmpData[6].Trim();
                }
                else
                {
                    currentPrice = tmpData[4].Trim();
                }



                tradeCurrentVolumn = int.Parse(tmpData[5].Trim());

                record.TradePrice = Convert.ToInt32(currentPrice);

                record.TradeVolumn = tradeCurrentVolumn / 2;

                record.TradeYear = int.Parse(record.TradeDate.Substring(0, 4));

                record.TradeMonth = int.Parse(record.TradeDate.Substring(4, 2));

                record.TradeDay = int.Parse(record.TradeDate.Substring(6, 2));

                record.TradeHour = int.Parse(record.TradeTime.Substring(0, 2));

                record.TradeMinute = int.Parse(record.TradeTime.Substring(2, 2));

                record.TradeSecond = int.Parse(record.TradeTime.Substring(4, 2));


                record.TradeMoment = new DateTime(record.TradeYear, record.TradeMonth, record.TradeDay, record.TradeHour, record.TradeMinute, record.TradeSecond);

                return record;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                return null ;

            }

        }
    }

}
