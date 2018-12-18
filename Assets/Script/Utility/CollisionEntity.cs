using UnityEngine;
using System.Linq;

public class CollisionEntity : BaseEntity
{
    public event System.Action<Collider> OnTriggerEvent = null;
    public event System.Action<Collision> OnCollisionEvent = null;


    private void OnTriggerEnter(Collider other)
    {
        CallEvent(OnTriggerEvent, other);

#if UNITY_EDITOR
        LogMessage("Trigger");
#endif
    }

    private void OnCollisionEnter(Collision collision)
    {
        CallEvent(OnCollisionEvent, collision);

#if UNITY_EDITOR
        LogMessage("Collision");
#endif
    }
}
