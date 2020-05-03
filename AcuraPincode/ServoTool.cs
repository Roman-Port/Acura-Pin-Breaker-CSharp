using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AcuraPincode
{
    public class ServoTool
    {
        public static void MoveServo(int pos)
        {
            //PLACEHOLDER
            Console.WriteLine("PLACEHOLDER; Servo moved to " + pos);
        }

        public static void PressButton(int time = 500)
        {
            MoveServo(10);
            Thread.Sleep(time);
            MoveServo(0);
        }
    }
}
