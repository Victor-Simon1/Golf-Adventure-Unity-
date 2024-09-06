using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class BallControler : NetworkBehaviour
{
    [Header("Control")]
    [Range(0.0f, 100.0f)]
    [SerializeField] private float force;
    [SerializeField] private float AbsMagn;
    private Rigidbody rb;
    private Vector3 sp;
    private Quaternion sr;

    [SerializeField] private Vector3 offset;

    private bool moving;
    private bool magnHasChanged;

    private Touch touch;
    [Header("Camera")]
    [SerializeField] private Camera cam;
    private Vector2 rotationValues;
    //La sensibilité n'est pas la même dans l'éditeur que sur téléphone
#if UNITY_EDITOR
    private float RotationSensitivity = 100f;
#else
    private float RotationSensitivity = 1f;
#endif
    float zoomLevel;
    [Header("Button")]
    [SerializeField] private Button bStart;
    [SerializeField] private Button bPush;
    [Header("Slider")]
    [SerializeField] private Slider sliderForce;
    [Header("Gameplay")]
    private Vector3 lastPosition;//Position avant de tirer afin de pouvoir replacé la balle en cas de sortie de terrain
    [Header("Movement")]
    private float limitForce = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        sp = transform.position;
        sr = transform.rotation;
        rb = GetComponent<Rigidbody>();

        bStart = GameObject.Find("ButtonStart").GetComponent<Button>();
        bStart.onClick.AddListener(TpStart);

        bPush = GameObject.Find("ButtonPush").GetComponent<Button>();
        bPush.onClick.AddListener(Push);

        sliderForce = GameObject.Find("Slider").GetComponent <Slider>();

        cam = transform.parent.GetChild(0).GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

        AbsMagn = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z);

        if(!magnHasChanged && AbsMagn > 0.1) 
            magnHasChanged = true;
        if(magnHasChanged && AbsMagn == 0) 
            magnHasChanged = false;

        if (moving && magnHasChanged && AbsMagn < limitForce) 
            Stopped();
        

        if (Input.GetMouseButton(1))
        {
            var mouseMovement = new Vector2(-Input.GetAxis("Mouse Y") * 3f, Input.GetAxis("Mouse X") * 3f);
            rotationValues += mouseMovement * RotationSensitivity * Time.unscaledDeltaTime;
            rotationValues = new Vector2(Mathf.Clamp(rotationValues.x, -80f, 80f), rotationValues.y);

        }

        if (Input.touchCount > 0)
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

    [Client]
    public void Push()
    {
        var vec = cam.transform.forward;
        force = sliderForce.value;
        vec = new Vector3(vec.x, 0, vec.z);
        rb.AddForce(vec * force, ForceMode.Impulse);
        //rb.velocity = vec * force;
        moving = true;
        CmdAddStrokes(transform.parent.name);
    }
    [Command]
    public void CmdAddStrokes(string sourceId)
    {

        Debug.Log(sourceId + " a tiré");
        Player player = GameManager.GetPlayer(sourceId);
        player.RpcAddStroke(sourceId);
    }
    [Client]
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
