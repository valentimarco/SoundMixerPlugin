using System.Numerics;
using OVRSharp;
using OVRSharp.Math;
using Valve.VR;

namespace SoundMixer {
    public class OverlayVR : Application {
        private Overlay _overlay;
        private string _keyoverlay = "soundmixer";
        private IntPtr _OverlayTexture;
        public OverlayVR(string? debug = null) : base(ApplicationType.Overlay) {
            _overlay = new Overlay(_keyoverlay, "SoundMixer", true);
            _overlay.WidthInMeters = 2f;
            _overlay.SetTexture(new Texture_t {
                eColorSpace = EColorSpace.Auto,
                eType = ETextureType.Vulkan,
                handle = _OverlayTexture
            });
            OpenVR.Overlay.SetOverlayInputMethod(_overlay.Handle, VROverlayInputMethod.Mouse);
            _overlay.StartPolling();


        }


        ~OverlayVR() {
            _overlay.Destroy();
            GC.SuppressFinalize(this);
        }

        


    }
}