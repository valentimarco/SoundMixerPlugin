using System.Diagnostics;
using Valve.VR;

namespace SoundMixer{
    class SoundMixer{
        
        public static void Main(string[] args){
            if (args.Length > 0)
            {
                if (args[0] == "--register")
                {
                    RegisterManifest();
                    return;
                }
                else if (args[0] == "--deregister")
                {
                    DeregisterManifest();
                    return;
                }
            }
            OverlayVR app = new OverlayVR();
            Console.ReadLine();
            app.Shutdown();
        }
        
        static void RegisterManifest()
        {
            EVRInitError initErr = EVRInitError.None;
            OpenVR.Init(ref initErr, EVRApplicationType.VRApplication_Utility);

            if (initErr != EVRInitError.None)
            {
                Console.WriteLine($"Error initializing OpenVR handle: {initErr}");
                Environment.Exit(1);
            }
            var err = OpenVR.Applications.AddApplicationManifest(Utilities.PathToResource("manifest.vrmanifest"), false);

            if (err != EVRApplicationError.None)
            {
                Console.WriteLine($"Error registering manifest with OpenVR runtime: {err}");
                Environment.Exit(1);
            }

            Console.WriteLine("Application manifest registered with OpenVR runtime!");

            OpenVR.Shutdown();
        }

        static void DeregisterManifest()
        {
            EVRInitError initErr = EVRInitError.None;
            OpenVR.Init(ref initErr, EVRApplicationType.VRApplication_Utility);

            if (initErr != EVRInitError.None)
            {
                Console.WriteLine($"Error initializing OpenVR handle: {initErr}");
                Environment.Exit(1);
            }

            var err = OpenVR.Applications.RemoveApplicationManifest(Utilities.PathToResource("manifest.vrmanifest"));

            if (err != EVRApplicationError.None)
            {
                Console.WriteLine($"Error de-registering manifest with OpenVR runtime: {err}");
                Environment.Exit(1);
            }

            Console.WriteLine("Application manifest de-registered with OpenVR runtime!");

            OpenVR.Shutdown();
        }
        


       
    }
}



