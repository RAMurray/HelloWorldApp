using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace HelloWorldConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("===================================");
            Console.WriteLine("--     Welcome to Hello.exe      --");
            Console.WriteLine("-- Select menu option from below --");
            Console.WriteLine("--      and press enter.         --");
            Console.WriteLine("=================================== \n");

            MainMenu();
        }

        struct Log
        {
            public string LogDate { get; set; }
            public string User { get; set; }
        }

        public static void MainMenu()
        {
            Console.WriteLine("[1] Print Hello World");
            Console.WriteLine("[2] View Log");
            Console.WriteLine("[x] To Exit");

            String sInput = Console.ReadLine();

            switch(sInput)
            {
                case "1":
                    Console.WriteLine("Hello World!");
                    string sDate = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");
                    Console.WriteLine(String.Format("This message was printed by {0} at {1}", Environment.UserName, DateTime.Now));
                    AddLog(sDate, Environment.UserName);
                    MainMenu();
                    break;
                case "2":
                    LoadLogs();
                    break;
                case "x":
                case "X":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid Entry");
                    break;
            }
        }

        public static void AddLog(string sDate, string sUser)
        {
            var oLog = JsonConvert.SerializeObject(new Log
            {
                LogDate = sDate,
                User = sUser

            });

            var oRequest = WebRequest.CreateHttp("https://helloworldapp-430d6.firebaseio.com/.json");
            oRequest.Method = "POST";
            oRequest.ContentType = "application/json";
            var oBuffer = Encoding.UTF8.GetBytes(oLog);
            oRequest.ContentLength = oBuffer.Length;
            oRequest.GetRequestStream().Write(oBuffer, 0, oBuffer.Length);
            var oResponse = oRequest.GetResponse();
            oLog = (new StreamReader(oResponse.GetResponseStream())).ReadToEnd();

            Console.WriteLine("Log has been added");
            AddSpace();
            
        }

        public static void LoadLogs()
        {
            string sURL = "https://helloworldapp-430d6.firebaseio.com/.json";
            string sOutput = "";
            HttpWebRequest oRequest = (HttpWebRequest)WebRequest.Create(sURL);
            oRequest.ContentType = "application/json: charset=utf-8";
            HttpWebResponse oResponse = oRequest.GetResponse() as HttpWebResponse;
            using (Stream oResponsestream = oResponse.GetResponseStream())
            {
                StreamReader oRead = new StreamReader(oResponsestream, Encoding.UTF8);
                sOutput = oRead.ReadLine();
            }

            //Console.WriteLine(sOutput);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(sOutput);
            List<Log> arrLogs = new List<Log>();
            foreach(var oLog in data)
            {
                arrLogs.Add(JsonConvert.DeserializeObject<Log>(((JProperty)oLog).Value.ToString()));
            }

            foreach(Log oLog in arrLogs)
            {
                Console.WriteLine(oLog.LogDate + ": " + oLog.User + " said hello to the world.");
            }

        }

        public static void AddSpace()
        {
            Console.WriteLine("\n \n \n");
            Console.WriteLine("===== MAIN MENU =====");
        }

    }
}
