using TMPro;
using UnityEngine;

public class PlayerCoinManager : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public int NumberOfCoins { get; private set; } = 0;

    public void AddOne()
    {
        NumberOfCoins++;
        UpdateUI();
    }

    public void ResetCoins()
    {
        NumberOfCoins = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (Text != null)
        {
            Text.text = NumberOfCoins.ToString();
        }
    }
}
