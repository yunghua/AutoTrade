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


    }
}
