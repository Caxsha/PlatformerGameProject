using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable; // Добавляем директиву

public class Scoreboard : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform container;
    [SerializeField] GameObject scoreboardItemPrefab;
    [SerializeField] CanvasGroup canvasGroup;

    Dictionary<Player, ScoreboardItem> scoreboardItems = new Dictionary<Player, ScoreboardItem>();

    void Start()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            AddScoreboardItem(player);
        }

        // Изначально скрываем табло
        canvasGroup.alpha = 0;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboardItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreboardItem(otherPlayer);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("deaths"))
        {
            UpdateStats();
        }
    }

    void AddScoreboardItem(Player player)
    {
        ScoreboardItem item = Instantiate(scoreboardItemPrefab, container).GetComponent<ScoreboardItem>();
        item.Initialize(player);
        scoreboardItems[player] = item;
        UpdateStats();
    }

    void RemoveScoreboardItem(Player player)
    {
        if (scoreboardItems.ContainsKey(player))
        {
            Destroy(scoreboardItems[player].gameObject);
            scoreboardItems.Remove(player);
        }
    }

    void UpdateStats()
    {
        // Сортируем элементы по смертям (по убыванию)
        var sortedItems = scoreboardItems.OrderByDescending(x => x.Value.GetDeaths()).ToList();
        for (int i = 0; i < sortedItems.Count; i++)
        {
            sortedItems[i].Value.transform.SetSiblingIndex(i);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            canvasGroup.alpha = 1;
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            canvasGroup.alpha = 0;
        }
    }
}