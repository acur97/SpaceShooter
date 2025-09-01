#if Platform_Mobile
using Firebase.Analytics;
#endif

public class AnalyticsManager
{
    public static void Init()
    {
#if Platform_Mobile
        FirebaseAnalytics.LogEvent("app_open");
#endif
    }

    public static void Log_LevelStart()
    {
#if Platform_Mobile
        FirebaseAnalytics.LogEvent("level_start", "mode", RoundsController.Instance.levelType.ToString());
#endif
    }

    public static void Log_LevelEnd()
    {
#if Platform_Mobile
        FirebaseAnalytics.LogEvent("level_end", new Parameter[]
        {
            new("mode", RoundsController.Instance.levelType.ToString()),
            new("level", RoundsController.Instance.levelCount),
            new("round", RoundsController.Instance.roundCount),
            new("group", RoundsController.Instance.groupCount),
            new("score", GameManager.Instance.Score),
            new("revived", GameManager.Instance.isRevived.ToString()),
            new("earn_virtual_currency", PlayerProgress.GetCoins()),
            new("time_played", GameManager.Instance.finalTimeOfGameplay)
        });
#endif
    }

    public static void Log_PostScore()
    {
#if Platform_Mobile
        FirebaseAnalytics.LogEvent("post_score", new Parameter[]
        {
            new("mode", RoundsController.Instance.levelType.ToString()),
            new("score", GameManager.Instance.Score)
        });
#endif
    }

    public static void Log_BuyPowerUp(PowerUpsManager.PowerUpType type, uint cost)
    {
#if Platform_Mobile
        FirebaseAnalytics.LogEvent("power_up", new Parameter[]
        {
            new("buy", type.ToString()),
            new("spend_virtual_currency", cost)
        });
#endif
    }

    public static void Log_SelectPowerUp(PowerUpsManager.PowerUpType type, bool fromStore)
    {
#if Platform_Mobile
        FirebaseAnalytics.LogEvent("power_up", new Parameter[]
        {
            new("select", type.ToString()),
            new("from_store", fromStore.ToString())
        });
#endif
    }

    public static void Log_UsePowerUp(PowerUpsManager.PowerUpType type)
    {
#if Platform_Mobile
        FirebaseAnalytics.LogEvent("power_up", "use", type.ToString());
#endif
    }
}