using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Photon.Extension
{
    public class PhotonChat : MonoBehaviour, IChatClientListener
    {
        public static bool IsConnectedAndReady { get; private set; }
        public static bool IsChattable { get; private set; }

        private static PhotonChat _instance;
        private static ChatClient _chatClient;

        public static event UnityAction onConnected;
        public static event UnityAction onDisconnected;
        public static event UnityAction<string, string> onGetMessages;
        public static event UnityAction<string[], bool[]> onSubscribed;
        public static event UnityAction<string[]> onUnsubscribed;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            _chatClient.Service();
        }

        public static async Task<bool> ConnectAsync()
        {
            _instance = new GameObject("PhotonChat").AddComponent<PhotonChat>();

            if (_chatClient != null)
            {
                return await Task.FromResult(false);
            }

            _chatClient = new ChatClient(_instance);
            _chatClient.AuthValues = new AuthenticationValues(PhotonNetwork.NickName);

            var appSettings = PhotonNetwork.PhotonServerSettings.AppSettings;
            var chatAppSettings = new ChatAppSettings
            {
                AppIdChat = appSettings.AppIdChat,
                AppVersion = appSettings.AppVersion,
                FixedRegion = appSettings.IsBestRegion ? null : appSettings.FixedRegion,
                NetworkLogging = appSettings.NetworkLogging,
                Protocol = appSettings.Protocol,
                EnableProtocolFallback = appSettings.EnableProtocolFallback,
                Server = appSettings.IsDefaultNameServer ? null : appSettings.Server,
                Port = (ushort)appSettings.Port
            };

            if (!_chatClient.ConnectUsingSettings(chatAppSettings))
            {
                return await Task.FromResult(false);
            }

            var tcs = new TaskCompletionSource<bool>();

            onConnected += OnSuccess;

            return await tcs.Task;

            // LOCAL FUNCTION
            void OnSuccess()
            {
                onConnected -= OnSuccess;

                IsConnectedAndReady = true;

                tcs.TrySetResult(true);
            }
        }

        public static async Task<bool> DisconnectAsync()
        {
            if (_instance == null || _chatClient == null)
            {
                return await Task.FromResult(false);
            }

            _chatClient.Disconnect();

            var tcs = new TaskCompletionSource<bool>();

            onDisconnected += OnSuccess;

            return await tcs.Task;

            // LOCAL FUNCTION
            void OnSuccess()
            {
                onDisconnected -= OnSuccess;

                Destroy(_instance.gameObject);
                _chatClient = null;
                IsConnectedAndReady = false;
                IsChattable = false;

                tcs.TrySetResult(true);
            }
        }

        public static async Task<bool> SubscribeAsync(string channel)
        {
            if (_chatClient == null)
            {
                return await Task.FromResult(false);
            }

            if (!_chatClient.Subscribe(channel))
            {
                return await Task.FromResult(false);
            }

            var tcs = new TaskCompletionSource<bool>();

            onSubscribed += OnSuccess;

            return await tcs.Task;

            // LOCAL FUNCTION
            void OnSuccess(string[] channels, bool[] results)
            {
                onSubscribed -= OnSuccess;

                IsChattable = true;

                tcs.TrySetResult(results.All((result) => result == true));
            }
        }

        public static async Task<bool> UnsubscribeAsync(string channel)
        {
            return await UnsubscribeAsync(new string[] { channel });
        }

        public static async Task<bool> UnsubscribeAsync(string[] channels)
        {
            if (_chatClient == null)
            {
                return await Task.FromResult(false);
            }

            if (!_chatClient.Unsubscribe(channels))
            {
                return await Task.FromResult(false);
            }

            var tcs = new TaskCompletionSource<bool>();

            onUnsubscribed += OnSuccess;

            return await tcs.Task;

            // LOCAL FUNCTION
            void OnSuccess(string[] channels)
            {
                onUnsubscribed -= OnSuccess;

                IsChattable = false;

                tcs.TrySetResult(true);
            }
        }

        public static bool SendMessage(string channelName, string message)
        {
            if (_chatClient == null || !_chatClient.PublicChannels.ContainsKey(channelName))
            {
                return false;
            }

            return _chatClient.PublishMessage(channelName, message);
        }

        public static string GetMessage(string channelName)
        {
            if (_chatClient == null) return null;

            return _chatClient.TryGetChannel(channelName, out var channel) ? channel.ToStringMessages() : null;
        }

        #region Callbacks
        public void OnConnected()
        {
            onConnected?.Invoke();
        }

        public void OnDisconnected()
        {
            onDisconnected?.Invoke();
        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            onGetMessages?.Invoke(channelName, _chatClient.TryGetChannel(channelName, out var channel) ? channel.ToStringMessages() : null);
        }

        public void OnSubscribed(string[] channels, bool[] results)
        {
            onSubscribed?.Invoke(channels, results);
        }

        public void OnUnsubscribed(string[] channels)
        {
            onUnsubscribed?.Invoke(channels);
        }

        public void DebugReturn(DebugLevel level, string message)
        {
            switch (level)
            {
                case DebugLevel.ERROR:
                    Debug.LogError(message);
                    break;

                case DebugLevel.WARNING:
                    Debug.LogWarning(message);
                    break;

                case DebugLevel.INFO:
                case DebugLevel.ALL:
                    //Debug.Log(message);
                    break;
            }
        }

        public void OnChatStateChange(ChatState state)
        {

        }

        public void OnPrivateMessage(string sender, object message, string channelName)
        {

        }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {

        }

        public void OnUserSubscribed(string channel, string user)
        {

        }

        public void OnUserUnsubscribed(string channel, string user)
        {

        }
        #endregion
    }
}
