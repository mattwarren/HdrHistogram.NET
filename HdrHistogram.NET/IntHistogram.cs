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
     * <h3>A High Dynamic Range (HDR) Histogram using an <b><code>int</code></b> count type </h3>
     * <p>
     * See package description for {@link org.HdrHistogram} for details.
     */
    public class IntHistogram : AbstractHistogram 
    {
        long totalCount;
        readonly int[] counts;

        // @Override
        public override long getCountAtIndex(/*final*/ int index) 
        {
            return counts[index];
        }

        // @Override
        public override void incrementCountAtIndex(/*final*/ int index) 
        {
            counts[index]++;
        }

        // @Override
        public override void addToCountAtIndex(/*final*/ int index, /*final*/ long value) 
        {
            counts[index] += (int)value;
        }

        // @Override
        public override void clearCounts() 
        {
            Array.Clear(counts, 0, counts.Length);
            totalCount = 0;
        }

        /**
         * @inheritDoc
         */
        // @Override
        public override /*IntHistogram*/ AbstractHistogram copy() 
        {
            IntHistogram copy = new IntHistogram(lowestTrackableValue, highestTrackableValue, numberOfSignificantValueDigits);
            copy.add(this);
            return copy;
        }

        /**
         * @inheritDoc
         */
        // @Override
        public override /*IntHistogram*/ AbstractHistogram copyCorrectedForCoordinatedOmission(/*final*/ long expectedIntervalBetweenValueSamples) 
        {
            IntHistogram toHistogram = new IntHistogram(lowestTrackableValue, highestTrackableValue, numberOfSignificantValueDigits);
            toHistogram.addWhileCorrectingForCoordinatedOmission(this, expectedIntervalBetweenValueSamples);
            return toHistogram;
        }

        // @Override
        public override long getTotalCount() 
        {
            return totalCount;
        }

        // @Override
        public override void setTotalCount(/*final*/ long totalCount) 
        {
            this.totalCount = totalCount;
        }

        // @Override
        public override void incrementTotalCount() 
        {
            totalCount++;
        }

        // @Override
        public override void addToTotalCount(long value) 
        {
            totalCount += value;
        }

        // @Override
        public override int getEstimatedFootprintInBytes() 
        {
            return (512 + (4 * counts.Length));
        }

        /**
         * Construct a IntHistogram given the Highest value to be tracked and a number of significant decimal digits. The
         * histogram will be constructed to implicitly track (distinguish from 0) values as low as 1.
         *
         * @param highestTrackableValue The highest value to be tracked by the histogram. Must be a positive
         *                              integer that is >= 2.
         * @param numberOfSignificantValueDigits The number of significant decimal digits to which the histogram will
         *                                       maintain value resolution and separation. Must be a non-negative
         *                                       integer between 0 and 5.
         */
        public IntHistogram(/*final*/ long highestTrackableValue, /*final*/ int numberOfSignificantValueDigits) 
            : this(1, highestTrackableValue, numberOfSignificantValueDigits)
        {
        }

        /**
         * Construct a IntHistogram given the Lowest and Highest values to be tracked and a number of significant
         * decimal digits. Providing a lowestTrackableValue is useful is situations where the units used
         * for the histogram's values are much smaller that the minimal accuracy required. E.g. when tracking
         * time values stated in nanosecond units, where the minimal accuracy required is a microsecond, the
         * proper value for lowestTrackableValue would be 1000.
         *
         * @param lowestTrackableValue The lowest value that can be tracked (distinguished from 0) by the histogram.
         *                             Must be a positive integer that is >= 1. May be internally rounded down to nearest
         *                             power of 2.
         * @param highestTrackableValue The highest value to be tracked by the histogram. Must be a positive
         *                              integer that is >= (2 * lowestTrackableValue).
         * @param numberOfSignificantValueDigits The number of significant decimal digits to which the histogram will
         *                                       maintain value resolution and separation. Must be a non-negative
         *                                       integer between 0 and 5.
         */
        public IntHistogram(/*final*/ long lowestTrackableValue, /*final*/ long highestTrackableValue, /*final*/ int numberOfSignificantValueDigits) 
            : base(lowestTrackableValue, highestTrackableValue, numberOfSignificantValueDigits)
        {
            counts = new int[countsArrayLength];
            wordSizeInBytes = 4;
        }

        ///**
        // * Construct a new histogram by decoding it from a ByteBuffer.
        // * @param buffer The buffer to decode from
        // * @param minBarForHighestTrackableValue Force highestTrackableValue to be set at least this high
        // * @return The newly constructed histogram
        // */
        //public static IntHistogram decodeFromByteBuffer(/*final*/ ByteBuffer buffer,
        //                                                /*final*/ long minBarForHighestTrackableValue) {
        //    return (IntHistogram) decodeFromByteBuffer(buffer, IntHistogram.class,
        //            minBarForHighestTrackableValue);
        //}

        ///**
        // * Construct a new histogram by decoding it from a compressed form in a ByteBuffer.
        // * @param buffer The buffer to encode into
        // * @param minBarForHighestTrackableValue Force highestTrackableValue to be set at least this high
        // * @return The newly constructed histogram
        // * @throws DataFormatException
        // */
        //public static IntHistogram decodeFromCompressedByteBuffer(/*final*/ ByteBuffer buffer,
        //                                                          /*final*/ long minBarForHighestTrackableValue) //throws DataFormatException 
        //{
        //    return (IntHistogram) decodeFromCompressedByteBuffer(buffer, IntHistogram.class,
        //            minBarForHighestTrackableValue);
        //}

        //private void readObject(/*final*/ ObjectInputStream o)
        //        throws IOException, ClassNotFoundException {
        //    o.defaultReadObject();
        //}

        /*synchronized*/
        public override void fillCountsArrayFromBuffer( /*final*/ ByteBuffer buffer, /*final*/ int length)
        {
            throw new NotImplementedException();
        }

        //// @Override
        //synchronized void fillCountsArrayFromBuffer(/*final*/ ByteBuffer buffer, /*final*/ int length) {
        //    buffer.asIntBuffer().get(counts, 0, length);
        //}

        //// We try to cache the LongBuffer used in output cases, as repeated
        //// output form the same histogram using the same buffer is likely:
        //private IntBuffer cachedDstIntBuffer = null;
        //private ByteBuffer cachedDstByteBuffer = null;
        //private int cachedDstByteBufferPosition = 0;

        /*synchronized*/
        public override void fillBufferFromCountsArray( /*final*/ ByteBuffer buffer, /*final*/ int length)
        {
            throw new NotImplementedException();
        }

        //// @Override
        //synchronized void fillBufferFromCountsArray(/*final*/ ByteBuffer buffer, /*final*/ int length) {
        //    if ((cachedDstIntBuffer == null) ||
        //            (buffer != cachedDstByteBuffer) ||
        //            (buffer.position() != cachedDstByteBufferPosition)) {
        //        cachedDstByteBuffer = buffer;
        //        cachedDstByteBufferPosition = buffer.position();
        //        cachedDstIntBuffer = buffer.asIntBuffer();
        //    }
        //    cachedDstIntBuffer.rewind();
        //    cachedDstIntBuffer.put(counts, 0, length);
        //}
    }
}
