using UnityEngine;

namespace Photon.Extension
{
    [CreateAssetMenu(fileName = "Resources", menuName = "ScriptableObject/Resources")]
    public class ResourcesSO : ScriptableObject
    {
        public GameObject[] List => _list;

        [SerializeField] private GameObject[] _list;
    }
}
