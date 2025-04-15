using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameManager : MonoBehaviour
{
    [SerializeField] TMP_InputField usernameInput;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Отладка для проверки ника
        Debug.Log($"PlayerNameManager Start - Current NickName: {PhotonNetwork.NickName}, PlayerPrefs username: {PlayerPrefs.GetString("username", "None")}");

        if (PlayerPrefs.HasKey("username"))
        {
            string savedUsername = PlayerPrefs.GetString("username");
            usernameInput.text = savedUsername;
            PhotonNetwork.NickName = savedUsername;
            Debug.Log($"Loaded username from PlayerPrefs: {savedUsername}");
        }
        else
        {
            string randomName = "Player" + Random.Range(0, 10000).ToString("0000");
            usernameInput.text = randomName;
            PhotonNetwork.NickName = randomName;
            PlayerPrefs.SetString("username", randomName);
            Debug.Log($"Generated random username: {randomName}");
        }
    }

    public void OnUsernameInputValueChanged()
    {
        PhotonNetwork.NickName = usernameInput.text;
        Debug.Log($"Username changed to: {usernameInput.text}");
    }
}