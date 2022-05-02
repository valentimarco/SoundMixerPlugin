using System.Numerics;
using OVRSharp;
using OVRSharp.Math;
using Valve.VR;

namespace SoundMixer {
    public class OverlayVR : Application {
        private Overlay _overlay;
        private string _keyoverlay = "soundmixer";
        private IntPtr _OverlayTexture; //TODO: creare Ui sfruttando vulkan
        private Window _window = new Window(1920,1080);
        public OverlayVR(string? debug = null) : base(ApplicationType.Overlay) {
            _overlay = new Overlay(_keyoverlay, "SoundMixer", true);
            _overlay.WidthInMeters = 2f;
            _overlay.SetTexture(new Texture_t {
                eColorSpace = EColorSpace.Auto,
                eType = ETextureType.OpenGL,
                handle = new IntPtr(_window.TextureId)
            });
            OpenVR.Overlay.SetOverlayInputMethod(_overlay.Handle, VROverlayInputMethod.Mouse);
            _overlay.StartPolling();


        }


        ~OverlayVR() {
            _overlay.Destroy();
        }

        


    }
}