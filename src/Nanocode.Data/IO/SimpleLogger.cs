using System;
using System.IO;
using System.Text;

namespace Nanocode.Data.IO
{
    public enum GizzaLogLevel
    {
        TRACE,
        INFO,
        DEBUG,
        WARNING,
        ERROR,
        FATAL
    }

    public class SimpleLogger
    {
        public string LogDirectory { get; set; }
        public string LogFileName { get; set; }
        public int ModulePad { get; set; }

        /// <summary>
        /// Initiate an instance of SimpleLogger class constructor.
        /// If log file does not exist, it will be created automatically.
        /// </summary>
        public SimpleLogger(string logDirectory, string logFilename, int modulePad = 20)
        {
            this.LogDirectory = logDirectory;
            this.LogFileName = logFilename;
            this.ModulePad = modulePad;
        }

        /// <summary>
        /// Log a DEBUG message
        /// </summary>
        /// <param name="text">Message</param>
        public void Debug(string logModule, string logContents)
        {
            WriteLog(GizzaLogLevel.DEBUG, logModule, logContents);
        }

        /// <summary>
        /// Log an ERROR message
        /// </summary>
        /// <param name="text">Message</param>
        public void Error(string logModule, string logContents)
        {
            WriteLog(GizzaLogLevel.ERROR, logModule, logContents);
        }

        /// <summary>
        /// Log a FATAL ERROR message
        /// </summary>
        /// <param name="text">Message</param>
        public void Fatal(string logModule, string logContents)
        {
            WriteLog(GizzaLogLevel.FATAL, logModule, logContents);
        }

        /// <summary>
        /// Log an INFO message
        /// </summary>
        /// <param name="text">Message</param>
        public void Info(string logModule, string logContents)
        {
            WriteLog(GizzaLogLevel.INFO, logModule, logContents);
        }

        /// <summary>
        /// Log a TRACE message
        /// </summary>
        /// <param name="text">Message</param>
        public void Trace(string logModule, string logContents)
        {
            WriteLog(GizzaLogLevel.TRACE, logModule, logContents);
        }

        /// <summary>
        /// Log a WARNING message
        /// </summary>
        /// <param name="text">Message</param>
        public void Warning(string logModule, string logContents)
        {
            WriteLog(GizzaLogLevel.WARNING, logModule, logContents);
        }

        public void WriteLog(GizzaLogLevel logLevel, string logModule, string logContents)
        {
            try
            {
                string pretext = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + logLevel.ToString().PadRight(8) + logModule.PadRight(this.ModulePad);
                string logFileName = this.LogDirectory + @"\" + this.LogFileName + " " + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                using (StreamWriter writer = new StreamWriter(logFileName, true, Encoding.UTF8))
                {
                    writer.WriteLine(pretext + " " + logContents);
                }
            }
            catch { }
        }
    }
}