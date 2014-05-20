﻿/**
 * Original version written by Gil Tene of Azul Systems, and released to the public domain,
 * as explained at http://creativecommons.org/publicdomain/zero/1.0/
 *
 * @author Gil Tene
 * 
 * This is a .NET port of the original Java version, .NET port by Matt Warren
 */

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace HdrHistogram.NET.Examples
{
    /**
     * A simple example of using HdrHistogram: run for 20 seconds collecting the
     * time it takes to perform a simple Datagram Socket create/close operation,
     * and report a histogram of the times at the end.
     */
    public class SimpleHistogramExample 
    {
        // A Histogram covering the range from 1 nsec to 1 hour (3,600,000,000,000 ns) with 3 decimal point resolution:
        static readonly Histogram histogram = new Histogram(3600000000000L, 3);

        static public volatile Socket socket;

        static long WARMUP_TIME_MSEC = 5000;
        static long RUN_TIME_MSEC = 20000;

        static void recordTimeToCreateAndCloseDatagramSocket() 
        {
            var hostIPAddress = Dns.GetHostEntry("google.com").AddressList[0];
            var hostIPEndPoint = new IPEndPoint(hostIPAddress, 80);

            var timer = Stopwatch.StartNew();
            try 
            {
                socket = new Socket(hostIPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            } 
            catch (SocketException ex) 
            {
            } 
            finally 
            {
                socket.Close();
            }
            timer.Stop();

            // From http://stackoverflow.com/questions/2329079/how-do-you-convert-stopwatch-ticks-to-nanoseconds-milliseconds-and-seconds/2329103#2329103
            //histogram.recordValue(endTime - startTime);
            // 1 msecs = 1000000 (1,000,000) ns (or nanos)
            // 1 msecs = 1000 (1,000) usec (or microseconds)
            // 1 usec = 1000 ns (nanos)
            long elapsedNanos = (long)(((double)timer.ElapsedTicks / Stopwatch.Frequency) * 1000000000);
            histogram.recordValue(elapsedNanos);
        }

        public static void Run()
        {
            var timer = Stopwatch.StartNew();

            do {
                recordTimeToCreateAndCloseDatagramSocket();
            } while (timer.ElapsedMilliseconds < WARMUP_TIME_MSEC);

            histogram.reset();

            do {
                recordTimeToCreateAndCloseDatagramSocket();
            } while (timer.ElapsedMilliseconds < RUN_TIME_MSEC);

            Console.WriteLine("Recorded latencies [in usec] for Create+Close of a DatagramSocket:");

            var data = histogram.getHistogramData();
            var size = histogram.getEstimatedFootprintInBytes();
            Console.WriteLine("Histogram size = {0} bytes ({1:F2} MB)", size, size / 1024.0 / 1024.0);

            // 1 usec = 1000 ns (nanos), results are displayed in usecs, so we need to scale
            data.outputPercentileDistribution(Console.Out, outputValueUnitScalingRatio: 1000.0);
            Console.WriteLine();
            data.outputPercentileDistribution(Console.Out, outputValueUnitScalingRatio: 1000.0 * 1000.0);
            //Console.WriteLine();
            //data.outputPercentileDistribution(
            //    Console.Out, percentileTicksPerHalfDistance: 5, outputValueUnitScalingRatio: 1000.0, useCsvFormat: true);
        }
    }
}
