using System.Runtime.InteropServices;
using UnityEngine;

public class Vibration
{
    public static void InitVibrate()
    {
#if Platform_Web
        Vibrate(500);
#else
        Handheld.Vibrate();
#endif
    }

#if Platform_Web
    [DllImport("__Internal")]
    private static extern void Vibrate(int ms);
#endif
}