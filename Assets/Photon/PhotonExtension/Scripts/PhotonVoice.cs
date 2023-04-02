using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

namespace Photon.Extension
{
    public class PhotonVoice
    {
        public static bool IsConnectedAndReady { get; private set; }

        private static PunVoiceClient _instance;

        public static void Connect()
        {
#if UNITY_EDITOR
            if (_instance != null) return;

            var go = new GameObject("PhotonVoice");

            var logger = go.AddComponent<VoiceLogger>();

            var recorder = go.AddComponent<Recorder>();
            recorder.TransmitEnabled = true;

            var voice = go.AddComponent<PunVoiceClient>();
            voice.VoiceLogger.LogLevel = ExitGames.Client.Photon.DebugLevel.WARNING;
            voice.UsePunAppSettings = true;
            voice.UsePunAuthValues = true;
            voice.AutoConnectAndJoin = true;
            voice.AutoLeaveAndDisconnect = false;
            voice.PrimaryRecorder = recorder;

            _instance = voice;

            IsConnectedAndReady = true;
#endif
        }

        public static void Disconnect()
        {
#if UNITY_EDITOR
            if (_instance == null) return;

            _instance.Disconnect();

            GameObject.Destroy(_instance.gameObject);
            IsConnectedAndReady = false;
#endif
        }

        public static void Pause()
        {
            if (_instance == null) return;

            _instance.enabled = false;
        }

        public static void Resume()
        {
            if (_instance == null) return;

            _instance.enabled = true;
        }
    }
}
