using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject PlayerListItemPrefab;
    [SerializeField] GameObject createRoomButton; // Это ButtonCreateRoom в CreateRoomMenu
    [SerializeField] GameObject startGameButton;  // Это ButtonStartGame в RoomMenu

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
        // Изначально отключаем кнопку StartGame
        if (startGameButton != null)
        {
            startGameButton.SetActive(false);
            Debug.Log("StartGameButton initially set to inactive.");
        }
        else
        {
            Debug.LogError("startGameButton is not assigned in the Inspector!");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("title");
        Debug.Log("Joined Lobby");
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(roomNameInputField.text, roomOptions);
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        // Очищаем текущий список игроков
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        // Получаем список игроков
        Player[] players = PhotonNetwork.PlayerList;

        // Сортируем игроков: хост (мастер-клиент) всегда первый
        List<Player> sortedPlayers = players.OrderBy(p => p.IsMasterClient ? 0 : 1).ToList();

        // Отображаем отсортированный список
        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(sortedPlayers[i]);
        }

        // Показываем кнопку StartGame только хосту
        if (startGameButton != null)
        {
            startGameButton.SetActive(PhotonNetwork.IsMasterClient);
            Debug.Log($"Joined room. IsMasterClient: {PhotonNetwork.IsMasterClient}, StartGameButton active: {startGameButton.activeSelf}");
        }
        else
        {
            Debug.LogError("startGameButton is not assigned in the Inspector!");
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // Очищаем текущий список
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        // Получаем обновлённый список игроков
        Player[] players = PhotonNetwork.PlayerList;

        // Сортируем: новый хост всегда первый
        List<Player> sortedPlayers = players.OrderBy(p => p.IsMasterClient ? 0 : 1).ToList();

        // Обновляем список
        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(sortedPlayers[i]);
        }

        // Обновляем видимость кнопки: только новый хост видит кнопку
        if (startGameButton != null)
        {
            startGameButton.SetActive(PhotonNetwork.IsMasterClient);
            Debug.Log($"Master client switched to {newMasterClient.NickName}. IsMasterClient: {PhotonNetwork.IsMasterClient}, StartGameButton active: {startGameButton.activeSelf}");
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // Очищаем текущий список
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        // Получаем обновлённый список игроков
        Player[] players = PhotonNetwork.PlayerList;

        // Сортируем: хост всегда первый
        List<Player> sortedPlayers = players.OrderBy(p => p.IsMasterClient ? 0 : 1).ToList();

        // Обновляем список
        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(sortedPlayers[i]);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        Debug.LogError("Room Creation Failed: " + message);
        MenuManager.Instance.OpenMenu("error");
    }

    public void StartGame()
    {
        // Проверка не нужна, так как клиенты не видят кнопку
        PhotonNetwork.LoadLevel("Game");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Очищаем текущий список
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        // Получаем обновлённый список игроков
        Player[] players = PhotonNetwork.PlayerList;

        // Сортируем: хост всегда первый
        List<Player> sortedPlayers = players.OrderBy(p => p.IsMasterClient ? 0 : 1).ToList();

        // Обновляем список
        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(sortedPlayers[i]);
        }
    }
}