using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcuraPincode
{
    public static class HttpStatsPage
    {
        public static void CreateTable(Stream s)
        {
            WriteString(s, "<html><head><title>AcuraPincode Crack</title><style>table, th, td { border: 1px solid black; }</style></head><body><table>");
            for(int i = 0; i<Program.pinIndex - 1; i++)
            {
                string b = $"<tr><td>{i}</td><td>{Program.codes[i]}</td><td><a href=\"/audio?index={i}\"><img src=\"/waveform?index={i}\"></a></td></tr>";
                WriteString(s, b);

            }
            WriteString(s, "</table></body>");
        }

        private static void WriteString(Stream s, string st)
        {
            byte[] buf = Encoding.UTF8.GetBytes(st);
            s.Write(buf, 0, buf.Length);
        }
    }
}
