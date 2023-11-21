using System;
using System.Diagnostics;
using System.Threading;

namespace CpuStressTest
{
    class Program
    {
        static void Main()
        {
            int numCores = Environment.ProcessorCount;
            int desiredCpuUsagePercentage = 20; 

            Console.WriteLine($"Stressing CPU stress test with {numCores} cores...");

            for (int i = 0; i < numCores; i++)
            {
                new Thread(() => StressThread(desiredCpuUsagePercentage)).Start();
            }
        }

        static void StressThread(int desiredCpuUsagePercentage)
        {
            Stopwatch stopwatch = new Stopwatch();

            while (true)
            {
                try
                {
                    stopwatch.Start();
                    while (stopwatch.ElapsedMilliseconds < (10 * desiredCpuUsagePercentage)) ;
                    stopwatch.Reset();
                    Thread.Sleep(10 * (100 - desiredCpuUsagePercentage));
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }
    }
}

