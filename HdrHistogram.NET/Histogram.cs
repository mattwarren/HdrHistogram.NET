﻿/**
 * Original version written by Gil Tene of Azul Systems, and released to the public domain,
 * as explained at http://creativecommons.org/publicdomain/zero/1.0/
 *
 * @author Gil Tene
 * 
 * This is a .NET port of the original Java version, .NET port by Matt Warren
 */

using System;
using HdrHistogram.NET.Utilities;

namespace HdrHistogram.NET
{
    /**
     * <h3>A High Dynamic Range (HDR) Histogram</h3>
     * <p>
     * Histogram supports the recording and analyzing sampled data value counts across a configurable integer value
     * range with configurable value precision within the range. Value precision is expressed as the number of significant
     * digits in the value recording, and provides control over value quantization behavior across the value range and the
     * subsequent value resolution at any given level.
     * <p>
     * For example, a Histogram could be configured to track the counts of observed integer values between 0 and
     * 3,600,000,000 while maintaining a value precision of 3 significant digits across that range. Value quantization
     * within the range will thus be no larger than 1/1,000th (or 0.1%) of any value. This example Histogram could
     * be used to track and analyze the counts of observed response times ranging between 1 microsecond and 1 hour
     * in magnitude, while maintaining a value resolution of 1 microsecond up to 1 millisecond, a resolution of
     * 1 millisecond (or better) up to one second, and a resolution of 1 second (or better) up to 1,000 seconds. At it's
     * maximum tracked value (1 hour), it would still maintain a resolution of 3.6 seconds (or better).
     * <p>
     * Histogram tracks value counts in <b><code>long</code></b> fields. Smaller field types are available in the
     * {@link org.HdrHistogram.IntHistogram} and {@link org.HdrHistogram.ShortHistogram} implementations of
     * {@link org.HdrHistogram.AbstractHistogram}.
     * <p>
     * See package description for {@link org.HdrHistogram} for details.
     */
    public class Histogram : AbstractHistogram
    {
        long totalCount;
        readonly long[] counts;

        public override long getCountAtIndex(/*final*/ int index) 
        {
            return counts[index];
        }

        public override void incrementCountAtIndex(/*final*/ int index) 
        {
            counts[index]++;
        }

        public override void addToCountAtIndex(/*final*/ int index, /*final*/ long value) 
        {
            counts[index] += value;
        }

        public override void clearCounts()
        {
            Array.Clear(counts, 0, counts.Length);
            totalCount = 0;
        }

        /**
         * @inheritDoc
         */
        public override /*Histogram*/ AbstractHistogram copy() 
        {
            Histogram copy = new Histogram(lowestTrackableValue, highestTrackableValue, numberOfSignificantValueDigits);
            copy.add(this);
            return copy;
        }

        /**
         * @inheritDoc
         */
        public override /*Histogram*/ AbstractHistogram copyCorrectedForCoordinatedOmission(/*final*/ long expectedIntervalBetweenValueSamples) 
        {
            Histogram toHistogram = new Histogram(lowestTrackableValue, highestTrackableValue, numberOfSignificantValueDigits);
            toHistogram.addWhileCorrectingForCoordinatedOmission(this, expectedIntervalBetweenValueSamples);
            return toHistogram;
        }

        public override long getTotalCount() 
        {
            return totalCount;
        }

        public override void setTotalCount(/*final*/ long totalCount) 
        {
            this.totalCount = totalCount;
        }

        public override void incrementTotalCount() 
        {
            totalCount++;
        }

        public override void addToTotalCount(/*final*/ long value) 
        {
            totalCount += value;
        }

        public override int getEstimatedFootprintInBytes() 
        {
            return (512 + (8 * counts.Length));
        }

        /**
         * Construct a Histogram given the Highest value to be tracked and a number of significant decimal digits. The
         * histogram will be constructed to implicitly track (distinguish from 0) values as low as 1.
         *
         * @param highestTrackableValue          The highest value to be tracked by the histogram. Must be a positive
         *                                       integer that is >= 2.
         * @param numberOfSignificantValueDigits The number of significant decimal digits to which the histogram will
         *                                       maintain value resolution and separation. Must be a non-negative
         *                                       integer between 0 and 5.
         */
        public Histogram(/*final*/ long highestTrackableValue, /*final*/ int numberOfSignificantValueDigits) 
            : this(1, highestTrackableValue, numberOfSignificantValueDigits)
        {
        }

        /**
         * Construct a Histogram given the Lowest and Highest values to be tracked and a number of significant
         * decimal digits. Providing a lowestTrackableValue is useful is situations where the units used
         * for the histogram's values are much smaller that the minimal accuracy required. E.g. when tracking
         * time values stated in nanosecond units, where the minimal accuracy required is a microsecond, the
         * proper value for lowestTrackableValue would be 1000.
         *
         * @param lowestTrackableValue           The lowest value that can be tracked (distinguished from 0) by the histogram.
         *                                       Must be a positive integer that is >= 1. May be internally rounded down to nearest
         *                                       power of 2.
         * @param highestTrackableValue          The highest value to be tracked by the histogram. Must be a positive
         *                                       integer that is >= (2 * lowestTrackableValue).
         * @param numberOfSignificantValueDigits The number of significant decimal digits to which the histogram will
         *                                       maintain value resolution and separation. Must be a non-negative
         *                                       integer between 0 and 5.
         */
        public Histogram(/*final*/ long lowestTrackableValue, /*final*/ long highestTrackableValue,
                         /*final*/ int numberOfSignificantValueDigits) 
            : base(lowestTrackableValue, highestTrackableValue, numberOfSignificantValueDigits)
        {
            counts = new long[countsArrayLength];
            wordSizeInBytes = 8;
        }

        ///**
        // * Construct a new histogram by decoding it from a ByteBuffer.
        // * @param buffer The buffer to decode from
        // * @param minBarForHighestTrackableValue Force highestTrackableValue to be set at least this high
        // * @return The newly constructed histogram
        // */
        //public static Histogram decodeFromByteBuffer(/*final*/ ByteBuffer buffer,
        //                                             /*final*/ long minBarForHighestTrackableValue) 
        //{
        //    return (Histogram) decodeFromByteBuffer(buffer, Histogram.class, minBarForHighestTrackableValue);
        //}

        ///**
        // * Construct a new histogram by decoding it from a compressed form in a ByteBuffer.
        // * @param buffer The buffer to encode into
        // * @param minBarForHighestTrackableValue Force highestTrackableValue to be set at least this high
        // * @return The newly constructed histogram
        // * @throws DataFormatException
        // */
        //public static Histogram decodeFromCompressedByteBuffer(/*final*/ ByteBuffer buffer,
        //                                                       /*final*/ long minBarForHighestTrackableValue) //throws DataFormatException 
        //{
        //    return (Histogram) decodeFromCompressedByteBuffer(buffer, Histogram.class, minBarForHighestTrackableValue);
        //}

        //private void readObject(/*final*/ ObjectInputStream o) // throws IOException, ClassNotFoundException 
        //{
        //    o.defaultReadObject();
        //}

        /*synchronized*/
        public override void fillCountsArrayFromBuffer( /*final*/ ByteBuffer buffer, /*final*/ int length)
        {
            throw new NotImplementedException();
        }

        ///*synchronized*/ public override void fillCountsArrayFromBuffer(/*final*/ ByteBuffer buffer, /*final*/ int length) 
        //{
        //    buffer.asLongBuffer().get(counts, 0, length);
        //}

        //// We try to cache the LongBuffer used in output cases, as repeated
        //// output form the same histogram using the same buffer is likely:
        //private LongBuffer cachedDstLongBuffer = null;
        //private ByteBuffer cachedDstByteBuffer = null;
        //private int cachedDstByteBufferPosition = 0;

        /*synchronized*/
        public override void fillBufferFromCountsArray( /*final*/ ByteBuffer buffer, /*final*/ int length)
        {
            throw new NotImplementedException();
        }

        ///*synchronized*/ public override void fillBufferFromCountsArray(/*final*/ ByteBuffer buffer, /*final*/ int length) 
        //{
        //    if ((cachedDstLongBuffer == null) ||
        //            (buffer != cachedDstByteBuffer) ||
        //            (buffer.position() != cachedDstByteBufferPosition)) 
        //    {
        //        cachedDstByteBuffer = buffer;
        //        cachedDstByteBufferPosition = buffer.position();
        //        cachedDstLongBuffer = buffer.asLongBuffer();
        //    }
        //    cachedDstLongBuffer.rewind();
        //    cachedDstLongBuffer.put(counts, 0, length);
        //}
    }
}
