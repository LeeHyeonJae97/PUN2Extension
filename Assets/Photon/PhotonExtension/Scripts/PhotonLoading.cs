using Photon.Pun;
using UnityEngine;

namespace Photon.Extension
{
    public class PhotonLoading
    {
        private static GameObject _instance;

        public static void Start()
        {
            if (_instance != null) return;

            _instance = PhotonNetwork.PrefabPool.Instantiate("Canvas - PhotonLoading", Vector3.zero, Quaternion.identity);

            if (_instance == null) return;

            GameObject.DontDestroyOnLoad(_instance);
        }

        public static void Stop()
        {
            if (_instance == null) return;

            PhotonNetwork.PrefabPool.Destroy(_instance);
        }
    }
}