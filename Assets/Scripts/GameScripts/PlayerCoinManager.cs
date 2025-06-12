using TMPro;
using UnityEngine;

public class PlayerCoinManager : MonoBehaviour
{
    public TextMeshProUGUI Text;

    private int coinCount = 0;

    public void SetCoins(int count)
    {
        coinCount = count;
        UpdateText();
    }

    private void UpdateText()
    {
        if (Text != null)
        {
            Text.text = coinCount.ToString();
        }
    }

    public void AddOne()
    {
        SetCoins(coinCount + 1);
    }

    public void ResetCoins()
    {
        SetCoins(0);
    }
}
