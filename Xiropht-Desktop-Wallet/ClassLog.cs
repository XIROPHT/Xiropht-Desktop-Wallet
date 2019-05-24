using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Xiropht_Wallet
{
    public class Log
    {
        private static StreamWriter _writerLog; // StreamWriter for write logs.
        public static List<string> ListLog = new List<string>(); // List of log lines.
        private static int maxLogList = 10; // Minimum lines required for write them to the log file.

        /// <summary>
        /// Initialization of StreamWriter for the log file.
        /// </summary>
        public static void InitializeLog()
        {
            _writerLog = new StreamWriter(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory+"\\wallet.log"))
            {
                AutoFlush = true
            };
        }

        /// <summary>
        /// Include the current datetime on logs.
        /// </summary>
        /// <param name="log"></param>
        public static void WriteLine(string log)
        {

            Console.WriteLine(DateTime.Now + @" - " + log);
            try
            {
                ListLog.Add(DateTime.Now + " - " + log);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Automaticaly write log line once the list of logs reach the minimum of lines.
        /// </summary>
        public static void AutoWriteLog()
        {
            new Thread(async delegate()
            {
                while (true)
                {
                    try
                    {
                        if (ListLog.Count > 0 && ListLog.Count >= maxLogList)
                        {
                            for (int i = 0; i < ListLog.Count; i++)
                            {
                                if (i < ListLog.Count)
                                {
                                    try
                                    {
                                        await _writerLog.WriteLineAsync(ListLog[i]).ConfigureAwait(false);
                                        await _writerLog.FlushAsync().ConfigureAwait(false);
                                    }
                                    catch
                                    {
                                        i = ListLog.Count;
                                    }
                                }
                            }

                            ListLog.Clear();
                        }
                    }
                    catch
                    {
                        ListLog.Clear();
                    }

                    Thread.Sleep(5000);
                }
            }).Start();
        }
    }
}