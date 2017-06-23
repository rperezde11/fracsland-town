using Utils;
using System.Collections.Generic;
using System;

namespace Items
{
    public class Fraction : Item
    {
        [Serializable]
        public struct SimplifiedFraction
        {
            public int top, down, fracIdentifier;

            public SimplifiedFraction(int t, int d, int iden)
            {
                top = t;
                down = d;
                fracIdentifier = iden;
            }
        
        }

        public static int identifiers = Constants.ID_FRACTION;
        public static List<SimplifiedFraction> alreadyCreatedFractions = new List<SimplifiedFraction>();

        public int top;
        public int down;
        public int fracIdentifier;

        public Fraction(int id, string name, string image, int top, int down) : base(id, name, image)
        {
            this.top = top;
            this.down = down;
            fracIdentifier = Fraction.GetFractionIdentifier(top, down);
        }

        /// <summary>
        /// This is done because all items in this game have an unique id and this way we can ensure that every fraction will
        /// have it's own id
        /// </summary>
        /// <param name="top"></param>
        /// <param name="down"></param>
        /// <returns></returns>
        public static int GetFractionIdentifier(int top, int down)
        {
            bool found = false;
            int i = 0;
            while(!found && i < alreadyCreatedFractions.Count)
            {
                if(alreadyCreatedFractions[i].top == top && alreadyCreatedFractions[i].down == down)
                {
                    found = true;
                    return alreadyCreatedFractions[i].fracIdentifier;
                }
                i++;
            }

            int newId = identifiers++;
            alreadyCreatedFractions.Add(new SimplifiedFraction(top, down, newId));
            return newId;
        }

        public float toDecimal()
        {
            return top / down;
        }

        public static bool operator !=(Fraction frac1, Fraction frac2)
        {
			return frac1.top * frac2.down != frac1.down * frac2.top;
        }

        public static bool operator ==(Fraction frac1, Fraction frac2)
        {
			return frac1.top * frac2.down == frac1.down * frac2.top;
        }

        public static bool operator !=(SimplifiedFraction frac1, Fraction frac2)
        {
			return frac1.top * frac2.down != frac1.down * frac2.top;
        }

        public static bool operator ==(SimplifiedFraction frac1, Fraction frac2)
        {
			return frac1.top * frac2.down == frac1.down * frac2.top;
        }

		public static bool operator !=(Fraction frac1, SimplifiedFraction frac2)
		{
			return frac1.top * frac2.down != frac1.down * frac2.top;
		}

		public static bool operator ==(Fraction frac1, SimplifiedFraction frac2)
		{
			return frac1.top * frac2.down == frac1.down * frac2.top;
		}
    }
}
