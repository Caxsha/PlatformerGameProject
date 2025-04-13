using UnityEngine;
using UnityEngine.SceneManagement;
using Unisave.Facets;
using TMPro;

public class AuthManager : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;

    public async void OnRegisterButton()
    {
        bool success = await this.CallFacet((AuthFacet f) => f.Register(
            usernameField.text, passwordField.text
        ));
        Debug.Log(success ? "Регистрация успешна!" : "Пользователь уже существует!");
    }

    public async void OnLoginButton()
    {
        // Проверка на пустые поля
        if (string.IsNullOrEmpty(usernameField.text) || string.IsNullOrEmpty(passwordField.text))
        {
            Debug.Log("Заполните все поля!");
            return;
        }

        string playerId = await this.CallFacet((AuthFacet f) => f.Login(
            usernameField.text, passwordField.text
        ));
        if (playerId != null)
        {
            PlayerPrefs.SetString("PlayerId", playerId); // Сохраняем ID игрока
            PlayerPrefs.SetString("username", usernameField.text); // Сохраняем логин как никнейм
            Debug.Log("Вход успешен! ID игрока: " + playerId);
            SceneManager.LoadScene("Menu");
        }
        else
        {
            Debug.Log("Неверный логин или пароль!");
        }
    }
}