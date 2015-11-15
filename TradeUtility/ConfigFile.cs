using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeUtility
{
    public class ConfigFile : TradeFile
    {

        public ConfigFile(String fileName)
            : base(fileName)
        {
        }

        public string readConfig(string headerText)
        {

            string config = string.Empty;
            string header = string.Empty;
            string context = string.Empty;
            string[] rows;

            this.prepareReader();

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
                catch (Exception)
                {
                    //Console.WriteLine(e.Message);
                    //Console.WriteLine(e.Source);
                    //Console.WriteLine(e.StackTrace);
                    continue;
                }

            }
            return null;
        }//end of readConfig

        //behindText，從哪個字眼之後開始，例如EndTrade，代表平倉後的紀錄
        public List<string> readMultiLineConfig(string headerText,string behindText)
        {
            string config = string.Empty;
            string header = string.Empty;
            string context = string.Empty;
            List<string> contextList = new List<string>();
            string[] rows;

            this.prepareReader();

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

            this.prepareReader();

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
