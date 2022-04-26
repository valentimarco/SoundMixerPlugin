using System.Diagnostics;

namespace SoundMixer{
    class SoundMixer{
        public static void Main(string[] args){
            IDictionary<string,int> devices = AudioManager.GetAllVolumeObjects();
            Console.WriteLine("Process with sound Active");
            foreach (var process in devices) {
                    Console.Write("Process name: " + process.Key);
                    Console.Write(",Volume:" + AudioManager.GetApplicationVolume(process.Value));
                    Console.Write(" ,Mute? " + AudioManager.GetApplicationMute(process.Value));
                    Console.Write("\n");
            }
                
            
            
            
            
            
            
            
            
        }



       
    }
}



