using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Managing.Timing;
using static UnityEngine.GraphicsBuffer;

public class BallControler : NetworkBehaviour
{
    [Range(0.0f, 100.0f)]
    [SerializeField] private float force;
    [SerializeField] private float AbsMagn;
    private Rigidbody rb;
    private Vector3 sp;
    private Quaternion sr;
    [SerializeField] private Camera cam;

    [SerializeField] private Vector3 offset;

    private bool moving;
    private bool magnHasChanged;

    float zoomLevel;
    private Vector2 rotationValues;
    private float RotationSensitivity = 1f;

    private Touch touch;

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

        AbsMagn = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z);

        if(!magnHasChanged && AbsMagn > 0.1) magnHasChanged = true;
        if(magnHasChanged && AbsMagn == 0) magnHasChanged = false;

        if (moving && magnHasChanged && AbsMagn < 1) Stopped();
        

        if (Input.GetMouseButton(1))
        {
            var mouseMovement = new Vector2(-Input.GetAxis("Mouse Y") * 3f, Input.GetAxis("Mouse X") * 3f);
            
        }

        if(Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Moved)
            {
                var mouseMovement = new Vector2(-touch.deltaPosition.y * 3f, touch.deltaPosition.x * 3f);
                rotationValues += mouseMovement * RotationSensitivity * Time.unscaledDeltaTime;
                rotationValues = new Vector2(Mathf.Clamp(rotationValues.x, -80f, 80f), rotationValues.y);
            }
        }

        var zoomInput = -Input.GetAxis("Mouse ScrollWheel") * 10f;
        zoomLevel = Mathf.Clamp(zoomLevel + zoomInput, 1f, 10f);

        var curRotation = Quaternion.Euler(rotationValues);
        var lookPosition = transform.position - (curRotation * Vector3.forward * zoomLevel);
        cam.transform.SetPositionAndRotation(lookPosition, curRotation);
    }

    private void OnEnable()
    {
        cam = Camera.main;
        rotationValues = transform.position + offset;
        zoomLevel = 10;
    }

    public void Push()
    {
        var vec = cam.transform.forward;
        vec = new Vector3(vec.x, 0, vec.z);
        rb.AddForce(vec * force, ForceMode.Impulse);
        moving = true;
    }

    public void TpStart()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.rotation = sr;
        transform.position = sp;
    }

    private void Stopped()
    {
        moving = false;
        Debug.Log("Ball stopped");
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void rotateLeft()
    {
        cam.transform.RotateAround(transform.position, Vector3.up, cam.transform.eulerAngles.y - transform.eulerAngles.y);
    }

    public void rotateLeft(float angle)
    {
        cam.transform.RotateAround(transform.position, Vector3.up, angle);
    }
}
