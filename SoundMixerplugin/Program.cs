using System;
using CoreAudio;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;


namespace SoundMixer{
    class SoundMixer{
        public static void Main(string[] args){
            IList<string> devices = AudioManager.GetAllVolumeObjects();
            foreach(var device in devices) Console.WriteLine(device);
        }



       
    }
}



