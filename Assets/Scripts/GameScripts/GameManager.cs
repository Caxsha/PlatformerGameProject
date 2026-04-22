using UnityEngine;
using Photon.Pun;
using System.IO;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Not in Photon Room, cannot instantiate PlayerManager!");
        }
    }
}
