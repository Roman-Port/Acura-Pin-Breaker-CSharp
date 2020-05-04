using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Speech.Recognition;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AcuraPincode
{
    class Program
    {
        static void Main(string[] args)
        {
            Run();
            //RunTest();
        }

        public static int pinIndex;
        public static string[] codes;
        public static HttpListener listener;

        static void RunTest()
        {
            Init();
            Console.ReadLine();
            //ResponseRecordTools.RecordResponse($"Session\\attempt_{pinIndex}", 5000);
        }

        static void Init()
        {
            //Load sound files
            PinVoiceTools.Init();

            //Set servo
            ServoTool.Init();

            //Load pin codes
            codes = File.ReadAllLines("Assets\\codes.txt");

            //Load the current saved index, if any
            if (File.Exists("Session\\location.txt"))
                pinIndex = int.Parse(File.ReadAllText("Session\\location.txt"));
            else
                pinIndex = 0;

            //Start HTTP server
            listener = new HttpListener();
            listener.Prefixes.Add("http://10.0.1.12:80/");
            listener.Start();
            listener.BeginGetContext(OnGetRequst, null);
        }

        public static void OnGetRequst(IAsyncResult ar)
        {
            //Get ctx
            var ctx = listener.EndGetContext(ar);
            HttpListenerRequest req = ctx.Request;
            HttpListenerResponse resp = ctx.Response;

            //Handle
            if(req.Url.AbsolutePath == "/audio")
            {
                resp.ContentType = "audio/wav";
                using (FileStream fs = new FileStream($"Session\\attempt_{req.QueryString["index"]}.wav", FileMode.Open, FileAccess.Read))
                    fs.CopyTo(resp.OutputStream);
            }
            if (req.Url.AbsolutePath == "/waveform")
            {
                resp.ContentType = "image/png";
                using (FileStream fs = new FileStream($"Session\\attempt_{req.QueryString["index"]}.png", FileMode.Open, FileAccess.Read))
                    fs.CopyTo(resp.OutputStream);
            }
            if(req.Url.AbsolutePath == "/")
            {
                HttpStatsPage.CreateTable(resp.OutputStream);
            }

            resp.Close();

            //Run next
            listener.BeginGetContext(OnGetRequst, null);
        }

        static void Run()
        {
            //Init all
            Init();

            //Run the cycle
            while(pinIndex + 3 < codes.Length)
            {
                RunCycle();
            }
        }

        static void RunCycle()
        {
            //Press button to start
            ServoTool.PressButton();

            //Delay a small amount
            Thread.Sleep(1000);

            //Run attempts
            RunAttempt();
            RunAttempt();
            RunAttempt();

            //Wait a little bit at the end
            Thread.Sleep(300);
        }

        static void RunAttempt()
        {
            //Log
            Console.WriteLine($"[ATTEMPT] Attempting {codes[pinIndex]}, index {pinIndex}");

            //Push button
            ServoTool.PressButton();

            //Delay
            Thread.Sleep(600);

            //Speak code
            PinVoiceTools.PlayPinCode(codes[pinIndex]);

            //Delay
            Thread.Sleep(1000);

            //Begin recording
            ResponseRecordTools.RecordResponse($"Session\\attempt_{pinIndex}", 5000);

            //Add to current value and save
            pinIndex++;
            File.WriteAllText("Session\\location.txt", pinIndex.ToString());
        }
    }
}
