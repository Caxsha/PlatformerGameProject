using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using System.Linq;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    GameObject controller;

    int deaths;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        if (spawnpoint == null)
        {
            Debug.LogError("Spawnpoint is null! Make sure SpawnManager is set up correctly.");
            return;
        }

        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
        if (controller == null)
        {
            Debug.LogError("Failed to instantiate PlayerController!");
        }
    }

    public void Die()
    {
        if (controller != null)
        {
            PhotonNetwork.Destroy(controller);
        }
        CreateController();

        deaths++;

        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
    }

    public int Deaths => deaths;
}