using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeUtility
{
    public static class Dice
    {

        public static Boolean run(int maxRange)
        {

            Random dice = new Random(Guid.NewGuid().GetHashCode());

            int randomNumber = dice.Next(1, maxRange);

            //Console.WriteLine("DICE:" + randomNumber);

            if (1 == randomNumber)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
