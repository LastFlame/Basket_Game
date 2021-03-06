﻿public static class Constant
{
    public enum ShotScores
    {
        PERFECT = 3,
        NORMAL = 2,
        NORMAL_BONUS = 4,
        EXTRA_BONUS = 5
    }

    /// <summary>
    /// This enum matchs the build index of your scene on Unity.
    /// </summary>
    public enum GameScenes
    {
        MAIN_MENU,
        SOLO_PRATICE,
        SHOOTING_RACE,
        REWARD
    }

    public enum AIDifficulty
    {
        EASY,
        MEDIUM,
        HARD
    }

    public const string MOVING_SCORE_TEMPLATE = "+{0} pts!";
    public const int ON_FIRE_SCORE_MULTIPLIER = 2;
    public const int BET_MONEY = 1000;

    public static int PLAYER_MONEY = 1000;

    public static UnityEngine.Vector3 GetProjectileVelocity(UnityEngine.Vector3 destinationV3, UnityEngine.Vector3 startV3, float maxHeight)
    {
        float deltaY = destinationV3.y - startV3.y;

        UnityEngine.Vector3 deltaZX = new UnityEngine.Vector3(destinationV3.x - startV3.x, 0, destinationV3.z - startV3.z);

        float gravity = UnityEngine.Physics.gravity.y;

        float time = UnityEngine.Mathf.Sqrt(-2 * maxHeight / gravity) + UnityEngine.Mathf.Sqrt(2 * (deltaY - maxHeight) / gravity);

        UnityEngine.Vector3 velocityY = UnityEngine.Vector3.up * UnityEngine.Mathf.Sqrt(-2 * gravity * maxHeight);

        UnityEngine.Vector3 velocityZ = (UnityEngine.Vector3.forward * deltaZX.magnitude) / time;

        return velocityZ + velocityY * -UnityEngine.Mathf.Sign(gravity);
    }

    public static float NormalizedData (float value, float min, float max)
    {
        return (value - min) / (max - min);
    }
}
