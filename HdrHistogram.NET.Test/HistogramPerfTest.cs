﻿/**
 * HistogramTest.java
 * Written by Gil Tene of Azul Systems, and released to the public domain,
 * as explained at http://creativecommons.org/publicdomain/zero/1.0/
 *
 * @author Gil Tene
 * 
 * This is a .NET port of the original Java version, .NET port by Matt Warren
 */

using System;
using System.Diagnostics;
using System.Threading;
using HdrHistogram.NET.Utilities;
using NUnit.Framework;
using System.Threading.Tasks;

namespace HdrHistogram.NET.Test
{
    /**
     * JUnit test for {@link Histogram}
     */
    [Category("Performance (Long Running)")]
    public class HistogramPerfTest 
    {
        /// <summary> 3,600,000,000 (3600L * 1000 * 1000, e.g. for 1 hr in usec units) </summary>
        static readonly long highestTrackableValue = 3600L * 1000 * 1000; // e.g. for 1 hr in usec units
        /// <summary> 3 </summary>
        static readonly int numberOfSignificantValueDigits = 3;
        /// <summary> 12340 </summary>
        static readonly long testValueLevel = 12340;
        /// <summary> 50,000 </summary>
        static readonly long warmupLoopLength = 50000;
        /// <summary> 200,000,000 </summary>
        static readonly long rawtimingLoopCount = 200000000L;
        /// <summary> 20,000,000 or 1/10th the regular count </summary>
        static readonly long synchronizedTimingLoopCount = 20000000L;
        /// <summary> 80,000,000 or 1/5th the regular count </summary>
        static readonly long atomicTimingLoopCount = 80000000L;

        void recordLoopWithExpectedInterval(AbstractHistogram histogram, long loopCount, long expectedInterval) 
        {
            for (long i = 0; i < loopCount; i++)
                histogram.recordValueWithExpectedInterval(testValueLevel + (i & 0x8000), expectedInterval);
        }

        long LeadingZerosSpeedLoop(long loopCount) 
        {
            long sum = 0;
            for (long i = 0; i < loopCount; i++) 
            {
                // long val = testValueLevel + (i & 0x8000);
                long val = testValueLevel;
                sum += MiscUtilities.numberOfLeadingZeros(val);
                sum += MiscUtilities.numberOfLeadingZeros(val);
                sum += MiscUtilities.numberOfLeadingZeros(val);
                sum += MiscUtilities.numberOfLeadingZeros(val);
                sum += MiscUtilities.numberOfLeadingZeros(val);
                sum += MiscUtilities.numberOfLeadingZeros(val);
                sum += MiscUtilities.numberOfLeadingZeros(val);                
                sum += MiscUtilities.numberOfLeadingZeros(val);
            }
            return sum;
        }

        private void testRawRecordingSpeedAtExpectedInterval(String label, AbstractHistogram histogram,
                                                            long expectedInterval, long timingLoopCount, 
                                                            bool assertNoGC = true, bool multiThreaded = false)  
        {
            Console.WriteLine("\nTiming recording speed with expectedInterval = " + expectedInterval + " :");
            // Warm up:
            var timer = Stopwatch.StartNew();
            recordLoopWithExpectedInterval(histogram, warmupLoopLength, expectedInterval);
            timer.Stop();
            // 1 millisecond (ms) = 1000 microsoecond (µs or usec)
            // 1 microsecond (µs or usec) = 1000 nanosecond (ns or nsec)
            long deltaUsec = timer.ElapsedMilliseconds * 1000L;
            long rate = 1000000 * warmupLoopLength / deltaUsec;
            Console.WriteLine("{0}Warmup:\n{1:N0} value recordings completed in {2:N0} usec, rate = {3:N0} value recording calls per sec.",
                                label, warmupLoopLength, deltaUsec, rate);
            histogram.reset();
            // Wait a bit to make sure compiler had a chance to do it's stuff:
            try 
            {
                Thread.Sleep(1000);
            } 
            catch (Exception) 
            {
            }

            var gcBefore = PrintGCAndMemoryStats("GC Before");
            timer = Stopwatch.StartNew();
            recordLoopWithExpectedInterval(histogram, timingLoopCount, expectedInterval);
            timer.Stop();
            var gcAfter = PrintGCAndMemoryStats("GC After ");
            deltaUsec = timer.ElapsedMilliseconds * 1000L;
            rate = 1000000 * timingLoopCount / deltaUsec;

            Console.WriteLine(label + "Hot code timing:");
            Console.WriteLine("{0}{1:N0} value recordings completed in {2:N0} usec, rate = {3:N0} value recording calls per sec.",
                                label, timingLoopCount, deltaUsec, rate);
            if (multiThreaded == false)
            {
                rate = 1000000 * histogram.getHistogramData().getTotalCount() / deltaUsec;
                Console.WriteLine("{0}{1:N0} raw recorded entries completed in {2:N0} usec, rate = {3:N0} recorded values per sec.",
                                    label, histogram.getHistogramData().getTotalCount(), deltaUsec, rate);
            }

            if (assertNoGC)
            {
                //// TODO work out why we always seems to get at least 1 GC here, maybe it's due to the length of the test run??
                //Assert.LessOrEqual(gcAfter.Item1 - gcBefore.Item1, 1, "There should be at MOST 1 Gen1 GC Collections");
                //Assert.LessOrEqual(gcAfter.Item2 - gcBefore.Item2, 1, "There should be at MOST 1 Gen2 GC Collections");
                //Assert.LessOrEqual(gcAfter.Item3 - gcBefore.Item3, 1, "There should be at MOST 1 Gen3 GC Collections");

                // TODO work out why we always seems to get at least 1 GC here, maybe it's due to the length of the test run??
                Assert.AreEqual(0, gcAfter.Item1 - gcBefore.Item1, "There should be NO Gen1 GC Collections");
                Assert.AreEqual(0, gcAfter.Item2 - gcBefore.Item2, "There should be NO Gen2 GC Collections");
                Assert.AreEqual(0, gcAfter.Item3 - gcBefore.Item3, "There should be NO Gen3 GC Collections");
            }
        }

        [Test]
        public void testRawRecordingSpeed()  
        {
            AbstractHistogram histogram;
            histogram = new Histogram(highestTrackableValue, numberOfSignificantValueDigits);
            Console.WriteLine("\n\nTiming Histogram:");
            testRawRecordingSpeedAtExpectedInterval("Histogram: ", histogram, 1000000000, rawtimingLoopCount);

            // Check that the histogram contains as many values are we wrote to it
            Assert.AreEqual(rawtimingLoopCount, histogram.getHistogramData().getTotalCount());
        }

        [Test]
        public void testRawSyncronizedRecordingSpeed()  
        {
            AbstractHistogram histogram;
            histogram = new SynchronizedHistogram(highestTrackableValue, numberOfSignificantValueDigits);
            Console.WriteLine("\n\nTiming SynchronizedHistogram:");
            testRawRecordingSpeedAtExpectedInterval("SynchronizedHistogram: ", histogram, 1000000000, synchronizedTimingLoopCount);

            // Check that the histogram contains as many values are we wrote to it
            Assert.AreEqual(synchronizedTimingLoopCount, histogram.getHistogramData().getTotalCount());
        }

        [Test]
        public void testRawSyncronizedRecordingSpeedMultithreaded()
        {
            AbstractHistogram histogram;
            histogram = new SynchronizedHistogram(highestTrackableValue, numberOfSignificantValueDigits);
            Console.WriteLine("\n\nTiming SynchronizedHistogram - Multithreaded:");

            var task1 = Task.Factory.StartNew(() =>
                testRawRecordingSpeedAtExpectedInterval("SynchronizedHistogram: ", histogram, 1000000000, synchronizedTimingLoopCount, assertNoGC: false, multiThreaded: true));
            var task2 = Task.Factory.StartNew(() =>
                testRawRecordingSpeedAtExpectedInterval("SynchronizedHistogram: ", histogram, 1000000000, synchronizedTimingLoopCount, assertNoGC: false, multiThreaded: true));
            var task3 = Task.Factory.StartNew(() =>
                testRawRecordingSpeedAtExpectedInterval("SynchronizedHistogram: ", histogram, 1000000000, synchronizedTimingLoopCount, assertNoGC: false, multiThreaded: true));

            Task.WaitAll(task1, task2, task3);

            // Check that the histogram contains as many values are we wrote to it
            Assert.AreEqual(synchronizedTimingLoopCount * 3L, histogram.getHistogramData().getTotalCount());
        }

        [Test]
        public void testRawAtomicRecordingSpeed()
        {
            AbstractHistogram histogram;
            histogram = new AtomicHistogram(highestTrackableValue, numberOfSignificantValueDigits);
            Console.WriteLine("\n\nTiming AtomicHistogram:");
            testRawRecordingSpeedAtExpectedInterval("AtomicHistogram: ", histogram, 1000000000, atomicTimingLoopCount);

            // Check that the histogram contains as many values are we wrote to it
            Assert.AreEqual(atomicTimingLoopCount, histogram.getHistogramData().getTotalCount());
        }

        [Test]
        public void testRawAtomicRecordingSpeedMultithreaded()
        {
            AbstractHistogram histogram;
            histogram = new AtomicHistogram(highestTrackableValue, numberOfSignificantValueDigits);
            Console.WriteLine("\n\nTiming AtomicHistogram - Multithreaded:");

            var task1 = Task.Factory.StartNew(() => 
                testRawRecordingSpeedAtExpectedInterval("AtomicHistogram: ", histogram, 1000000000, atomicTimingLoopCount, assertNoGC: false, multiThreaded: true));
            var task2 = Task.Factory.StartNew(() =>
                testRawRecordingSpeedAtExpectedInterval("AtomicHistogram: ", histogram, 1000000000, atomicTimingLoopCount, assertNoGC: false, multiThreaded: true));
            var task3 = Task.Factory.StartNew(() =>
                testRawRecordingSpeedAtExpectedInterval("AtomicHistogram: ", histogram, 1000000000, atomicTimingLoopCount, assertNoGC: false, multiThreaded: true));

            Task.WaitAll(task1, task2, task3);

            // Check that the histogram contains as many values are we wrote to it
            Assert.AreEqual(atomicTimingLoopCount * 3L, histogram.getHistogramData().getTotalCount());
        }

        [Test]
        public void testLeadingZerosSpeed()  
        {
            var callPerMethodCall = 8;
            Console.WriteLine("\nTiming LeadingZerosSpeed :");
            var timer = Stopwatch.StartNew();
            LeadingZerosSpeedLoop(warmupLoopLength);
            timer.Stop();
            // 1 millisecond (ms) = 1000 microsoecond (µs or usec)
            // 1 microsecond (µs or usec) = 1000 nanosecond (ns or nsec)
            long deltaUsec = timer.ElapsedMilliseconds * 1000L;
            long rate = 1000000 * warmupLoopLength * callPerMethodCall / deltaUsec;
            Console.WriteLine("Warmup:\n{0:N0} Leading Zero loops completed in {1:N0} usec, rate = {2:N0} value recording calls per sec.",
                                warmupLoopLength * callPerMethodCall, deltaUsec, rate);
            // Wait a bit to make sure compiler had a chance to do it's stuff:
            try 
            {
                Thread.Sleep(1000);
            }
            catch (Exception)
            {
            }

            var gcBefore = PrintGCAndMemoryStats("GC Before");
            var loopCount = rawtimingLoopCount / 10;
            timer = Stopwatch.StartNew();
            LeadingZerosSpeedLoop(loopCount);
            timer.Stop();
            var gcAfter = PrintGCAndMemoryStats("GC After ");
            deltaUsec = timer.ElapsedMilliseconds * 1000L;
            rate = 1000000 * loopCount * callPerMethodCall / deltaUsec;

            Console.WriteLine("Hot code timing:");
            Console.WriteLine("{0:N0} leading Zero loops completed in {1:N0} usec, rate = {2:N0} value recording calls per sec.",
                                loopCount * callPerMethodCall, deltaUsec, rate);

            // TODO work out why we always seems to get at least 1 GC here, maybe it's due to the length of the test run??
            Assert.LessOrEqual(gcAfter.Item1 - gcBefore.Item1, 1, "There should be at MOST 1 Gen1 GC Collections");
            Assert.LessOrEqual(gcAfter.Item2 - gcBefore.Item2, 1, "There should be at MOST 1 Gen2 GC Collections");
            Assert.LessOrEqual(gcAfter.Item3 - gcBefore.Item3, 1, "There should be at MOST 1 Gen3 GC Collections");
        }

        private Tuple<int, int, int> PrintGCAndMemoryStats(string label)
        {
            var bytesUsed = GC.GetTotalMemory(forceFullCollection: false);
            var gen1 = GC.CollectionCount(0);
            var gen2 = GC.CollectionCount(1);
            var gen3 = GC.CollectionCount(2);
            Console.WriteLine("{0}: {1:0.00} MB ({2:N0} bytes), Gen0 {3}, Gen1 {4}, Gen2 {5}",
                                label, bytesUsed / 1024.0 / 1024.0, bytesUsed, gen1, gen2, gen3);

            return Tuple.Create(gen1, gen2, gen3);
        }

        public static void main(String[] args) 
        {
            try 
            {
                HistogramPerfTest test = new HistogramPerfTest();
                test.testLeadingZerosSpeed();
                Thread.Sleep(1000000);
            } 
            catch (Exception e) 
            {
                Console.WriteLine("Exception: " + e);
            }
        }
    }
}
