using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ScoreboardItem : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text deathsText;

    private Player player;

    public void Initialize(Player player)
    {
        this.player = player;

        if (usernameText != null)
        {
            usernameText.text = player.NickName;
            usernameText.color = Color.white; // Белый цвет для ника
        }

        if (deathsText != null)
        {
            deathsText.color = Color.red; // Красный цвет для смертей
        }

        UpdateStats();
    }

    void UpdateStats()
    {
        if (deathsText != null)
        {
            if (player.CustomProperties.TryGetValue("deaths", out object deaths))
            {
                deathsText.text = deaths.ToString();
            }
            else
            {
                deathsText.text = "0";
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer == player && changedProps.ContainsKey("deaths"))
        {
            UpdateStats();
        }
    }

    public int GetDeaths()
    {
        if (player.CustomProperties.TryGetValue("deaths", out object deaths))
        {
            return (int)deaths;
        }
        return 0;
    }
}