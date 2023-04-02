using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Photon.Extension
{
    [RequireComponent(typeof(PhotonView))]
    public class PhotonEventHandler : MonoBehaviour
    {
        private static PhotonEventHandler _instance;

        [SerializeField] private PhotonView _pv;
        [SerializeField] private PhotonEventChannelSO _channel;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        // may not get event channel from dictionary if called on Awake or before Awake called
        public static void AddListener(UnityAction<object> callback)
        {
            _instance._channel.onEventRaised += callback;
        }

        // may not get event channel from dictionary if called on Awake or before Awake called
        public static void RemoveListener(UnityAction<object> callback)
        {
            _instance._channel.onEventRaised -= callback;
        }

        public static void RaiseEvent(object args = null, float delay = 0, bool fromMaster = false)
        {
            if (delay == 0)
            {
                if (!fromMaster || PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    _instance._pv.RPC(nameof(RaiseEventRPC), RpcTarget.All, args);
                }
            }
            else
            {
                _instance.StartCoroutine(CoRaiseEvent());
            }

            // LOCAL FUNCTION
            IEnumerator CoRaiseEvent()
            {
                yield return new WaitForSeconds(delay);

                if (!fromMaster || PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    _instance._pv.RPC(nameof(RaiseEventRPC), RpcTarget.All, args);
                }
            }
        }

        [PunRPC]
        private void RaiseEventRPC(object args)
        {
            _instance._channel.RaiseEvent(args);
        }
    }
}