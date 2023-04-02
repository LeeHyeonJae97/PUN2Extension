using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace Photon.Extension
{
    public class PhotonTimer : MonoBehaviour
    {
        public const byte START = 0;
        public const byte SEC_ELAPSED = 1;
        public const byte EXPIRED = 2;

        public static event UnityAction<byte, int, int> onMilliSecElapsed;
        public static event UnityAction<byte, int, int> onSecElapsed;
        public static event UnityAction<byte> onExpired;

        [SerializeField] private PhotonView _pv;
        private byte _code;
        private int _time;
        private int _startTime;
        private int _elapsedMilliSec;
        private int _elapsedSec;

        public static void Start(byte code, int time)
        {
            var timer = PhotonUnity.InstantiateRoomObject<PhotonTimer>("PhotonTimer", Vector3.zero, Quaternion.identity);

            timer._pv.RPC(nameof(StartRPC), RpcTarget.All, code, time, PhotonNetwork.ServerTimestamp);
        }

        public void Expire()
        {
            onExpired?.Invoke(_code);

            if (_pv.IsMine)
            {
                PhotonUnity.Destroy(gameObject);
            }
        }

        private void Update()
        {
            _elapsedMilliSec = PhotonNetwork.ServerTimestamp - _startTime;

            onMilliSecElapsed?.Invoke(_code, _elapsedMilliSec, _time);

            // check elapsed more than sec
            if (_elapsedMilliSec > (_elapsedSec + 1) * 1000)
            {
                _elapsedSec = _elapsedMilliSec / 1000;

                onSecElapsed?.Invoke(_code, _elapsedSec, _time);

                // check expired
                if (_elapsedMilliSec > _time * 1000)
                {
                    Expire();
                }
            }
        }

        [PunRPC]
        private void StartRPC(byte code, int time, int startTime)
        {
            _code = code;
            _time = time;
            _startTime = startTime;
            _elapsedMilliSec = 0;
            _elapsedSec = 0;
        }
    }
}
