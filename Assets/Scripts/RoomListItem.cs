using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] TMP_Text playerCountText; // Новый текст для количества игроков

    public RoomInfo info;

    public void SetUp(RoomInfo _info)
    {
        info = _info;
        text.text = _info.Name; // Устанавливаем название комнаты
        playerCountText.text = $"{_info.PlayerCount}/{_info.MaxPlayers}"; // Устанавливаем количество игроков
    }

    public void OnClick()
    {
        Launcher.Instance.JoinRoom(info);
    }
}
