using System.Collections.ObjectModel;
using CoreAudio;
using CoreAudio.Interfaces;

namespace SoundMixerplugin.AudioManager {
    public class Mixer {
        public ObservableCollection<AudioSessionControl2> ListSession { get; } = new();

        private readonly MMDevice _device;
        public string InterfaceName { get; }
        public int SessionCount { get; }
        
        //If the 2 or more channels are not align, then the value is the Highest volume across the channels!
        public AudioEndpointVolume MasterVolume { get; }

        public Mixer() {
            // Take MMDevice
            MMDeviceEnumerator devEnum = new MMDeviceEnumerator(Guid.NewGuid());
            _device = devEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            MasterVolume = _device.AudioEndpointVolume;
            InterfaceName = _device.DeviceInterfaceFriendlyName;
            var sessions = _device?.AudioSessionManager2!.Sessions;
            SessionCount = sessions!.Count;


            foreach (var endpoint in sessions!) {
                endpoint.OnStateChanged += HandleStateChanges;
                ListSession.Add(endpoint);
            }

            //Attach Action to Add new AudioSession in the list!
            _device.AudioSessionManager2!.OnSessionCreated += HandleSessionCreated;
        }

        #region Handlers

        private void HandleSessionCreated(object sender, IAudioSessionControl2 newSession) {
            try {
                var asm = (AudioSessionManager2) sender;
                newSession.GetProcessId(out uint newSessionId);
                asm.RefreshSessions();
                var processSession = asm.Sessions!.First(x => x.ProcessID == newSessionId);
                ListSession.Add(processSession);
            }
            catch (InvalidOperationException) {
                //log the error. Shouldn't be anytime empty...
                return;
            }
        }

        private void HandleStateChanges(object sender, AudioSessionState state) {
            if (state == AudioSessionState.AudioSessionStateExpired)
                ListSession.Remove((AudioSessionControl2) sender);
        }

        #endregion
    }
}