using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        //static StreamWriter sr = new StreamWriter("log.txt");
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime: 
            DateTime localDateNow = DateTime.Now;
            //message:
            string[] textToBeInFile = { ex.Message,localDateNow.ToString("en-GB") };
            // for each exception write its details associated with datetime 
            File.WriteAllLines("log.txt", textToBeInFile);
          
        }

       
    }
}
