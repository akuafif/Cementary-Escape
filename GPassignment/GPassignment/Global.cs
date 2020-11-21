using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cemetery_Escape
{
	static class Global
	{
        public static Player player { get; set; }
        public static HighScore highscore { get; set; }

        /// <summary>
        /// Random Number Generator.
        /// There isn't any similar random generator, like Math.Random() in Java, in C#.
        /// </summary>
        static System.Random objRandom = new System.Random(((int)((System.DateTime.Now.Second % System.Int32.MaxValue))));
        public static int GetRandomNumber(int Low, int High)
        {
            return objRandom.Next(Low, (High + 1));
        }
	}
}
