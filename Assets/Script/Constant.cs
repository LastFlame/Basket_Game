public static class Constant
{
    public const string MOVING_SCORE_TEMPLATE = "+{0} pts!";

    public enum ShotScores
    {
        PERFECT = 3,
        NORMAL = 2,
        NORMAL_BONUS = 4,
        EXTRA_BONUS = 5
    }

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
