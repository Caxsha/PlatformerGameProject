using UnityEngine;
using UnityEngine.SceneManagement; // Для перехода на другую сцену
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
        string playerId = await this.CallFacet((AuthFacet f) => f.Login(
            usernameField.text, passwordField.text
        ));
        if (playerId != null)
        {
            PlayerPrefs.SetString("PlayerId", playerId); // Сохраняем ID игрока
            Debug.Log("Вход успешен! ID игрока: " + playerId);
            SceneManager.LoadScene("MainMenu"); // Переходим на следующую сцену
        }
        else
        {
            Debug.Log("Неверный логин или пароль!");
        }
    }
}