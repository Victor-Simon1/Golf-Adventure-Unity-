using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBallController : MonoBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SimplePush(Vector3 dir, float force, bool b)
    {
        rb.AddForce(dir * force, ForceMode.Impulse);
    }
}
