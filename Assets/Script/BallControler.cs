using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallControler : MonoBehaviour//NetworkBehaviour
{
    [Range(0.0f, 100.0f)]
    [SerializeField] private float force;
    private Rigidbody rb;
    public Vector3 sp;
    public Quaternion sr;
    
    // Start is called before the first frame update
    void Start()
    {
        sp = transform.position;
        sr = transform.rotation;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Test
    public void Avance(InputAction.CallbackContext context)
    {
        //if (!base.Owner)
          //  return;
        transform.position += new Vector3(1, 0, 0);
        AvanceServer();
    }

    //[ServerRpc]
    private void AvanceServer()
    {
        transform.position += new Vector3(1, 0, 0);
    }

    public void Push()
    {
        rb.AddForce(Vector3.left * force, ForceMode.Impulse);
    }

    public void TpStart()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.rotation = sr;
        transform.position = sp;
    }
}
