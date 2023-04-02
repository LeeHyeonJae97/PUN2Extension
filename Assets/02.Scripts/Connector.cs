using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Extension;
using Photon.Voice.PUN;

public class Connector : MonoBehaviour
{
    [SerializeField] private ResourcesSO _resources;

    private void Start()
    {
        Connect();
    }

    private async void Connect()
    {
        bool success = await PhotonUnity.ConnectUsingSettingsAsync();

        if (!success) return;

        PhotonNetwork.PrefabPool = new ResourcesPool(_resources);

        await PhotonUnity.JoinRandomOrCreateRoom();

        PhotonUnity.Instantiate("Player");
    }
}
