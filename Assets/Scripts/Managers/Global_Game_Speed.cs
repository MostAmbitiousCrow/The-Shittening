using UnityEngine;

public static class Global_Game_Speed
{
    public static float GetDeltaTime()
    {
        return Settings_Manager.gameSpeed * Time.deltaTime;
    }

    public static float GetUnscaledDeltaTime()
    {
        return Settings_Manager.gameSpeed * Time.unscaledDeltaTime;
    }

    public static float GetFixedDeltaTime()
    {
        return Settings_Manager.gameSpeed * Time.fixedDeltaTime;
    }

    public static float GetFixedUnscaledDeltaTime()
    {
        return Settings_Manager.gameSpeed * Time.fixedUnscaledDeltaTime;
    }
}
