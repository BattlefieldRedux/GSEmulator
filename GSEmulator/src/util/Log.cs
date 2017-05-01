using System;
using System.IO;


namespace GSEmulator.Util
{
    class Log
    {
        private static StreamWriter mLogginFile = new StreamWriter("GSEmulator.log", true);


        public static void d(string TAG, string message)
        {
            write(String.Format("D: {0} - {1}", TAG, message));
        }

        public static void w(string TAG, string message)
        {
            write(String.Format("W: {0} - {1}", TAG, message));
        }

        public static void e(string TAG, string message)
        {
            write(String.Format("E: {0} - {1}", TAG, message));
        }

        private static void write(string message)
        {

            try
            {
                mLogginFile.WriteLineAsync(message);
                mLogginFile.FlushAsync();
            }
            catch { }

            Console.WriteLine(message);

        }
    }
}
