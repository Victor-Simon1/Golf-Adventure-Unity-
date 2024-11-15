using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBallController : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float deltaPos = 0.5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SimplePush(Vector3 dir, float force, bool b)
    {
        rb.AddForce(dir * force, ForceMode.Impulse);
    }

    public Vector3 GetLinePosition()
    {
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;
        return new Vector3(x, y-deltaPos, z);
    }
}
