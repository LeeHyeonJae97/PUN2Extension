using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Extension
{
    public class ResourcesPool : IPunPrefabPool
    {
        private Dictionary<string, GameObject> _dic;

        public ResourcesPool(ResourcesSO resources)
        {
            if (_dic == null)
            {
                _dic = new Dictionary<string, GameObject>();

                foreach (var prefab in resources.List)
                {
                    _dic.Add(prefab.name, prefab);
                }
            }
        }

        public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
        {
            return _dic.TryGetValue(prefabId, out var prefab) ? GameObject.Instantiate(prefab, position, rotation) : null;
        }

        public void Destroy(GameObject gameObject)
        {
            GameObject.Destroy(gameObject);
        }
    }
}