using UnityEngine;

public class TrashPickupEffect : MonoBehaviour
{
    public void AnimationEnd()
    {
        Destroy(gameObject);
    }
}
