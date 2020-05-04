using NAudio.Wave;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AcuraPincode
{
    public static class ResponseRecordTools
    {
        public const int SAMPLE_RATE = 44100;
        public const byte BITS_PER_SAMPLE = 16;
        public const int CHANNELS = 1;

        public const int PIXELS_PER_SECOND = 10;
        public const int IMG_HEIGHT = 100;
        public const int IMG_RES = 256;

        public static void RecordResponse(string filename, int time)
        {
            //Create peaks buffer
            List<short> peaks = new List<short>();

            //Record audio
            using (FileStream fs = new FileStream(filename + ".wav", FileMode.Create))
            {
                //Allocate 44 bytes at the beginning for use later
                byte[] buffer = new byte[44];
                fs.Write(buffer, 0, 44);
                
                //Begin recording
                var waveSource = new WaveInEvent();
                waveSource.WaveFormat = new WaveFormat(SAMPLE_RATE, CHANNELS);
                waveSource.DataAvailable += new EventHandler<WaveInEventArgs>((object sender, WaveInEventArgs e) =>
                {
                    //Write to buffer
                    fs.Write(e.Buffer, 0, e.BytesRecorded);

                    //Get peaks
                    short avg = 0;
                    int pos = 0;
                    for(int i = 0; i<e.BytesRecorded; i+= 2)
                    {
                        avg = Math.Max(BitConverter.ToInt16(e.Buffer, i), avg);
                        pos++;
                        if(pos > IMG_RES)
                        {
                            pos = 0;
                            peaks.Add(avg);
                            avg = 0;
                        }
                    }
                });
                waveSource.StartRecording();
                Thread.Sleep(time);
                waveSource.StopRecording();

                //Write the wav header
                WriteTag(buffer, 0, "RIFF");
                WriteUnsignedInt(buffer, 4, (uint)fs.Length - 8);
                WriteTag(buffer, 8, "WAVE");
                WriteTag(buffer, 12, "fmt ");
                WriteUnsignedInt(buffer, 16, 16U);
                WriteUnsignedShort(buffer, 20, 1);
                WriteUnsignedShort(buffer, 22, CHANNELS);
                WriteUnsignedInt(buffer, 24, SAMPLE_RATE);
                WriteUnsignedInt(buffer, 28, SAMPLE_RATE * (CHANNELS * (BITS_PER_SAMPLE / 8)));
                WriteUnsignedShort(buffer, 32, CHANNELS * (BITS_PER_SAMPLE / 8));
                WriteUnsignedShort(buffer, 34, BITS_PER_SAMPLE);
                WriteTag(buffer, 36, "data");
                WriteUnsignedInt(buffer, 40, (uint)fs.Length - 44);
                fs.Position = 0;
                fs.Write(buffer, 0, 44);
            }

            //Create waveform
            using (Image<Rgba32> img = new Image<Rgba32>(peaks.Count, IMG_HEIGHT))
            {
                for (int i = 0; i < peaks.Count; i += 1)
                {
                    int len = peaks[i] / (ushort.MaxValue / (IMG_HEIGHT / 2));
                    for (int y = 0; y < len; y++)
                    {
                        img[i, y + (IMG_HEIGHT / 2)] = new Rgba32(1, 1, 1);
                        img[i, (IMG_HEIGHT / 2) - y] = new Rgba32(1, 1, 1);
                    }
                }

                using (FileStream fs = new FileStream(filename + ".png", FileMode.Create))
                    img.SaveAsPng(fs);
            }
        }

        static void WriteTag(byte[] buffer, int pos, string tag)
        {
            Encoding.ASCII.GetBytes(tag).CopyTo(buffer, pos);
        }

        static void WriteUnsignedInt(byte[] buffer, int pos, uint value)
        {
            BitConverter.GetBytes(value).CopyTo(buffer, pos);
        }

        static void WriteUnsignedShort(byte[] buffer, int pos, ushort value)
        {
            BitConverter.GetBytes(value).CopyTo(buffer, pos);
        }
    }
}
