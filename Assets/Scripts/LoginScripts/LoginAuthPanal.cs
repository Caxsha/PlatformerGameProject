using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unisave.Facades;
using Unisave.Facets;
using Button = UnityEngine.UI.Button;

namespace Unisave.LoginSystem
{
    public class LoginAuthPanal : MonoBehaviour
    {
        [Header("The two forms")]
        public GameObject loginForm;
        public GameObject registerForm;

        [Header("Login form controls")]
        public TMP_InputField loginField;
        public TMP_InputField loginPasswordField;
        public Button loginButton;
        public GameObject loginSpinner;
        public TMP_Text loginErrorText;
        public Button gotoRegistrationButton;

        [Header("Registration form controls")]
        public TMP_InputField registerLoginField;
        public TMP_InputField registerPasswordField;
        public TMP_InputField registerConfirmPasswordField;
        public Button registerButton;
        public GameObject registerSpinner;
        public TMP_Text registerErrorText;
        public Button gotoLoginButton;

        [Header("Text messages")]
        public string msgLoginFailedGeneric = "Вход не удался.";
        public string msgInvalidLoginCredentials = "Указанные данные для входа неверны.";
        public string msgRegisterFailedGeneric = "Регистрация не удалась.";
        public string msgPasswordsDontMatch = "Поля паролей не совпадают.";
        public string msgLoginTaken = "Этот логин уже зарегистрирован.";
        public string msgInvalidLogin = "Недопустимый логин.";
        public string msgWeakPassword = "Пароль должен содержать не менее 8 символов.";
        private string msgEmptyFields = "Заполните все поля.";

        [Serializable]
        public class LoginEvent : UnityEvent { }

        [Serializable]
        public class RegisterEvent : UnityEvent { }

        public LoginEvent onLoginSuccess = new LoginEvent();
        public RegisterEvent onRegistrationSuccess = new RegisterEvent();

        public EventSystem eventSystem;

        void Start()
        {
            CheckRequiredDependencies();

            if (eventSystem == null)
                eventSystem = EventSystem.current;

            loginButton.onClick.AddListener(OnLoginClicked);
            gotoRegistrationButton.onClick.AddListener(OnGotoRegisterClicked);
            registerButton.onClick.AddListener(OnRegisterClicked);
            gotoLoginButton.onClick.AddListener(OnGotoLoginClicked);

            loginField.onSubmit.AddListener(_ => OnLoginClicked());
            loginPasswordField.onSubmit.AddListener(_ => OnLoginClicked());

            registerLoginField.onSubmit.AddListener(_ => OnRegisterClicked());
            registerPasswordField.onSubmit.AddListener(_ => OnRegisterClicked());
            registerConfirmPasswordField.onSubmit.AddListener(_ => OnRegisterClicked());

            ShowLoginForm();
        }

        private void CheckRequiredDependencies()
        {
            if (loginForm == null || registerForm == null ||
                loginField == null || loginPasswordField == null || loginButton == null ||
                loginSpinner == null || loginErrorText == null || gotoRegistrationButton == null ||
                registerLoginField == null || registerPasswordField == null ||
                registerConfirmPasswordField == null || registerButton == null ||
                registerSpinner == null || registerErrorText == null || gotoLoginButton == null)
            {
                throw new ArgumentException("Проверь, что все поля назначены в инспекторе.");
            }
        }

        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                NavigateNext(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
            }
        }

        public void ShowLoginForm()
        {
            loginForm.SetActive(true);
            registerForm.SetActive(false);

            loginField.text = "";
            loginPasswordField.text = "";
            loginSpinner.SetActive(false);
            loginErrorText.gameObject.SetActive(false);
            loginErrorText.text = msgLoginFailedGeneric;

            loginField.Select();
            loginField.ActivateInputField();
        }

        public void ShowRegisterForm()
        {
            loginForm.SetActive(false);
            registerForm.SetActive(true);

            registerLoginField.text = "";
            registerPasswordField.text = "";
            registerConfirmPasswordField.text = "";
            registerErrorText.gameObject.SetActive(false);
            registerErrorText.text = msgRegisterFailedGeneric;
            registerSpinner.SetActive(false);

            registerLoginField.Select();
            registerLoginField.ActivateInputField();
        }

        public async void OnLoginClicked()
        {
            if (string.IsNullOrEmpty(loginField.text) || string.IsNullOrEmpty(loginPasswordField.text))
            {
                loginErrorText.gameObject.SetActive(true);
                loginErrorText.text = msgEmptyFields;
                ForceRefreshElementSizes(loginForm);
                return;
            }

            loginSpinner.SetActive(true);

            string playerId = await this.CallFacet((Unisave.Backend.AuthFacet f) =>
                f.Login(loginField.text, loginPasswordField.text));

            loginSpinner.SetActive(false);

            if (!string.IsNullOrEmpty(playerId))
            {
                PlayerPrefs.SetString("PlayerId", playerId);
                PlayerPrefs.SetString("username", loginField.text);
                onLoginSuccess?.Invoke();
                SceneManager.LoadScene("Menu");
            }
            else
            {
                loginErrorText.gameObject.SetActive(true);
                loginErrorText.text = msgInvalidLoginCredentials;
                ForceRefreshElementSizes(loginForm);
            }
        }

        public void OnGotoRegisterClicked() => ShowRegisterForm();

        public async void OnRegisterClicked()
        {
            if (registerPasswordField.text != registerConfirmPasswordField.text)
            {
                registerErrorText.gameObject.SetActive(true);
                registerErrorText.text = msgPasswordsDontMatch;
                ForceRefreshElementSizes(registerForm);
                return;
            }

            if (string.IsNullOrEmpty(registerLoginField.text) || string.IsNullOrEmpty(registerPasswordField.text))
            {
                registerErrorText.gameObject.SetActive(true);
                registerErrorText.text = msgEmptyFields;
                ForceRefreshElementSizes(registerForm);
                return;
            }

            registerSpinner.SetActive(true);

            var result = await this.CallFacet((Unisave.Backend.AuthFacet f) =>
                f.Register(registerLoginField.text, registerPasswordField.text));

            registerSpinner.SetActive(false);

            switch (result.ToString())
            {
                case "Success":
                    onRegistrationSuccess?.Invoke();
                    ShowLoginForm();
                    ForceRefreshElementSizes(loginForm);
                    return;
                case "WeakPassword":
                    registerErrorText.text = msgWeakPassword;
                    break;
                case "UsernameTaken":
                    registerErrorText.text = msgLoginTaken;
                    break;
                case "InvalidUsername":
                    registerErrorText.text = msgInvalidLogin;
                    break;
                default:
                    registerErrorText.text = msgRegisterFailedGeneric;
                    break;
            }

            registerErrorText.gameObject.SetActive(true);
            ForceRefreshElementSizes(registerForm);
        }

        public void OnGotoLoginClicked() => ShowLoginForm();

        private void ForceRefreshElementSizes(GameObject form)
        {
            Canvas.ForceUpdateCanvases();
            var layout = form.GetComponent<VerticalLayoutGroup>();
            if (layout != null)
            {
                layout.enabled = false;
                layout.enabled = true;
            }
        }

        private void NavigateNext(bool reversed = false)
        {
            GameObject current = eventSystem.currentSelectedGameObject;
            if (current == null)
            {
                TMP_InputField firstField = loginForm.activeSelf ? loginField : registerLoginField;
                firstField.Select();
                firstField.ActivateInputField();
                return;
            }

            Selectable next = reversed
                ? current.GetComponent<Selectable>()?.FindSelectableOnUp()
                : current.GetComponent<Selectable>()?.FindSelectableOnDown();

            if (next == null) return;

            next.Select();

            if (next is TMP_InputField inputField)
                inputField.ActivateInputField();
        }
    }
}
