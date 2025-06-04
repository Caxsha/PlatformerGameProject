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
        // Проверяем, существует ли другой экземпляр
        if (Instance != null && Instance != this)
        {
            // Уничтожаем новый объект локально, если старый уже есть
            Destroy(gameObject);
            return;
        }

        Instance = this;
        pv = GetComponent<PhotonView>();
        DontDestroyOnLoad(gameObject); // Убедитесь, что объект сохраняется между сценами
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
        if (scene.buildIndex == 2) // Мы находимся на игровой сцене
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }

    // Вызывается для уничтожения RoomManager
    public void DestroyInstance()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Только мастер-клиент отправляет RPC для уничтожения
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

    // Вызывается при выходе из комнаты
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        // Уничтожаем RoomManager при выходе из комнаты
        DestroyInstance();
    }

    // Вызывается при смене хоста
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Уничтожаем объект на старом хосте
            DestroyInstance();
        }
    }

    // Добавляем явное уничтожение объекта при переходе на другую сцену
    public void LoadNewScene(int sceneIndex)
    {
        DestroyInstance(); // Уничтожаем объект перед загрузкой новой сцены
        PhotonNetwork.LoadLevel(sceneIndex); // Загружаем новую сцену
    }
}
