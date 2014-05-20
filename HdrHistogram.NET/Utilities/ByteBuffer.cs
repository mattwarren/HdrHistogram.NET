/**
 * Original version written by Gil Tene of Azul Systems, and released to the public domain,
 * as explained at http://creativecommons.org/publicdomain/zero/1.0/
 *
 * @author Gil Tene
 * 
 * This is a .NET port of the original Java version, .NET port by Matt Warren
 */

namespace HdrHistogram.NET.Utilities
{
    // See http://stackoverflow.com/questions/1261543/equivalent-of-javas-bytebuffer-puttype-in-c-sharp
    // and http://stackoverflow.com/questions/18040012/what-is-the-equivalent-of-javas-bytebuffer-wrap-in-c
    // and http://stackoverflow.com/questions/1261543/equivalent-of-javas-bytebuffer-puttype-in-c-sharp
    public class ByteBuffer
    {
        public static ByteBuffer allocate(int bufferCapacity)
        {
            throw new System.NotImplementedException();
        }

        public int capacity()
        {
            throw new System.NotImplementedException();
        }

        public void clear()
        {
            throw new System.NotImplementedException();
        }

        public int getInt()
        {
            throw new System.NotImplementedException();
        }

        public long getLong()
        {
            throw new System.NotImplementedException();
        }

        public void putInt(int value)
        {
            throw new System.NotImplementedException();
        }

        public void putLong(long value)
        {
            throw new System.NotImplementedException();
        }
    }
}