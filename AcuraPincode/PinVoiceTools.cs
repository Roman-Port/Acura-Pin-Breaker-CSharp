using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AcuraPincode
{
    public static class PinVoiceTools
    {
        private static SoundPlayer[] sounds;

        public const int NUMBER_DELAY = 150;

        public static void Init()
        {
            sounds = new SoundPlayer[10];
            for (int i = 0; i < sounds.Length; i++) {
                sounds[i] = new SoundPlayer($"Assets\\audio\\{i}.wav");
                sounds[i].Load();
            }
        }
        
        public static void PlayPinCode(string code)
        {
            //Read each digit
            for(int i = 0; i<4; i++)
            {
                sounds[code[i] - 48].PlaySync();
                Thread.Sleep(NUMBER_DELAY);
            }
        }
    }
}
