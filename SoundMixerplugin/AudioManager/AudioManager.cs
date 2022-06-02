using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SoundMixer{
    public static class AudioManager
    {
        #region Master Volume Manipulation

        /// <summary>
        /// Gets the current master volume in scalar values (percentage)
        /// </summary>
        /// <returns>-1 in case of an error, if successful the value will be between 0 and 100</returns>
        public static float GetMasterVolume()
        {
            IAudioEndpointVolume masterVol = null;
            try
            {
                masterVol = GetMasterVolumeObject();
                if (masterVol == null)
                    return -1;

                float volumeLevel;
                masterVol.GetMasterVolumeLevelScalar(out volumeLevel);
                return volumeLevel*100;
            }
            finally
            {
                if (masterVol != null)
                    Marshal.ReleaseComObject(masterVol);
            }
        }

        /// <summary>
        /// Gets the mute state of the master volume. 
        /// While the volume can be muted the <see cref="GetMasterVolume"/> will still return the pre-muted volume value.
        /// </summary>
        /// <returns>false if not muted, true if volume is muted</returns>
        public static bool GetMasterVolumeMute()
        {
            IAudioEndpointVolume masterVol = null;
            try
            {
                masterVol = GetMasterVolumeObject();
                if (masterVol == null)
                    return false;

                bool isMuted;
                masterVol.GetMute(out isMuted);
                return isMuted;
            }
            finally
            {
                if (masterVol != null)
                    Marshal.ReleaseComObject(masterVol);
            }
        }

        /// <summary>
        /// Sets the master volume to a specific level
        /// </summary>
        /// <param name="newLevel">Value between 0 and 100 indicating the desired scalar value of the volume</param>
        public static void SetMasterVolume(float newLevel)
        {
            IAudioEndpointVolume masterVol = null;
            try
            {
                masterVol = GetMasterVolumeObject();
                if (masterVol == null)
                    return;

                masterVol.SetMasterVolumeLevelScalar(newLevel/100, Guid.Empty);
            }
            finally
            {
                if (masterVol != null)
                    Marshal.ReleaseComObject(masterVol);
            }
        }

        /// <summary>
        /// Increments or decrements the current volume level by the <see cref="stepAmount"/>.
        /// </summary>
        /// <param name="stepAmount">Value between -100 and 100 indicating the desired step amount. Use negative numbers to decrease
        /// the volume and positive numbers to increase it.</param>
        /// <returns>the new volume level assigned</returns>
        public static float StepMasterVolume(float stepAmount)
        {
            IAudioEndpointVolume masterVol = null;
            try
            {
                masterVol = GetMasterVolumeObject();
                if (masterVol == null)
                    return -1;

                float stepAmountScaled = stepAmount/100;

                // Get the level
                float volumeLevel;
                masterVol.GetMasterVolumeLevelScalar(out volumeLevel);

                // Calculate the new level
                float newLevel = volumeLevel + stepAmountScaled;
                newLevel = Math.Min(1, newLevel);
                newLevel = Math.Max(0, newLevel);

                masterVol.SetMasterVolumeLevelScalar(newLevel, Guid.Empty);

                // Return the new volume level that was set
                return newLevel*100;
            }
            finally
            {
                if (masterVol != null)
                    Marshal.ReleaseComObject(masterVol);
            }
        }

        /// <summary>
        /// Mute or unmute the master volume
        /// </summary>
        /// <param name="isMuted">true to mute the master volume, false to unmute</param>
        public static void SetMasterVolumeMute(bool isMuted)
        {
            IAudioEndpointVolume masterVol = null;
            try
            {
                masterVol = GetMasterVolumeObject();
                if (masterVol == null)
                    return;

                masterVol.SetMute(isMuted, Guid.Empty);
            }
            finally
            {
                if (masterVol != null)
                    Marshal.ReleaseComObject(masterVol);
            }
        }

        /// <summary>
        /// Switches between the master volume mute states depending on the current state
        /// </summary>
        /// <returns>the current mute state, true if the volume was muted, false if unmuted</returns>
        public static bool ToggleMasterVolumeMute()
        {
            IAudioEndpointVolume masterVol = null;
            try
            {
                masterVol = GetMasterVolumeObject();
                if (masterVol == null)
                    return false;

                bool isMuted;
                masterVol.GetMute(out isMuted);
                masterVol.SetMute(!isMuted, Guid.Empty);

                return !isMuted;
            }
            finally
            {
                if (masterVol != null)
                    Marshal.ReleaseComObject(masterVol);
            }
        }

        private static IAudioEndpointVolume GetMasterVolumeObject()
        {
            IMMDeviceEnumerator deviceEnumerator = null;
            IMMDevice speakers = null;
            try
            {
                deviceEnumerator = (IMMDeviceEnumerator) (new MMDeviceEnumerator());
                deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out speakers);

                Guid IID_IAudioEndpointVolume = typeof (IAudioEndpointVolume).GUID;
                object o;
                speakers.Activate(ref IID_IAudioEndpointVolume, 0, IntPtr.Zero, out o);
                IAudioEndpointVolume masterVol = (IAudioEndpointVolume) o;

                return masterVol;
            }
            finally
            {
                if (speakers != null) Marshal.ReleaseComObject(speakers);
                if (deviceEnumerator != null) Marshal.ReleaseComObject(deviceEnumerator);
            }
        }

        #endregion
        
        #region Individual Application Volume Manipulation
        

        public static float? GetApplicationVolume(int pid)
        {
            ISimpleAudioVolume volume = GetVolumeObject(pid);
            if (volume == null)
                return null;

            float level;
            volume.GetMasterVolume(out level);
            Marshal.ReleaseComObject(volume);
            return level*100;
        }

        public static bool? GetApplicationMute(int pid)
        {
            ISimpleAudioVolume volume = GetVolumeObject(pid);
            if (volume == null)
                return null;

            bool mute;
            volume.GetMute(out mute);
            Marshal.ReleaseComObject(volume);
            return mute;
        }

        public static void SetApplicationVolume(int pid, float level)
        {
            ISimpleAudioVolume volume = GetVolumeObject(pid);
            if (volume == null)
                return;

            Guid guid = Guid.Empty;
            volume.SetMasterVolume(level/100, ref guid);
            Marshal.ReleaseComObject(volume);
        }

        public static void SetApplicationMute(int pid, bool mute)
        {
            ISimpleAudioVolume volume = GetVolumeObject(pid);
            if (volume == null)
                return;

            Guid guid = Guid.Empty;
            volume.SetMute(mute, ref guid);
            Marshal.ReleaseComObject(volume);
        }

        private static ISimpleAudioVolume GetVolumeObject(int pid)
        {
            IMMDeviceEnumerator deviceEnumerator = null;
            IAudioSessionEnumerator sessionEnumerator = null;
            IAudioSessionManager2 mgr = null;
            IMMDevice speakers = null;
            try
            {
                // get the speakers (1st render + multimedia) device
                deviceEnumerator = (IMMDeviceEnumerator) (new MMDeviceEnumerator());
                deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out speakers);

                // activate the session manager. we need the enumerator
                Guid IID_IAudioSessionManager2 = typeof (IAudioSessionManager2).GUID;
                object o;
                speakers.Activate(ref IID_IAudioSessionManager2, 0, IntPtr.Zero, out o);
                mgr = (IAudioSessionManager2) o;

                // enumerate sessions for on this device
                mgr.GetSessionEnumerator(out sessionEnumerator);
                int count;
                sessionEnumerator.GetCount(out count);

                // search for an audio session with the required process-id
                ISimpleAudioVolume volumeControl = null;
                for (int i = 0; i < count; ++i)
                {
                    IAudioSessionControl2 ctl = null;
                    try
                    {
                        sessionEnumerator.GetSession(i, out ctl);

                        // NOTE: we could also use the app name from ctl.GetDisplayName()
                        int cpid;
                        ctl.GetProcessId(out cpid);

                        if (cpid == pid)
                        {
                            volumeControl = ctl as ISimpleAudioVolume;
                            break;
                        }
                    }
                    //Don't know why remove this method i don't get an expetion...
                    finally
                    {
                        //if (ctl != null) Marshal.ReleaseComObject(ctl);
                    }
                }

                return volumeControl;
            }
            finally
            {
                if (sessionEnumerator != null) Marshal.ReleaseComObject(sessionEnumerator);
                if (mgr != null) Marshal.ReleaseComObject(mgr);
                if (speakers != null) Marshal.ReleaseComObject(speakers);
                if (deviceEnumerator != null) Marshal.ReleaseComObject(deviceEnumerator);
            }
        }
        #endregion

        #region Volume stuff
        
        public static IDictionary<string, int> GetAllVolumeObjectsFromDefualtInterface(){
            IMMDeviceEnumerator deviceEnumerator = null;
            IAudioSessionEnumerator sessionEnumerator = null;
            IAudioSessionManager2 mgr = null;
            IMMDevice speakers = null;
            try
            {
                // get the speakers (1st render + multimedia) device
                deviceEnumerator = (IMMDeviceEnumerator) (new MMDeviceEnumerator());
                deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out speakers);
    
                // activate the session manager. we need the enumerator
                Guid IID_IAudioSessionManager2 = typeof (IAudioSessionManager2).GUID;
                object o;
                speakers.Activate(ref IID_IAudioSessionManager2, 0, IntPtr.Zero, out o);
                mgr = (IAudioSessionManager2) o;
    
                // enumerate sessions for on this device
                
                mgr.GetSessionEnumerator(out sessionEnumerator);
                int count;
                sessionEnumerator.GetCount(out count);
    
                // search for an audio session with the required process-id
                IDictionary<string, int> process = new Dictionary<string, int>();
                for (int i = 0; i < count; ++i)
                {
                    IAudioSessionControl2 ctl = null;
                    try
                    {
                        sessionEnumerator.GetSession(i, out ctl);
                        int pid = 0;
                        ctl.GetProcessId(out pid);
                        string process_name = Process.GetProcessById(pid).ProcessName;
                        if(!process.ContainsKey(process_name)) 
                            process.Add(process_name,pid);
                        
                    }
                    finally
                    {
                        //if (ctl != null) Marshal.ReleaseComObject(ctl);
                    }
                }
    
                return process;
            }
            finally
            {
                if (sessionEnumerator != null) Marshal.ReleaseComObject(sessionEnumerator);
                if (mgr != null) Marshal.ReleaseComObject(mgr);
                if (speakers != null) Marshal.ReleaseComObject(speakers);
                if (deviceEnumerator != null) Marshal.ReleaseComObject(deviceEnumerator);
            }
    }
        
        
                  
        public static  IDictionary<string, IDictionary<string, int>> GetAudioEndpointsAndVolumeObjects(){
            IDictionary<string, IDictionary<string, int>> map = new Dictionary<string, IDictionary<string, int>>();
            
            IMMDeviceEnumerator deviceEnumerator = null;
            IAudioSessionEnumerator sessionEnumerator = null;
            IAudioSessionManager2 mgr = null;
            IMMDevice speakers = null;
            IMMDeviceCollection deviceCollection = null;
            IPropertyStore prop;
            PropVariant NameEndpoint;
            PropertyKey key = new PropertyKey() {
                fmtid = PKEY.PKEY_Device_FriendlyName,
                pid = 14 // https://stackoverflow.com/questions/32151133/how-to-get-pkey-device-friendlyname-if-its-not-defined
            };
            
            uint countCollecitionEndpoints = 0;
            int count,pid;
            object o;
            string process_name;
            
            try
            {
                // Get a Collection of AudioEndpoints
                deviceEnumerator = (IMMDeviceEnumerator) (new MMDeviceEnumerator());
                deviceEnumerator.EnumAudioEndpoints(EDataFlow.eRender, EDeviceState.DEVICE_STATE_ACTIVE,
                    out deviceCollection);
                deviceCollection.GetCount(out countCollecitionEndpoints);

                for (uint i = 0; i < countCollecitionEndpoints; i++) {
                    IDictionary<string, int> process = new Dictionary<string, int>();
                    string id;
                    deviceCollection.Item(i, out speakers);
                    //Name of AudioEndpoint
                    speakers.OpenPropertyStore(EStgmAccess.STGM_READ, out prop);
                    prop.GetValue(ref key, out NameEndpoint);
                    var nameinterfaceaudio = (string) NameEndpoint.Value;
                    
                    //Get a map(nameapp,pid) of app attatch to the i-th AudioEndpoint
                    Guid IID_IAudioSessionManager2 = typeof (IAudioSessionManager2).GUID;
                    speakers.Activate(ref IID_IAudioSessionManager2, 0, IntPtr.Zero, out o);
                    mgr = (IAudioSessionManager2) o;
                    mgr.GetSessionEnumerator(out sessionEnumerator);
                    sessionEnumerator.GetCount(out count);
                    for (int j = 0; j < count; ++j)
                    {
                        IAudioSessionControl2 ctl = null;
                        try
                        {
                            sessionEnumerator.GetSession(j, out ctl);
                            ctl.GetProcessId(out pid);
                             process_name = Process.GetProcessById(pid).ProcessName;
                            if(!process.ContainsKey(process_name)) 
                                process.Add(process_name,pid);
                            
                        }
                        finally
                        {
                            //if (ctl != null) Marshal.ReleaseComObject(ctl);
                        }
                    }
                    map.Add(nameinterfaceaudio,process);
                    
                }

                return map;


            }
            finally
            {
                
                if (deviceEnumerator != null) Marshal.ReleaseComObject(deviceEnumerator);
                if (sessionEnumerator != null) Marshal.ReleaseComObject(sessionEnumerator);
                if (mgr != null) Marshal.ReleaseComObject(mgr);
                if (speakers != null) Marshal.ReleaseComObject(speakers);
            }
    }
        public static string GetDefaultAudioEndpointsName(){

            IMMDeviceEnumerator deviceEnumerator = null;
            IAudioSessionEnumerator sessionEnumerator = null;
            IAudioSessionManager2 mgr = null;
            IMMDevice speakers = null;
            IPropertyStore prop;
            PropVariant NameEndpoint;
            PropertyKey key = new PropertyKey() {
                fmtid = PKEY.PKEY_Device_FriendlyName,
                pid = 14 // https://stackoverflow.com/questions/32151133/how-to-get-pkey-device-friendlyname-if-its-not-defined
            };
            
            string nameinterfaceaudio;
            
            try
            {
                deviceEnumerator = (IMMDeviceEnumerator) (new MMDeviceEnumerator());
                deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out speakers);
                //Name of AudioEndpoint
                speakers.OpenPropertyStore(EStgmAccess.STGM_READ, out prop);
                
                prop.GetValue(ref key, out NameEndpoint);
                nameinterfaceaudio = (string) NameEndpoint.Value;


                return nameinterfaceaudio;


            }
            finally
            {
                
                if (deviceEnumerator != null) Marshal.ReleaseComObject(deviceEnumerator);
                if (sessionEnumerator != null) Marshal.ReleaseComObject(sessionEnumerator);
                if (mgr != null) Marshal.ReleaseComObject(mgr);
                if (speakers != null) Marshal.ReleaseComObject(speakers);
            }
    }
    #endregion
    
    }
}