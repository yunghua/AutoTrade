using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeUtility
{
    public static class RecordScanner
    //用來掃描交易紀錄LIST
    {

        public static List<OriginalRecord> recordList;

        public static void setRecordList(List<OriginalRecord> list)
        {

            recordList = list;

        }

        //取得一段時間前或是之後的交易紀錄
        public static OriginalRecord getRecordMinuteBeforeOrAfter(List<OriginalRecord> list, DateTime baseTime)
        {
            try
            {
                recordList = list;

                return getRecordMinuteBeforeOrAfter(baseTime);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //取得一段時間前或是之後的交易紀錄
        public static OriginalRecord getRecordMinuteBeforeOrAfter(DateTime baseTime)
        {
            try
            {
                if (recordList ==null)
                {
                    Console.WriteLine("recordListis null");
                    return null; 
                }

                OriginalRecord record;

                //for (int i = recordList.Count; i > 0; i--)
                for (int i =0;i< recordList.Count;   i++)
                {
                    //record = (OriginalRecord)recordList[i - 1];

                    record = (OriginalRecord)recordList[i];

                    if (record.TradeMoment < baseTime)
                    {
                        return record;
                    }
                }

                return null;

            }
            catch (Exception e)
            {
                throw e;
            }

        }

    }
}
