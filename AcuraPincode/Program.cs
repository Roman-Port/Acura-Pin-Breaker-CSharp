using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        }

        public static int pinIndex;
        public static string[] codes;

        static void Init()
        {
            //Load sound files
            PinVoiceTools.Init();

            //Load pin codes
            codes = File.ReadAllLines("Assets\\codes.txt");

            //Load the current saved index, if any
            if (File.Exists("Session\\location.txt"))
                pinIndex = int.Parse(File.ReadAllText("Session\\location.txt"));
            else
                pinIndex = 0;
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
            Thread.Sleep(100);

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
            Thread.Sleep(200);

            //Speak code
            PinVoiceTools.PlayPinCode(codes[pinIndex]);

            //Delay
            Thread.Sleep(1000);

            //Begin recording
            ResponseRecordTools.RecordResponse($"Session\\attempt_{pinIndex}.wav", 3000);

            //Add to current value and save
            pinIndex++;
            File.WriteAllText("Session\\location.txt", pinIndex.ToString());
        }
    }
}
