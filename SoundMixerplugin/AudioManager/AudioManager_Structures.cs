using System;
using System.Runtime.InteropServices;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;
namespace SoundMixer{
    #region Abstracted COM interfaces from Windows CoreAudio API

    [ComImport]
    [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
    internal class MMDeviceEnumerator
    {
    }

    public struct PropertyKey
    {
        public Guid fmtid;
        public int pid;
    };
    internal enum EDataFlow
    {
        eRender,
        eCapture,
        eAll,
        EDataFlow_enum_count
    }

    internal enum ERole
    {
        eConsole,
        eMultimedia,
        eCommunications,
        ERole_enum_count
    }
    
    [Flags]
    public enum EDeviceState : uint
    {
        DEVICE_STATE_ACTIVE = 0x00000001,
        DEVICE_STATE_UNPLUGGED = 0x00000002,
        DEVICE_STATE_NOTPRESENT = 0x00000004,
        DEVICE_STATEMASK_ALL = 0x00000007
    }
    internal enum EStgmAccess
    {
        STGM_READ = 0x00000000,
        STGM_WRITE = 0x00000001,
        STGM_READWRITE = 0x00000002
    }
    
    internal struct Blob
    {
        public IntPtr Data;
        public int Length;

        //Code Should Compile at warning level4 without any warnings, 
        //However this struct will give us Warning CS0649: Field [Fieldname] 
        //is never assigned to, and will always have its default value
        //You can disable CS0649 in the project options but that will disable
        //the warning for the whole project, it's a nice warning and we do want 
        //it in other places so we make a nice dummy function to keep the compiler
        //happy.
        private void FixCS0649()
        {
            Length = 0;
            Data = IntPtr.Zero;
        }
    }
    public static class PKEY
    {
        public static readonly Guid PKEY_DeviceInterface_FriendlyName = new Guid(0x026e516e, 0xb814, 0x414b, 0x83, 0xcd,
            0x85, 0x6d, 0x6f, 0xef, 0x48, 0x22);

        public static readonly Guid PKEY_AudioEndpoint_FormFactor = new Guid(0x1da5d803, 0xd492, 0x4edd, 0x8c, 0x23,
            0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e);

        public static readonly Guid PKEY_AudioEndpoint_ControlPanelPageProvider = new Guid(0x1da5d803, 0xd492, 0x4edd,
            0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e);

        public static readonly Guid PKEY_AudioEndpoint_Association = new Guid(0x1da5d803, 0xd492, 0x4edd, 0x8c, 0x23,
            0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e);

        public static readonly Guid PKEY_AudioEndpoint_PhysicalSpeakers = new Guid(0x1da5d803, 0xd492, 0x4edd, 0x8c,
            0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e);

        public static readonly Guid PKEY_AudioEndpoint_GUID = new Guid(0x1da5d803, 0xd492, 0x4edd, 0x8c, 0x23, 0xe0,
            0xc0, 0xff, 0xee, 0x7f, 0x0e);

        public static readonly Guid PKEY_AudioEndpoint_Disable_SysFx = new Guid(0x1da5d803, 0xd492, 0x4edd, 0x8c, 0x23,
            0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e);

        public static readonly Guid PKEY_AudioEndpoint_FullRangeSpeakers = new Guid(0x1da5d803, 0xd492, 0x4edd, 0x8c,
            0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e);

        public static readonly Guid PKEY_AudioEngine_DeviceFormat = new Guid(0xf19f064d, 0x82c, 0x4e27, 0xbc, 0x73, 0x68,
            0x82, 0xa1, 0xbb, 0x8e, 0x4c);

        public static readonly Guid PKEY_Device_FriendlyName = new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67,
            0xd1, 0x46, 0xa8, 0x50, 0xe0);

        public static readonly Guid PKEY_Device_DeviceDescription = new Guid("a45c254e-df1c-4efd-8020-67d146a850e0");
        public static readonly Guid PKEY_Device_DeviceName = new Guid("026e516e-b814-414b-83cd-856d6fef4822");
        public static readonly Guid PKEY_Device_IconPath = new Guid("259abffc-50a7-47ce-af08-68c9a7d73366");
    }
    
    [StructLayout(LayoutKind.Explicit)]
    public struct PropVariant
    {
        [FieldOffset(0)] private readonly short vt;
        [FieldOffset(2)] private readonly short wReserved1;
        [FieldOffset(4)] private readonly short wReserved2;
        [FieldOffset(6)] private readonly short wReserved3;
        [FieldOffset(8)] private readonly sbyte cVal;
        [FieldOffset(8)] private readonly byte bVal;
        [FieldOffset(8)] private readonly short iVal;
        [FieldOffset(8)] private readonly ushort uiVal;
        [FieldOffset(8)] private readonly int lVal;
        [FieldOffset(8)] private readonly uint ulVal;
        [FieldOffset(8)] private readonly long hVal;
        [FieldOffset(8)] private readonly ulong uhVal;
        [FieldOffset(8)] private readonly float fltVal;
        [FieldOffset(8)] private readonly double dblVal;
        [FieldOffset(8)] private Blob blobVal;
        [FieldOffset(8)] private readonly DateTime date;
        [FieldOffset(8)] private readonly bool boolVal;
        [FieldOffset(8)] private readonly int scode;
        [FieldOffset(8)] private readonly FILETIME filetime;
        [FieldOffset(8)] private readonly IntPtr everything_else;

        //I'm sure there is a more efficient way to do this but this works ..for now..
        internal byte[] GetBlob()
        {
            var Result = new byte[blobVal.Length];
            for (int i = 0; i < blobVal.Length; i++)
            {
                Result[i] = Marshal.ReadByte((IntPtr) ((long) (blobVal.Data) + i));
            }
            return Result;
        }

        public object Value
        {
            get
            {
                var ve = (VarEnum) vt;
                switch (ve)
                {
                    case VarEnum.VT_I1:
                        return bVal;
                    case VarEnum.VT_I2:
                        return iVal;
                    case VarEnum.VT_I4:
                        return lVal;
                    case VarEnum.VT_I8:
                        return hVal;
                    case VarEnum.VT_INT:
                        return iVal;
                    case VarEnum.VT_UI4:
                        return ulVal;
                    case VarEnum.VT_LPWSTR:
                        return Marshal.PtrToStringUni(everything_else);
                    case VarEnum.VT_BLOB:
                        return GetBlob();
                }
                return "FIXME Type = " + ve;
            }
        }
    }

    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMDeviceEnumerator
    {
        [PreserveSig]
        int EnumAudioEndpoints(EDataFlow dataFlow, EDeviceState StateMask, out IMMDeviceCollection device);

        [PreserveSig]
        int GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role, out IMMDevice ppDevice);
        

        // the rest is not implemented
    }
    
    [Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMDeviceCollection
    {
        [PreserveSig]
        int GetCount(out uint pcDevices);

        [PreserveSig]
        int Item(uint nDevice, out IMMDevice Device);
    }

    [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMDevice
    {
        [PreserveSig]
        int Activate(ref Guid iid, int dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);
        
        [PreserveSig]
        int OpenPropertyStore(EStgmAccess stgmAccess, out IPropertyStore propertyStore);

        [PreserveSig]
        int GetId([MarshalAs(UnmanagedType.LPWStr)] out string ppstrId);

        [PreserveSig]
        int GetState(out EDeviceState pdwState);
    }
    
    [Guid("886d8eeb-8cf2-4446-8d02-cdba1dbdcf99"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPropertyStore
    {
        [PreserveSig]
        int GetCount(out Int32 count);

        [PreserveSig]
        int GetAt(int iProp, out PropertyKey pkey);

        [PreserveSig]
        int GetValue(ref PropertyKey key, out PropVariant pv);

        [PreserveSig]
        int SetValue(ref PropertyKey key, ref PropVariant propvar);

        [PreserveSig]
        int Commit();
    };

    [Guid("77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioSessionManager2
    {
        int NotImpl1();
        int NotImpl2();

        [PreserveSig]
        int GetSessionEnumerator(out IAudioSessionEnumerator SessionEnum);

        // the rest is not implemented
    }

    [Guid("E2F5BB11-0570-40CA-ACDD-3AA01277DEE8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioSessionEnumerator
    {
        [PreserveSig]
        int GetCount(out int SessionCount);

        [PreserveSig]
        int GetSession(int SessionCount, out IAudioSessionControl2 Session);
    }

    [Guid("87CE5498-68D6-44E5-9215-6DA47EF883D8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISimpleAudioVolume
    {
        [PreserveSig]
        int SetMasterVolume(float fLevel, ref Guid EventContext);

        [PreserveSig]
        int GetMasterVolume(out float pfLevel);

        [PreserveSig]
        int SetMute(bool bMute, ref Guid EventContext);

        [PreserveSig]
        int GetMute(out bool pbMute);
    }

    [Guid("bfb7ff88-7239-4fc9-8fa2-07c950be9c6d"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioSessionControl2
    {
        // IAudioSessionControl
        [PreserveSig]
        int NotImpl0();

        [PreserveSig]
        int GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);

        [PreserveSig]
        int SetDisplayName([MarshalAs(UnmanagedType.LPWStr)]string Value, [MarshalAs(UnmanagedType.LPStruct)] Guid EventContext);

        [PreserveSig]
        int GetIconPath([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);

        [PreserveSig]
        int SetIconPath([MarshalAs(UnmanagedType.LPWStr)] string Value, [MarshalAs(UnmanagedType.LPStruct)] Guid EventContext);

        [PreserveSig]
        int GetGroupingParam(out Guid pRetVal);

        [PreserveSig]
        int SetGroupingParam([MarshalAs(UnmanagedType.LPStruct)] Guid Override, [MarshalAs(UnmanagedType.LPStruct)] Guid EventContext);

        [PreserveSig]
        int NotImpl1();

        [PreserveSig]
        int NotImpl2();

        // IAudioSessionControl2
        [PreserveSig]
        int GetSessionIdentifier([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);

        [PreserveSig]
        int GetSessionInstanceIdentifier([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);

        [PreserveSig]
        int GetProcessId(out int pRetVal);

        [PreserveSig]
        int IsSystemSoundsSession();

        [PreserveSig]
        int SetDuckingPreference(bool optOut);
    }
    
    [Guid("7991EEC9-7E89-4D85-8390-6C703CEC60C0"), 
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMNotificationClient
    {
        /// <summary>
        /// Device State Changed
        /// </summary>
        void OnDeviceStateChanged([MarshalAs(UnmanagedType.LPWStr)] string deviceId, [MarshalAs(UnmanagedType.I4)] EDeviceState newState);

        /// <summary>
        /// Device Added
        /// </summary>
        void OnDeviceAdded([MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId);

        /// <summary>
        /// Device Removed
        /// </summary>
        void OnDeviceRemoved([MarshalAs(UnmanagedType.LPWStr)] string deviceId);

        /// <summary>
        /// Default Device Changed
        /// </summary>
        void OnDefaultDeviceChanged(EDataFlow flow, ERole role, [MarshalAs(UnmanagedType.LPWStr)] string defaultDeviceId);

        /// <summary>
        /// Property Value Changed
        /// </summary>
        /// <param name="pwstrDeviceId"></param>
        /// <param name="key"></param>
        void OnPropertyValueChanged([MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId, PropertyKey key);
    }

    // http://netcoreaudio.codeplex.com/SourceControl/latest#trunk/Code/CoreAudio/Interfaces/IAudioEndpointVolume.cs
    [Guid("5CDF2C82-841E-4546-9722-0CF74078229A"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAudioEndpointVolume
    {
        [PreserveSig]
        int NotImpl1();

        [PreserveSig]
        int NotImpl2();

        /// <summary>
        /// Gets a count of the channels in the audio stream.
        /// </summary>
        /// <param name="channelCount">The number of channels.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int GetChannelCount(
            [Out] [MarshalAs(UnmanagedType.U4)] out UInt32 channelCount);

        /// <summary>
        /// Sets the master volume level of the audio stream, in decibels.
        /// </summary>
        /// <param name="level">The new master volume level in decibels.</param>
        /// <param name="eventContext">A user context value that is passed to the notification callback.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int SetMasterVolumeLevel(
            [In] [MarshalAs(UnmanagedType.R4)] float level,
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        /// <summary>
        /// Sets the master volume level, expressed as a normalized, audio-tapered value.
        /// </summary>
        /// <param name="level">The new master volume level expressed as a normalized value between 0.0 and 1.0.</param>
        /// <param name="eventContext">A user context value that is passed to the notification callback.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int SetMasterVolumeLevelScalar(
            [In] [MarshalAs(UnmanagedType.R4)] float level,
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        /// <summary>
        /// Gets the master volume level of the audio stream, in decibels.
        /// </summary>
        /// <param name="level">The volume level in decibels.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int GetMasterVolumeLevel(
            [Out] [MarshalAs(UnmanagedType.R4)] out float level);

        /// <summary>
        /// Gets the master volume level, expressed as a normalized, audio-tapered value.
        /// </summary>
        /// <param name="level">The volume level expressed as a normalized value between 0.0 and 1.0.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int GetMasterVolumeLevelScalar(
            [Out] [MarshalAs(UnmanagedType.R4)] out float level);

        /// <summary>
        /// Sets the volume level, in decibels, of the specified channel of the audio stream.
        /// </summary>
        /// <param name="channelNumber">The channel number.</param>
        /// <param name="level">The new volume level in decibels.</param>
        /// <param name="eventContext">A user context value that is passed to the notification callback.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int SetChannelVolumeLevel(
            [In] [MarshalAs(UnmanagedType.U4)] UInt32 channelNumber,
            [In] [MarshalAs(UnmanagedType.R4)] float level,
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        /// <summary>
        /// Sets the normalized, audio-tapered volume level of the specified channel in the audio stream.
        /// </summary>
        /// <param name="channelNumber">The channel number.</param>
        /// <param name="level">The new master volume level expressed as a normalized value between 0.0 and 1.0.</param>
        /// <param name="eventContext">A user context value that is passed to the notification callback.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int SetChannelVolumeLevelScalar(
            [In] [MarshalAs(UnmanagedType.U4)] UInt32 channelNumber,
            [In] [MarshalAs(UnmanagedType.R4)] float level,
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        /// <summary>
        /// Gets the volume level, in decibels, of the specified channel in the audio stream.
        /// </summary>
        /// <param name="channelNumber">The zero-based channel number.</param>
		/// <param name="level">The volume level in decibels.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int GetChannelVolumeLevel(
            [In] [MarshalAs(UnmanagedType.U4)] UInt32 channelNumber,
            [Out] [MarshalAs(UnmanagedType.R4)] out float level);

        /// <summary>
        /// Gets the normalized, audio-tapered volume level of the specified channel of the audio stream.
        /// </summary>
        /// <param name="channelNumber">The zero-based channel number.</param>
		/// <param name="level">The volume level expressed as a normalized value between 0.0 and 1.0.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int GetChannelVolumeLevelScalar(
            [In] [MarshalAs(UnmanagedType.U4)] UInt32 channelNumber,
            [Out] [MarshalAs(UnmanagedType.R4)] out float level);

        /// <summary>
        /// Sets the muting state of the audio stream.
        /// </summary>
        /// <param name="isMuted">True to mute the stream, or false to unmute the stream.</param>
        /// <param name="eventContext">A user context value that is passed to the notification callback.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int SetMute(
            [In] [MarshalAs(UnmanagedType.Bool)] Boolean isMuted,
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        /// <summary>
        /// Gets the muting state of the audio stream.
        /// </summary>
        /// <param name="isMuted">The muting state. True if the stream is muted, false otherwise.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int GetMute(
            [Out] [MarshalAs(UnmanagedType.Bool)] out Boolean isMuted);

        /// <summary>
        /// Gets information about the current step in the volume range.
        /// </summary>
        /// <param name="step">The current zero-based step index.</param>
        /// <param name="stepCount">The total number of steps in the volume range.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int GetVolumeStepInfo(
            [Out] [MarshalAs(UnmanagedType.U4)] out UInt32 step,
            [Out] [MarshalAs(UnmanagedType.U4)] out UInt32 stepCount);

        /// <summary>
        /// Increases the volume level by one step.
        /// </summary>
        /// <param name="eventContext">A user context value that is passed to the notification callback.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int VolumeStepUp(
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        /// <summary>
        /// Decreases the volume level by one step.
        /// </summary>
        /// <param name="eventContext">A user context value that is passed to the notification callback.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int VolumeStepDown(
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        /// <summary>
        /// Queries the audio endpoint device for its hardware-supported functions.
        /// </summary>
        /// <param name="hardwareSupportMask">A hardware support mask that indicates the capabilities of the endpoint.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int QueryHardwareSupport(
            [Out] [MarshalAs(UnmanagedType.U4)] out UInt32 hardwareSupportMask);

        /// <summary>
        /// Gets the volume range of the audio stream, in decibels.
        /// </summary>
		/// <param name="volumeMin">The minimum volume level in decibels.</param>
		/// <param name="volumeMax">The maximum volume level in decibels.</param>
		/// <param name="volumeStep">The volume increment level in decibels.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int GetVolumeRange(
            [Out] [MarshalAs(UnmanagedType.R4)] out float volumeMin,
            [Out] [MarshalAs(UnmanagedType.R4)] out float volumeMax,
            [Out] [MarshalAs(UnmanagedType.R4)] out float volumeStep);
    }

    #endregion
}