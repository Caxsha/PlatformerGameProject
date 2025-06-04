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
    [SerializeField] GameObject createRoomButton;
    [SerializeField] GameObject startGameButton;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Debug.Log("Connecting to Master");
        MenuManager.Instance.OpenMenu("loading"); // Открываем LoadingMenu во время подключения
        PhotonNetwork.ConnectUsingSettings();
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
        MenuManager.Instance.OpenMenu("title"); // Переключаемся на TitleMenu после присоединения к лобби
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

        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        Player[] players = PhotonNetwork.PlayerList;
        List<Player> sortedPlayers = players.OrderBy(p => p.IsMasterClient ? 0 : 1).ToList();

        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(sortedPlayers[i]);
        }

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
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        Player[] players = PhotonNetwork.PlayerList;
        List<Player> sortedPlayers = players.OrderBy(p => p.IsMasterClient ? 0 : 1).ToList();

        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(sortedPlayers[i]);
        }

        if (startGameButton != null)
        {
            startGameButton.SetActive(PhotonNetwork.IsMasterClient);
            Debug.Log($"Master client switched to {newMasterClient.NickName}. IsMasterClient: {PhotonNetwork.IsMasterClient}, StartGameButton active: {startGameButton.activeSelf}");
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        Player[] players = PhotonNetwork.PlayerList;
        List<Player> sortedPlayers = players.OrderBy(p => p.IsMasterClient ? 0 : 1).ToList();

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
        // Очищаем PlayerPrefs при выходе из комнаты
        PlayerPrefs.DeleteKey("username");
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
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        Player[] players = PhotonNetwork.PlayerList;
        List<Player> sortedPlayers = players.OrderBy(p => p.IsMasterClient ? 0 : 1).ToList();

        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(sortedPlayers[i]);
        }
    }
}