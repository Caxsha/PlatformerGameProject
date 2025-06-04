using Photon.Pun;
using UnityEngine;

public class CoinSpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    private Spawnpoint[] coinSpawnPoints;

    void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        coinSpawnPoints = GetComponentsInChildren<Spawnpoint>();
        foreach (var spawnPoint in coinSpawnPoints)
        {
            PhotonNetwork.Instantiate(coinPrefab.name, spawnPoint.transform.position, spawnPoint.transform.rotation);
        }
    }
}
