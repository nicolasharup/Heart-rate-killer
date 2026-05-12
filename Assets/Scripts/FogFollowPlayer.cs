using UnityEngine;

public class FogFollowPlayer : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 0.2f, 0f);

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + offset;
    }
}