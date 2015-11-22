using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TradeUtility
{
    public class ConfigFile : TradeFile
    {

        public ConfigFile(String fileName)
            : base(fileName)
        {
            try
            {
                this.prepareReader();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool isEndTrade()
        {
            string config = string.Empty;
            string header = string.Empty;
            string context = string.Empty;
            List<string> contextList = new List<string>();
            string[] rows;

            base.Reader.DiscardBufferedData();
            base.Reader.BaseStream.Seek(0, SeekOrigin.Begin);
            base.Reader.BaseStream.Position = 0;

            while (this.hasNext())
            {
                try
                {
                    config = this.getLine();

                    {
                        rows = config.Split('=');

                        if (rows.Length == 1)
                        {
                            header = rows[0].Trim();

                            if (header.Equals("EndTrade"))
                            {
                                contextList.Clear();
                            }

                        }
                        else
                        {
                            contextList.Add(rows[0].Trim());
                        }
                    }

                }
                catch (Exception)
                {
                    continue;
                }

            }//end while

            base.Reader.DiscardBufferedData();
            base.Reader.BaseStream.Seek(0, SeekOrigin.Begin);
            base.Reader.BaseStream.Position = 0;

            if (contextList == null || contextList.Count == 0)
            {
                return true;//交易結束END TRADE
            }
            else if (contextList.Count > 0)
            {
                for (int i = 0; i < contextList.Count; i++)
                {
                    if(!contextList[i].Trim().Equals("")){
                        return false;
                    }
                }
                return true;
            }

            return true;

        }

        //public bool isEndStringContains(string compareStr)
        //{
        //    if (base.Reader != null)
        //    {
        //        base.Reader.DiscardBufferedData();
        //        base.Reader.BaseStream.Seek(- (compareStr.Length), SeekOrigin.End);
        //    }
        //    string endStr = this.getLine();

        //    return endStr.Contains(compareStr);
        //}

        public string readConfig(string headerText)
        {

            string config = string.Empty;
            string header = string.Empty;
            string context = string.Empty;
            string[] rows;


            base.Reader.DiscardBufferedData();
            base.Reader.BaseStream.Seek(0, SeekOrigin.Begin);
            base.Reader.BaseStream.Position = 0;

            while (this.hasNext())
            {
                try
                {
                    config = this.getLine();

                    if (config.StartsWith("##"))
                    {
                        continue;
                    }
                    else
                    {
                        rows = config.Split('=');

                        if (rows.Length >= 2)
                        {
                            header = rows[0].Trim();
                            context = rows[1].Trim();
                        }

                        if (header.Equals(headerText))
                        {
                            return context;
                        }

                    }

                }
                catch (Exception e)
                {
                    //Console.WriteLine(e.Message);
                    //Console.WriteLine(e.Source);
                    //Console.WriteLine(e.StackTrace);
                    throw e;

                }

            }
            return null;
        }//end of readConfig

        //behindText，從哪個字眼之後開始，例如EndTrade，代表平倉後的紀錄
        public List<string> readMultiLineConfig(string headerText, string behindText)
        {
            string config = string.Empty;
            string header = string.Empty;
            string context = string.Empty;
            List<string> contextList = new List<string>();
            string[] rows;

            base.Reader.DiscardBufferedData();
            base.Reader.BaseStream.Seek(0, SeekOrigin.Begin);
            base.Reader.BaseStream.Position = 0;

            while (this.hasNext())
            {
                try
                {
                    config = this.getLine();

                    if (config.StartsWith("##"))
                    {
                        continue;
                    }
                    else
                    {
                        rows = config.Split('=');

                        if (rows.Length >= 2)
                        {
                            header = rows[0].Trim();
                            context = rows[1].Trim();

                            if (header.Equals(headerText))
                            {
                                contextList.Add(context);
                            }

                        }
                        else if (rows.Length == 1)
                        {
                            header = rows[0].Trim();

                            if (header.Equals(behindText))
                            {
                                contextList.Clear();
                            }

                            if (header.Equals(headerText))
                            {
                                contextList.Add(header);
                            }

                        }


                    }

                }
                catch (Exception)
                {
                    //Console.WriteLine(e.Message);
                    //Console.WriteLine(e.Source);
                    //Console.WriteLine(e.StackTrace);
                    continue;
                }

            }//end while
            return contextList;
        }

        public List<string> readMultiLineConfig(string headerText)
        {

            string config = string.Empty;
            string header = string.Empty;
            string context = string.Empty;
            List<string> contextList = new List<string>();
            string[] rows;

            base.Reader.DiscardBufferedData();
            base.Reader.BaseStream.Seek(0, SeekOrigin.Begin);
            base.Reader.BaseStream.Position = 0;

            while (this.hasNext())
            {
                try
                {
                    config = this.getLine();

                    if (config.StartsWith("##"))
                    {
                        continue;
                    }
                    else
                    {
                        rows = config.Split('=');

                        if (rows.Length >= 2)
                        {
                            header = rows[0].Trim();
                            context = rows[1].Trim();

                            if (header.Equals(headerText))
                            {
                                contextList.Add(context);
                            }
                        }

                    }

                }
                catch (Exception)
                {
                    //Console.WriteLine(e.Message);
                    //Console.WriteLine(e.Source);
                    //Console.WriteLine(e.StackTrace);
                    continue;
                }

            }//end while
            return contextList;
        }//end of readMultiLineConfig


    }
}
