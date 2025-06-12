using Photon.Pun;
using UnityEngine;

public class Coin : MonoBehaviourPun
{
    [SerializeField] private float _rotationSpeed = 360f;
    [SerializeField] private GameObject _effectPrefab;

    private Collider coinCollider;
    private Renderer[] renderers;

    void Awake()
    {
        coinCollider = GetComponent<Collider>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        transform.Rotate(0, _rotationSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        PhotonView playerView = player.GetComponent<PhotonView>();
        if (playerView == null) return;

        DisableCoin();

        playerView.RPC("AddCoin", playerView.Owner);

        if (_effectPrefab != null)
        {
            photonView.RPC("SpawnEffect", RpcTarget.All, transform.position, transform.rotation);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            photonView.RPC("RequestDestroyCoin", RpcTarget.MasterClient);
        }
    }

    private void DisableCoin()
    {
        if (coinCollider != null)
            coinCollider.enabled = false;

        foreach (var r in renderers)
            r.enabled = false;
    }

    [PunRPC]
    void SpawnEffect(Vector3 pos, Quaternion rot)
    {
        if (_effectPrefab != null)
        {
            Instantiate(_effectPrefab, pos, rot);
        }
    }

    [PunRPC]
    void RequestDestroyCoin()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
