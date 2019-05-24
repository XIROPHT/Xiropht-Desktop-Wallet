using System;
using System.Threading;

namespace Xiropht_Wallet
{

    /// <summary>
    /// This is optional.
    /// </summary>
    public class ClassMemory
    {
        public static void CleanMemory()
        {
            new Thread(Start).Start();
        }

        private static void Start()
        {
            while (true)
            {
                long memory = GC.GetTotalMemory(false);
                double megabyte = ConvertBytesToMegabytes(memory);

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                Log.WriteLine("Clean memory done. Total Memory Cleaned: " + megabyte + " MB");
                Thread.Sleep(60000);
            }
        }

        static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
    }
}