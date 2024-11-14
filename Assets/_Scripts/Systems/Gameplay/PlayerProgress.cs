using UnityEngine;

public class PlayerProgress
{
    private struct Player_Progress
    {
        public string playerName;

        public int coins;

        public uint[] powerUpAmounts;
        public int powerUpIndex;

        public bool[] customsOwneds;
        public int customsIndex;
    }

    private static Player_Progress progress;

    private const string _progress = "PlayerProgress";

    public static void Init()
    {
        if (!PlayerPrefs.HasKey(_progress))
        {
            progress = new Player_Progress();
            return;
        }

        progress = JsonUtility.FromJson<Player_Progress>(PlayerPrefs.GetString(_progress));

        InitPowerUps();
        InitCustoms();
    }

    private static void InitPowerUps()
    {
        for (int i = 0; i < progress.powerUpAmounts.Length; i++)
        {
            PowerUpsManager.Instance.powerUps[i].currentAmount = progress.powerUpAmounts[i];
        }

        if (progress.powerUpIndex >= 0)
        {
            PowerUpsManager.Instance.selectedPowerUp = PowerUpsManager.Instance.powerUps[progress.powerUpIndex];
        }
    }

    private static void InitCustoms()
    {
        for (int i = 0; i < progress.customsOwneds.Length; i++)
        {
            GameManager.Instance.customs[i].owned = progress.customsOwneds[i];
        }

        if (progress.customsIndex >= 0)
        {
            GameManager.Instance.selectedCustoms = GameManager.Instance.customs[progress.customsIndex];
        }
    }

    public static void SaveAll()
    {
        SavePowerUps(false);
        SaveCustoms(false);

        WriteSaves();
    }

    private static void WriteSaves()
    {
        PlayerPrefs.SetString(_progress, JsonUtility.ToJson(progress));
        PlayerPrefs.Save();
    }

    public static void SavePowerUps(bool _writeSave)
    {
        progress.powerUpAmounts = new uint[PowerUpsManager.Instance.powerUps.Count];

        for (int i = 0; i < PowerUpsManager.Instance.powerUps.Count; i++)
        {
            progress.powerUpAmounts[i] = PowerUpsManager.Instance.powerUps[i].currentAmount;
        }

        progress.powerUpIndex = PowerUpsManager.Instance.powerUps.IndexOf(PowerUpsManager.Instance.selectedPowerUp);

        if (_writeSave)
        {
            WriteSaves();
        }
    }

    public static void SaveCustoms(bool _writeSave)
    {
        progress.customsOwneds = new bool[GameManager.Instance.customs.Count];

        for (int i = 0; i < GameManager.Instance.customs.Count; i++)
        {
            progress.customsOwneds[i] = GameManager.Instance.customs[i].owned;
        }

        progress.customsIndex = GameManager.Instance.customs.IndexOf(GameManager.Instance.selectedCustoms);

        if (_writeSave)
        {
            WriteSaves();
        }
    }

    public static void SavePlayerName(string name)
    {
        progress.playerName = name;
        WriteSaves();
    }

    public static string GetPlayerName()
    {
        return progress.playerName;
    }

    public static int UpCoins(int value)
    {
        return progress.coins += value;
    }

    public static int GetCoins()
    {
        return progress.coins;
    }
}