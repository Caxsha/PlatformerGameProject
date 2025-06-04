using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerCore : MonoBehaviourPunCallbacks
{
    [SerializeField] private Image healthbarImage;
    [SerializeField] private GameObject ui;
    [SerializeField] private PlayerCoinManager coinManager;

    private const float maxHealth = 100f;
    private float currentHealth = maxHealth;

    private PhotonView PV;
    private Animator animator;
    private PlayerManager playerManager;
    private int coins = 0;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        animator = GetComponentInChildren<Animator>();

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
            if (healthbarImage != null)
                healthbarImage.fillAmount = currentHealth / maxHealth;

            if (ui != null)
            {
                ui.SetActive(true);

                if (coinManager != null && coinManager.Text == null)
                {
                    coinManager.Text = ui.transform.Find("Text (TMP)")?.GetComponent<TextMeshProUGUI>();
                }
            }

            if (coinManager != null)
                coinManager.ResetCoins();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>()?.gameObject);
            Destroy(GetComponentInChildren<AudioListener>());
            if (ui != null) ui.SetActive(false);
        }
    }

    void Update()
    {
        if (!PV.IsMine) return;

        if (transform.position.y < -10f)
        {
            Die();
        }
    }

    void Die()
    {
        if (playerManager != null)
        {
            playerManager.Die();
        }
        else if (PV.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    public void AddCoin()
    {
        coins++;
        Debug.Log($"{photonView.Owner.NickName} собрал монету. Всего монет: {coins}");

        if (PV.IsMine && coinManager != null)
        {
            coinManager.AddOne();
        }
    }

    [PunRPC]
    public void DeclareVictory(string winnerName)
    {
        Debug.Log($"Финиш достигнут игроком: {winnerName}");

        if (PV.IsMine && animator != null)
        {
            animator.SetTrigger("Victory");
        }

        StartCoroutine(ReturnToMenu());
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
        transform.SetPositionAndRotation(newPosition, newRotation);
    }
}
