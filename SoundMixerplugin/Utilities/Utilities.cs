
namespace SoundMixer {
    public static class Utilities {
        
        public static float DegreesToRadians(float degrees)
        {
            return (float)(degrees * (Math.PI / 180f));
        }
        public static string PathToResource(string file)
        {
            return Path.Combine("C:\\Users\\x-hal\\Desktop\\Programmi_Uni\\C#\\SoundMixerPlugin\\SoundMixerplugin", "Resources", file);
        }
        
    }
}
