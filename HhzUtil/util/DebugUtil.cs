using System;
using System.Collections.Generic;
using System.Text;

namespace HhzUtil.util
{
    using System.Diagnostics;
    /// <summary>
    /// debugging program needs to be built with #DEBUG defined
    /// #if DEBUG
    /// Debug.WriteLine("debug message");
    /// #endif
    /// </summary>
    public static class DebugUtil
    {
        /// <summary>
        /// Log to a console window
        /// </summary>
        static void LogToConsole()
        {
            Debug.Listeners.Add(new ConsoleTraceListener());

            // now use System.Net.FtpCLient as usual and the server transactions
            // will be written to the Console window.
        }

        /// <summary>
        /// Log to a text file
        /// </summary>
        static void LogToFile(string logfilename)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(logfilename));

            // now use System.Net.FtpCLient as usual and the server transactions
            // will be written to the Console window.
        }

        /// <summary>
        /// Custom trace listener class that can log the transaction
        /// however you want.
        /// </summary>
        class CustomTraceListener : TraceListener
        {
            public override void Write(string message)
            {
                Console.Write(message);
            }

            public override void WriteLine(string message)
            {
                Console.WriteLine(message);
            }
        }

        /// <summary>
        /// Log to a custom TraceListener
        /// </summary>
        static void LogToCustomListener()
        {
            Debug.Listeners.Add(new CustomTraceListener());
        }
    }
}
