using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    Spawnpoint[] spawnpoints;

    void Awake()
    {
        Instance = this;
        spawnpoints = GetComponentsInChildren<Spawnpoint>();
    }

    public Transform GetSpawnpoint()
    {
        // Получаем ActorNumber локального игрока (начинается с 1)
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        // Возвращаем точку спавна, соответствующую номеру игрока
        return spawnpoints[playerIndex % spawnpoints.Length].transform;
    }
}
