using Firebase.Analytics;

public class AnalyticsManager
{
    public static void Init()
    {
        FirebaseAnalytics.LogEvent("app_open");
    }

    public static void Log_LevelStart()
    {
        FirebaseAnalytics.LogEvent("level_start", "mode", RoundsController.Instance.levelType.ToString());
    }

    public static void Log_LevelEnd()
    {
        FirebaseAnalytics.LogEvent("level_end", new Parameter[]
        {
            new("mode", RoundsController.Instance.levelType.ToString()),
            new("level", RoundsController.Instance.levelCount),
            new("round", RoundsController.Instance.roundCount),
            new("group", RoundsController.Instance.groupCount),
            new("score", GameManager.Instance.score),
            new("revived", GameManager.Instance.isRevived.ToString()),
            new("coins_collected", PlayerProgress.GetCoins()),
            new("time_played", GameManager.Instance.finalTimeOfGameplay)
        });
    }

    public static void Log_PostScore()
    {
        FirebaseAnalytics.LogEvent("post_score", new Parameter[]
        {
            new("mode", RoundsController.Instance.levelType.ToString()),
            new("score", GameManager.Instance.score)
        });
    }

    public static void Log_BuyPowerUp(PowerUpsManager.PowerUpType type)
    {
        FirebaseAnalytics.LogEvent("power_up", "buy", type.ToString());
    }

    public static void Log_SelectPowerUp(PowerUpsManager.PowerUpType type, bool fromStore)
    {
        FirebaseAnalytics.LogEvent("power_up", new Parameter[]
        {
            new("select", type.ToString()),
            new("from_store", fromStore.ToString())
        });
    }

    public static void Log_UsePowerUp(PowerUpsManager.PowerUpType type)
    {
        FirebaseAnalytics.LogEvent("power_up", "use", type.ToString());
    }
}