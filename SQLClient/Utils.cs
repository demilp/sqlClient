using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

    public class Utils
    {
        public delegate void LogMessageDelegate(string pMessage);
        static public event LogMessageDelegate OnLogMessage;
        static public event LogMessageDelegate OnLogMessageInFile;

        static public void LogMessage(string pMessage)
        {
            LogMessage(pMessage, true);
        }

        static public void LogMessage(string pMessage,bool pInFile)
        {
            Console.WriteLine(pMessage);

            if (pInFile)
            {
                if (OnLogMessageInFile != null)
                    OnLogMessageInFile(pMessage);
            }
            else
            {
                if (OnLogMessage != null)
                    OnLogMessage(pMessage);
            }
        }

        static public String Today
        {
            get            
            {
                return ("#" + DateTime.Now.Month + "/" + DateTime.Now.Day + "/" + DateTime.Now.Year + '#');
            }
        }
        static public String Now
        {
            get
            {
                return ("#" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + '#');
            }
        }
    }
