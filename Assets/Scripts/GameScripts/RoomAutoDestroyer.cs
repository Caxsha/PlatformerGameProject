using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class RoomAutoDestroyer : MonoBehaviourPunCallbacks
{
    void Start()
    {
        Debug.Log("RoomAutoDestroyer активирован. Игроков в комнате: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Игрок покинул комнату: " + otherPlayer.NickName);
        CheckRoomState();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Игрок вошел в комнату: " + newPlayer.NickName);
    }

    void CheckRoomState()
    {
        // Если мы остались последним игроком и не являемся хозяином — просто выходим
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 1 && PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Остался только хост. Закрываем комнату...");
            CloseRoomAndReturnToMenu();
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount == 0)
        {
            Debug.Log("Нет игроков в комнате. Комната будет удалена сервером автоматически.");
        }
    }

    void CloseRoomAndReturnToMenu()
    {
        // Отключаем возможность подключения к комнате
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        // Покидаем комнату
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Покинули комнату. Загружаем сцену Menu.");
        SceneManager.LoadScene("Menu"); // Убедись, что сцена Menu добавлена в Build Settings
    }
}
