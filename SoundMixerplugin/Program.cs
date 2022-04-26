using System.Diagnostics;

namespace SoundMixer{
    class SoundMixer{
        public static void Main(string[] args){
            IDictionary<string,int> devices = AudioManager.GetAllVolumeObjects();
            
            
            foreach (var process in devices) {
                Console.Write("Process name:" + process.Key);
                Console.Write(", Volume:" + AudioManager.GetApplicationVolume(process.Value));
                Console.Write("\n");
            }
            
            
        }



       
    }
}



