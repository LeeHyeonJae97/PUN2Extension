using Photon.Pun;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressablesPool : IPunPrefabPool
{
    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        return Addressables.InstantiateAsync(prefabId, position, rotation).WaitForCompletion();
    }

    public void Destroy(GameObject gameObject)
    {
        Addressables.ReleaseInstance(gameObject);
    }
}