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
        usernameInput.interactable = false;
        // Проверяем, есть ли сохранённый логин из сцены Login
        if (PlayerPrefs.HasKey("username"))
        {
            string savedUsername = PlayerPrefs.GetString("username");
            usernameInput.text = savedUsername;
            PhotonNetwork.NickName = savedUsername;
        }
        else
        {
            // Если логина нет, генерируем случайное имя
            usernameInput.text = "Player" + Random.Range(0, 10000).ToString("0000");
            PhotonNetwork.NickName = usernameInput.text;
            PlayerPrefs.SetString("username", usernameInput.text); // Сохраняем сгенерированное имя
        }
    }

	public void OnUsernameInputValueChanged()
	{
        // Если пользователь изменит имя в поле ввода, обновляем никнейм
        PhotonNetwork.NickName = usernameInput.text;
        PlayerPrefs.SetString("username", usernameInput.text);
    }
}
