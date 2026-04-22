using UnityEngine;
using TMPro;

public class VictoryUIManager : MonoBehaviour
{
    public static VictoryUIManager Instance;

    [SerializeField] private GameObject victoryCanvas;
    [SerializeField] private TextMeshProUGUI victoryText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            victoryCanvas.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowVictory(string winnerName)
    {
        victoryText.text = $"Победил: {winnerName}";
        victoryCanvas.SetActive(true);
    }
}
