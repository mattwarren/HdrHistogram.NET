﻿using System;
using System.Linq;
using HdrHistogram.NET.Utilities;
using NUnit.Framework;

namespace HdrHistogram.NET.Test
{
    public class MiscUtilitiesTest
    {
        static long[] TestNumbers = new long[]
                                        {
                                            //-1, long.MinValue, //MiscUtilities.numberOfLeadingZeros doesn't handle -ve numbers!!!
                                            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
                                            1024,
                                            int.MaxValue,
                                            long.MaxValue - 1,
                                            long.MaxValue
                                        };

        [Test, TestCaseSource("TestNumbers")]
        public void testnumberOfLeadingZeros(long numberToTest)
        {
            Assert.AreEqual(numberOfLeadingZerosSLOW(numberToTest), MiscUtilities.numberOfLeadingZeros(numberToTest));
        }

        private int numberOfLeadingZerosSLOW(long value)
        {
            var valueAsText = Convert.ToString(value, 2);
            if (valueAsText.All(c => c == '0')) 
                valueAsText = string.Empty;
            var leadingZeros = 64 - valueAsText.Length;
            //Console.WriteLine("Value: {0} - \"{1}\", leading zeros = {2} length = {3}", value, valueAsText, leadingZeros, valueAsText.Length);
            return leadingZeros;
        }
    }
}
