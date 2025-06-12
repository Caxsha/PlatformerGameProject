using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    PhotonView pv;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        pv = GetComponent<PhotonView>();
        DontDestroyOnLoad(gameObject);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 2)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }

    public void DestroyInstance()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC("DestroyRoomManager", RpcTarget.All);
        }
    }

    [PunRPC]
    void DestroyRoomManager()
    {
        if (Instance == this)
        {
            Instance = null;
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        DestroyInstance();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            DestroyInstance();
        }
    }

    public void LoadNewScene(int sceneIndex)
    {
        DestroyInstance();
        PhotonNetwork.LoadLevel(sceneIndex);
    }
}
