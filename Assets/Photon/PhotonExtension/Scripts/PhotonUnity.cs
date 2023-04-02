using Photon.Pun;
using Photon.Realtime;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Photon.Extension
{
    public class PhotonUnity : MonoBehaviourPunCallbacks
    {
        private static PhotonUnity _instance;

        public static event UnityAction onConnectedToMaster;
        public static event UnityAction onCreatedRoom;
        public static event UnityAction<short, string> onCreateRoomFailed;
        public static event UnityAction<DisconnectCause> onDisconnected;
        public static event UnityAction<ErrorInfo> onErrorInfo;
        public static event UnityAction onJoinedLobby;
        public static event UnityAction onJoinedRoom;
        public static event UnityAction<short, string> onJoinRandomFailed;
        public static event UnityAction<short, string> onJoinRoomFailed;
        public static event UnityAction onLeftLobby;
        public static event UnityAction onLeftRoom;
        public static event UnityAction<Player> onMasterClientSwitched;
        public static event UnityAction<Player> onPlayerEnteredRoom;
        public static event UnityAction<Player> onPlayerLeftRoom;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public static async Task<bool> ConnectUsingSettingsAsync()
        {
            if (_instance == null)
            {
                _instance = new GameObject("PhotonUnity").AddComponent<PhotonUnity>();
            }

            if (PhotonNetwork.IsConnectedAndReady)
            {
                return await Task.FromResult(false);
            }

            if (!PhotonNetwork.ConnectUsingSettings())
            {
                return await Task.FromResult(false);
            }

            var tcs = new TaskCompletionSource<bool>();

            onConnectedToMaster += OnSuccess;

            return await tcs.Task;

            // LOCAL FUNTION
            void OnSuccess()
            {
                onConnectedToMaster -= OnSuccess;

                tcs.TrySetResult(true);
            }
        }

        public static async Task<bool> DisconnectAsync()
        {
            if (!PhotonNetwork.IsConnectedAndReady)
            {
                return await Task.FromResult(false);
            }

            PhotonNetwork.Disconnect();

            var tcs = new TaskCompletionSource<bool>();

            onDisconnected += OnSuccess;

            return await tcs.Task;

            // LOCAL FUNTION
            void OnSuccess(DisconnectCause cause)
            {
                onDisconnected -= OnSuccess;

                Destroy(_instance.gameObject);

                tcs.TrySetResult(true);
            }
        }

        public static async Task<bool> JoinLobbyAsync(string lobbyName, LobbyType lobbyType)
        {
            if (!PhotonNetwork.IsConnectedAndReady || PhotonNetwork.InLobby || PhotonNetwork.InRoom)
            {
                return await Task.FromResult(false);
            }

            if (!PhotonNetwork.JoinLobby(new TypedLobby(lobbyName, lobbyType)))
            {
                return await Task.FromResult(false);
            }

            var tcs = new TaskCompletionSource<bool>();

            onJoinedLobby += OnSuccess;

            return await tcs.Task;

            // LOCAL FUNTION
            void OnSuccess()
            {
                onJoinedLobby -= OnSuccess;

                tcs.TrySetResult(true);
            }
        }

        public static async Task<bool> CreateRoomAsync(string roomName, RoomOptions roomOptions = null, TypedLobby typedLobby = null, string[] expectedUsers = null)
        {
            if (!PhotonNetwork.IsConnectedAndReady || PhotonNetwork.InRoom)
            {
                return await Task.FromResult(false);
            }

            if (!PhotonNetwork.CreateRoom(roomName, roomOptions, typedLobby, expectedUsers))
            {
                return await Task.FromResult(false);
            }

            var tcs = new TaskCompletionSource<bool>();

            onCreatedRoom += OnSuccess;
            onCreateRoomFailed += OnFail;

            return await tcs.Task;

            // LOCAL FUNTION
            void OnSuccess()
            {
                onCreatedRoom -= OnSuccess;
                onCreateRoomFailed -= OnFail;

                tcs.TrySetResult(true);
            }

            // LOCAL FUNTION
            void OnFail(short returnCode, string message)
            {
                onCreatedRoom -= OnSuccess;
                onCreateRoomFailed -= OnFail;

                tcs.TrySetResult(false);
            }
        }

        public static async Task<bool> JoinRandomOrCreateRoom(ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = null, byte expectedMaxPlayers = 0, MatchmakingMode matchingType = MatchmakingMode.FillRoom, TypedLobby typedLobby = null, string sqlLobbyFilter = null, string roomName = null, RoomOptions roomOptions = null, string[] expectedUsers = null)
        {
            if (!PhotonNetwork.IsConnectedAndReady || PhotonNetwork.InRoom)
            {
                return await Task.FromResult(false);
            }

            if (!PhotonNetwork.JoinRandomOrCreateRoom(expectedCustomRoomProperties, expectedMaxPlayers, matchingType, typedLobby, sqlLobbyFilter, roomName, roomOptions, expectedUsers))
            {
                return await Task.FromResult(false);
            }

            var tcs = new TaskCompletionSource<bool>();

            onJoinedRoom += OnSuccess;
            onJoinRoomFailed += OnFail;

            return await tcs.Task;

            // LOCAL FUNTION
            void OnSuccess()
            {
                onJoinedRoom -= OnSuccess;
                onJoinRoomFailed -= OnFail;

                tcs.TrySetResult(true);
            }

            // LOCAL FUNTION
            void OnFail(short returnCode, string message)
            {
                onJoinedRoom -= OnSuccess;
                onJoinRoomFailed -= OnFail;

                tcs.TrySetResult(false);
            }
        }

        public static async Task<bool> LeaveRoomAsync(bool becomeInactive = true)
        {
            if (!PhotonNetwork.IsConnectedAndReady || !PhotonNetwork.InRoom)
            {
                return await Task.FromResult(false);
            }

            if (!PhotonNetwork.LeaveRoom(becomeInactive))
            {
                return await Task.FromResult(false);
            }

            var tcs = new TaskCompletionSource<bool>();

            onConnectedToMaster += OnSuccess;

            return await tcs.Task;

            // LOCAL FUNTION
            void OnSuccess()
            {
                onConnectedToMaster -= OnSuccess;

                tcs.TrySetResult(true);
            }
        }

        public static GameObject Instantiate(string prefabName, byte group = 0, object[] data = null)
        {
            return PhotonNetwork.Instantiate(prefabName, Vector3.zero, Quaternion.identity, group, data);
        }

        public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null)
        {
            return PhotonNetwork.Instantiate(prefabName, position, rotation, group, data);
        }

        public static GameObject InstantiateRoomObject(string prefabName, byte group = 0, object[] data = null)
        {
            return PhotonNetwork.InstantiateRoomObject(prefabName, Vector3.zero, Quaternion.identity, group, data);
        }

        public static GameObject InstantiateRoomObject(string prefabName, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null)
        {
            return PhotonNetwork.InstantiateRoomObject(prefabName, position, rotation, group, data);
        }

        public static T Instantiate<T>(string prefabName, byte group = 0, object[] data = null) where T : MonoBehaviour
        {
            return Instantiate(prefabName, group, data).GetComponent<T>();
        }

        public static T Instantiate<T>(string prefabName, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null) where T : MonoBehaviour
        {
            return Instantiate(prefabName, position, rotation, group, data).GetComponent<T>();
        }

        public static T InstantiateRoomObject<T>(string prefabName, byte group = 0, object[] data = null) where T : MonoBehaviour
        {
            return InstantiateRoomObject(prefabName, group, data).GetComponent<T>();
        }

        public static T InstantiateRoomObject<T>(string prefabName, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null) where T : MonoBehaviour
        {
            return InstantiateRoomObject(prefabName, position, rotation, group, data).GetComponent<T>();
        }

        #region Callbacks
        public override void OnConnectedToMaster()
        {
            onConnectedToMaster?.Invoke();
        }

        public override void OnCreatedRoom()
        {
            onCreatedRoom?.Invoke();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            onCreateRoomFailed?.Invoke(returnCode, message);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            onDisconnected?.Invoke(cause);
        }

        public override void OnErrorInfo(ErrorInfo errorInfo)
        {
            onErrorInfo?.Invoke(errorInfo);
        }

        public override void OnJoinedLobby()
        {
            onJoinedLobby?.Invoke();
        }

        public override void OnJoinedRoom()
        {
            onJoinedRoom?.Invoke();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            onJoinRandomFailed?.Invoke(returnCode, message);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            onJoinRoomFailed?.Invoke(returnCode, message);
        }

        public override void OnLeftLobby()
        {
            onLeftLobby?.Invoke();
        }

        public override void OnLeftRoom()
        {
            onLeftRoom?.Invoke();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            onMasterClientSwitched?.Invoke(newMasterClient);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            onPlayerEnteredRoom?.Invoke(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            onPlayerLeftRoom?.Invoke(otherPlayer);
        }
        #endregion
    }
}
