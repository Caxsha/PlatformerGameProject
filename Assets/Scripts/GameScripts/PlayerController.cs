using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("UI Components")]
    [SerializeField] private Image healthbarFillImage;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private GameObject ui;
    [SerializeField] private GameObject cameraHolder;
    [SerializeField] private PlayerCoinManager coinManager;

    private float verticalLookRotation;
    private bool grounded;
    private Vector3 smoothMoveVelocity;
    private Vector3 moveAmount;

    private PhotonView PV;
    private CharacterController characterController;

    private const float maxHealth = 100f;
    private float currentHealth = maxHealth;

    private PlayerManager playerManager;
    private int coins = 0;


    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        PV = GetComponent<PhotonView>();

        if (PV.InstantiationData != null && PV.InstantiationData.Length > 0)
        {
            int playerManagerViewID = (int)PV.InstantiationData[0];
            var playerManagerPhotonView = PhotonView.Find(playerManagerViewID);
            if (playerManagerPhotonView != null)
            {
                playerManager = playerManagerPhotonView.GetComponent<PlayerManager>();
            }
        }
    }

    void Start()
    {
        if (PV.IsMine)
        {
            currentHealth = maxHealth;
            UpdateHealthUI();

            if (ui != null)
            {
                ui.SetActive(true);

                if (coinManager != null && coinManager.Text == null)
                {
                    coinManager.Text = ui.transform.Find("Text (TMP)")?.GetComponent<TextMeshProUGUI>();
                }
            }

            if (coinManager != null)
            {
                coins = playerManager != null ? playerManager.savedCoins : 0;
                coinManager.SetCoins(coins);
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(GetComponentInChildren<AudioListener>());
            Destroy(characterController);
            if (ui != null) ui.SetActive(false);
        }
    }

    void Update()
    {
        if (!PV.IsMine)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (transform.position.y < -10f)
        {
            Die();
        }
    }

    void FixedUpdate()
    {
        if (!PV.IsMine)
            return;

        characterController.Move(transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    void Die()
    {
        if (playerManager != null)
        {
            playerManager.Die();
        }
        else
        {
            if (PV.IsMine)
                PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    public void AddCoin()
    {
        coins++;

        if (playerManager != null)
            playerManager.savedCoins = coins;

        Debug.Log($"{photonView.Owner.NickName} собрал монету. Всего монет: {coins}");

        if (PV.IsMine && coinManager != null)
        {
            coinManager.SetCoins(coins);
        }
    }

    [PunRPC]
    public void DeclareVictory(string winnerName)
    {
        Debug.Log("Победил: " + winnerName);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PhotonView view = player.GetComponent<PhotonView>();
            if (view != null)
            {
                view.RPC("DisablePlayerControl", RpcTarget.All);
            }
        }

        PlayerPrefs.SetString("LastWinner", winnerName);

        VictoryUIManager.Instance.ShowVictory(winnerName);

        StartCoroutine(DisconnectAll());
    }

    private IEnumerator DisconnectAll()
    {
        yield return new WaitForSeconds(5f);

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
            yield return new WaitUntil(() => !PhotonNetwork.IsConnected);
        }

        SceneManager.LoadScene("Menu");
    }

    private IEnumerator ReturnToMenu()
    {
        yield return new WaitForSeconds(5f);

        PhotonNetwork.Disconnect();
        yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.Disconnected);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    [PunRPC]
    public void TeleportPlayer(Vector3 newPosition, Quaternion newRotation)
    {
        Debug.Log($"[Teleport RPC] to {newPosition}");

        var cc = GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            transform.SetPositionAndRotation(newPosition, newRotation);
            cc.enabled = true;
        }
        else
        {
            transform.SetPositionAndRotation(newPosition, newRotation);
        }
    }

    [PunRPC]
    public void TakeDamage(float damageAmount)
    {
        if (!PV.IsMine) return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

        if (currentHealth <= 0f)
        {
            Die();
        }

        PV.RPC("UpdateHealthForOthers", RpcTarget.Others, currentHealth);
    }

    [PunRPC]
    void UpdateHealthForOthers(float newHealth)
    {
        currentHealth = newHealth;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthbarFillImage != null)
            healthbarFillImage.fillAmount = currentHealth / maxHealth;

        if (healthText != null)
            healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
    }

    public float GetHealth() => currentHealth;
}
