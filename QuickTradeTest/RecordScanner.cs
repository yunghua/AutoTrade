using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickTradeTest
{
    static class RecordScanner
    {

        public static List<OriginalRecord> recordList;



        public static void setRecordList(List<OriginalRecord> list)
        {

            recordList = list;

        }

        public static OriginalRecord getRecordMinuteBefore(DateTime beforeTime)
        {

            //Console.WriteLine("beforeTime base:" + beforeTime);

            //Console.WriteLine("recordList count:" + recordList.Count);

            //foreach(OriginalRecord record in recordList){

            OriginalRecord record;

            //for (int i = recordList.Count-1; i > 0; i--)
            for (int i = 0; i < recordList.Count; i++)
            {

                record = (OriginalRecord)recordList[i];

                //Console.WriteLine("NOW Time:" + record.TradeMoment);

                if (record.TradeMoment > beforeTime)
                {
                    //Console.WriteLine("Before Time:" + record.TradeMoment);
                    //Console.WriteLine("Before Price:" + record.TradePrice);

                    return record;
                }
            }

            return null;

        }



        public static OriginalRecord getRecordThreeMinuteAfter(DateTime afterTime)
        {
            Console.WriteLine("afterTime base:" + afterTime);


            foreach (OriginalRecord record in recordList)
            {

                if (record.TradeMoment > afterTime)
                {
                    //Console.WriteLine("Before Time:" + record.TradeMoment);
                    //Console.WriteLine("Before Price:" + record.TradePrice);

                    return record;
                }
            }

            return null;

        }


    }
}
