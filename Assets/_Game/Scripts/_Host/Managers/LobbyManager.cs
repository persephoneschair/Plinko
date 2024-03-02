using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyManager : SingletonMonoBehaviour<LobbyManager>
{
    public TextMeshProUGUI welcomeMessageMesh;

    [Button]
    public void OnOpenLobby()
    {
        welcomeMessageMesh.text = welcomeMessageMesh.text.Replace("[ABCD]", HostManager.Get.host.RoomCode.ToUpperInvariant());
    }
}
