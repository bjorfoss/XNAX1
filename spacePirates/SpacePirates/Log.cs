using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SpacePirates
{
    class Log
    {

        private Log log; //This is a singleton object

        ArrayList eventLog;

        static readonly object padlock = new Object();

        //Constructor
        private Log()
        {
            eventLog = new ArrayList();
        }

        /// <summary>
        /// Gets the singleton object, so it
        /// can be used.
        /// </summary>
        /// <returns></returns>
        public Log getLog()
        {
            lock (padlock)
            {
                if (log == null)
                {
                    log = new Log();
                }
                return log;
            }
        }

        /// <summary>
        /// Adds an event to the log. The event should be in String format, and
        /// should explain an event that just occured. Preferably, some variable
        /// values should be included, and the message should be short and consise.
        /// </summary>
        /// <param name="happening"></param>
        public void addEvent(String happening)
        {
            eventLog.Add(happening);
        }

    }
}
