using UnityEngine;
using System.Linq;

public class CollisionEntity : BaseEntity
{
    public event System.Action<Collider> OnTriggerEvent = null;
    public event System.Action<Collision> OnCollisionEvent = null;


    private void OnTriggerEnter(Collider other)
    {
        CallEvent(OnTriggerEvent, other);

        LogMessage("Trigger");
    }

    private void OnCollisionEnter(Collision collision)
    {
        CallEvent(OnCollisionEvent, collision);

        LogMessage("Collision");
    }
}
