namespace HdrHistogram.NET.Utilities
{
    public static class MiscUtilities
    {
        // Conversion of int version from http://stackoverflow.com/a/10439718
        public static int numberOfLeadingZeros(long value)
        {
            int leadingZeros = 0;
            while (value > 0)
            {
                value = value >> 1;
                leadingZeros++;
            }

            return (64 - leadingZeros);
        }
    }
}
