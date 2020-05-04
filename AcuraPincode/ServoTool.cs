using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AcuraPincode
{
    public class ServoTool
    {
        public static SerialPort io;

        public static void Init()
        {
            io = new SerialPort();
            io.PortName = "COM3";
            io.BaudRate = 9600;
            io.Open();
        }

        public static void PressButton(int time = 500)
        {
            io.Write("P");
            Thread.Sleep(time);
            io.Write("O");
        }
    }
}
