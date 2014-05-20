/**
 * Original version written by Gil Tene of Azul Systems, and released to the public domain,
 * as explained at http://creativecommons.org/publicdomain/zero/1.0/
 *
 * @author Gil Tene
 * 
 * This is a .NET port of the original Java version, .NET port by Matt Warren
 */

using CSharp.Atomic;

namespace HdrHistogram.NET
{
    public abstract class AbstractHistogramBase
    {
        internal static readonly AtomicLong constructionIdentityCount = new AtomicLong(0);

        // "Cold" accessed fields. Not used in the recording code path:
        internal long identity;

        internal long highestTrackableValue;

        internal long lowestTrackableValue;

        internal int numberOfSignificantValueDigits;

        internal int bucketCount;

        internal int subBucketCount;

        internal int countsArrayLength;

        internal int wordSizeInBytes;

        internal long startTimeStampMsec;

        internal long endTimeStampMsec;

        internal HistogramData histogramData;
    }
}