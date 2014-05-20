using NUnit.Framework;

namespace HdrHistogram.NET.Test
{
    /// <summary>
    /// This class is here just to make the porting easier, means we can leave the original unit test alone 
    /// and use the wrapper to pass the Java junit method calls through to the equivalent C# nuit method
    /// </summary>
    public static class AssertEx
    {
        public static void assertEquals(string message, long expected, long actual)
        {
            Assert.AreEqual(expected, actual, message);
        }

        internal static void assertEquals(string message, double expected, double actual, double delta)
        {
            Assert.AreEqual(expected, actual, delta, message);
        }

        public static void assertEquals(long expected, long actual)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void assertEquals(object expected, object actual)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void assertNotSame(object expected, object actual)
        {
            Assert.AreNotSame(expected, actual);
        }

        public static void assertTrue(bool condition)
        {
            Assert.IsTrue(condition);
        }

        internal static void assertTrue(string message, bool condition)
        {
            Assert.IsTrue(condition, message);
        }

        public static void assertFalse(bool condition)
        {
            Assert.IsFalse(condition);
        }
    }
}