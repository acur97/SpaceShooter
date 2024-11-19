using System;
using UnityEngine;

public class PlayerProgress
{
    [Serializable]
    public struct Player_Progress
    {
        public string playerName;

        public int coins;

        public uint[] powerUpAmounts;
        public int powerUpIndex;

        public bool[] customsOwneds;
        public int customsIndex;
    }

    private static GameplayScriptable scriptable;

    private const string _progress = "PlayerProgress";

    public static void Init(GameplayScriptable _scriptable)
    {
        scriptable = _scriptable;

        if (!PlayerPrefs.HasKey(_progress))
        {
            scriptable.progress = new Player_Progress();
            return;
        }

        scriptable.progress = JsonUtility.FromJson<Player_Progress>(PlayerPrefs.GetString(_progress));

        InitPowerUps();
        InitCustoms();
    }

    private static void InitPowerUps()
    {
        for (int i = 0; i < scriptable.progress.powerUpAmounts.Length; i++)
        {
            scriptable.powerUps[i].currentAmount = scriptable.progress.powerUpAmounts[i];
        }

        if (scriptable.progress.powerUpIndex >= 0)
        {
            PowerUpsManager.Instance.SelectPowerUp(scriptable.powerUps[scriptable.progress.powerUpIndex]);
        }
        else
        {
            PowerUpsManager.Instance.SelectPowerUp(null);
        }
    }

    private static void InitCustoms()
    {
        for (int i = 0; i < scriptable.progress.customsOwneds.Length; i++)
        {
            scriptable.customs[i].owned = scriptable.progress.customsOwneds[i];
        }

        if (scriptable.progress.customsIndex >= 0)
        {
            scriptable.selectedCustoms = scriptable.customs[scriptable.progress.customsIndex];
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
        PlayerPrefs.SetString(_progress, JsonUtility.ToJson(scriptable.progress));
        PlayerPrefs.Save();
    }

    public static void SavePowerUps(bool _writeSave)
    {
        scriptable.progress.powerUpAmounts = new uint[scriptable.powerUps.Count];

        for (int i = 0; i < scriptable.powerUps.Count; i++)
        {
            scriptable.progress.powerUpAmounts[i] = scriptable.powerUps[i].currentAmount;
        }

        scriptable.progress.powerUpIndex = scriptable.powerUps.IndexOf(scriptable.selectedPowerUp);

        if (_writeSave)
        {
            WriteSaves();
        }
    }

    public static void SaveCustoms(bool _writeSave)
    {
        scriptable.progress.customsOwneds = new bool[scriptable.customs.Count];

        for (int i = 0; i < scriptable.customs.Count; i++)
        {
            scriptable.progress.customsOwneds[i] = scriptable.customs[i].owned;
        }

        scriptable.progress.customsIndex = scriptable.customs.IndexOf(scriptable.selectedCustoms);

        if (_writeSave)
        {
            WriteSaves();
        }
    }

    public static void SavePlayerName(string name)
    {
        scriptable.progress.playerName = name;
        WriteSaves();
    }

    public static string GetPlayerName()
    {
        return scriptable.progress.playerName;
    }

    public static int UpCoins(int value)
    {
        return scriptable.progress.coins += value;
    }

    public static int GetCoins()
    {
        return scriptable.progress.coins;
    }
}