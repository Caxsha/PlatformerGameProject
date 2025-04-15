using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] Image healthbarImage;
    [SerializeField] GameObject ui;

    [SerializeField] GameObject cameraHolder;

    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;

    float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    Rigidbody rb;
    PhotonView PV;
    const float maxHealth = 100f;
    float currentHealth = maxHealth;

    PlayerManager playerManager;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();

        if (PV.InstantiationData != null && PV.InstantiationData.Length > 0)
        {
            int playerManagerViewID = (int)PV.InstantiationData[0];
            var playerManagerPhotonView = PhotonView.Find(playerManagerViewID);
            if (playerManagerPhotonView != null)
            {
                playerManager = playerManagerPhotonView.GetComponent<PlayerManager>();
                if (playerManager == null)
                {
                    Debug.LogError($"PlayerManager component not found on PhotonView with ViewID {playerManagerViewID}", this);
                }
            }
            else
            {
                Debug.LogError($"PhotonView with ViewID {playerManagerViewID} not found!", this);
            }
        }
        else
        {
            Debug.LogError("InstantiationData is null or empty in PlayerController! Make sure PlayerController is spawned with InstantiationData containing PlayerManager's ViewID.", this);
        }
    }

    void Start()
    {
        if (PV.IsMine)
        {
            currentHealth = maxHealth;
            if (healthbarImage != null)
                healthbarImage.fillAmount = currentHealth / maxHealth;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(GetComponentInChildren<AudioListener>());
            Destroy(rb);
            Destroy(ui);
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

        Look();
        Move();
        Jump();

        if (transform.position.y < -10f)
        {
            Die();
        }
    }

    void Look()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

            verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
            verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

            cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
        }
    }

    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    void FixedUpdate()
    {
        if (!PV.IsMine)
            return;

        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    void Die()
    {
        if (playerManager != null)
        {
            playerManager.Die();
        }
        else
        {
            Debug.LogWarning("PlayerManager is null, handling death locally.");
            if (PV.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    [PunRPC]
    public void DeclareVictory(string winnerName)
    {
        Debug.Log($"Финиш достигнут игроком: {winnerName}");
        Debug.Log($"Победитель: {winnerName}");

        StartCoroutine(ReturnToMenu());
    }

    private IEnumerator ReturnToMenu()
    {
        yield return new WaitForSeconds(5f);

        // Уничтожаем RoomManager синхронно
        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.DestroyInstance();
        }

        // Ждем, пока RoomManager уничтожится
        yield return new WaitUntil(() => RoomManager.Instance == null);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}