using UnityEngine;
using UnityEngine.Events;

namespace Photon.Extension
{
    [CreateAssetMenu(fileName = "PhotonEventChannel", menuName = "ScriptableObject/PhotonEventChannel")]
    public class PhotonEventChannelSO : EventChannel
    {
        public event UnityAction<object> onEventRaised;

        public void RaiseEvent(object args)
        {
            onEventRaised?.Invoke(args);
        }
    }
}