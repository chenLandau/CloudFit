using System;
using System.Threading;

namespace MemoryStressTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Memory stress test");

            int numCores = Environment.ProcessorCount;
            double memoryInGB = double.Parse(args[0]);
            long bytesPerThread = (long)((memoryInGB * Math.Pow(2, 30)) / (2 * numCores)); // total 0.5 of memory

            for (int i = 0; i < numCores; i++)
            {
                new Thread(() => StressThread(bytesPerThread)).Start();
            }
        }

        static void StressThread(long bytesPerThread)
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[bytesPerThread];
                    while (true)
                    {
                        Thread.Sleep(8000);
                        for (int j = 0; j < buffer.Length; j++)
                        {
                            buffer[j]++;
                        }
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }
    }
}
