using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    public static PlayerProgress Instance;

    private static int coins = 0;

    private const string _coins = "Coins";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }

        Instance = this;

        coins = PlayerPrefs.GetInt(_coins, 0);
    }

    public static int UpCoins(int value)
    {
        coins += value;
        PlayerPrefs.SetInt(_coins, coins);
        return coins;
    }

    public static int GetCoins()
    {
        return coins;
    }
}