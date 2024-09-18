using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BallControler : MonoBehaviour
{
    [Header("Control")]
    [Range(0.0f, 100.0f)]
    [SerializeField] private float scaleForce = 100f;
    [SerializeField] private float force;
    [SerializeField] private float AbsMagn;
    [SerializeField] private Rigidbody rb;
    private Vector3 sp;
    private Quaternion sr;

    [SerializeField] private Vector3 offset;

    private bool moving;
    private bool magnHasChanged;

    private Touch touch;
    [Header("Camera")]
    [SerializeField] private Camera cam;
    private Vector2 rotationValues = new Vector2(0,0);
    //La sensibilité n'est pas la même dans l'éditeur que sur téléphone
#if UNITY_EDITOR
    private float RotationSensitivity = 100f;
#else
    private float RotationSensitivity = 1f;
#endif
    float zoomLevel;
    float touchesPrevPosDifference, touchesCurPosDifference,zoomModifier;
    Vector2 firstTouchPrevPos, secondTouchPrevPos;
    [SerializeField]
    float zoomModifierSpeed = 0.1f;

    [Header("Button")]
  //  [SerializeField] private Button bStart;
    [SerializeField] private Button bPush;
    [Header("Slider")]
    [SerializeField] private Slider sliderForce;
    [Header("Gameplay")]
    private Vector3 lastPosition;//Position avant de tirer afin de pouvoir replacé la balle en cas de sortie de terrain
    private bool endFirstPut;//Pour remettre les collisions entre les balles
    [SerializeField] private LayerMask ballLayer;
    [Header("Movement")]
    private float limitForce = 0.5f;

    [SerializeField] private PlayerController pc;
    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        sp = transform.position;
        sr = transform.rotation;
       // rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        //bStart = GameObject.Find("ButtonStart").GetComponent<Button>();
        //bStart.onClick.AddListener(TpStart);

        bPush = GameObject.Find("ButtonPush").GetComponent<Button>();
        bPush.onClick.AddListener(Push);

        sliderForce = GameObject.Find("SliderForce").GetComponent <Slider>();

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

#if UNITY_EDITOR
        //Permet de tester l'orientation et le zomm dans l'éditeur
        if (Input.GetMouseButton(1))
        {
            var mouseMovement = new Vector2(-Input.GetAxis("Mouse Y") * 3f, Input.GetAxis("Mouse X") * 3f);
            rotationValues += mouseMovement * RotationSensitivity * Time.unscaledDeltaTime;
            rotationValues = new Vector2(Mathf.Clamp(rotationValues.x, -80f, 80f), rotationValues.y);

        }
        var zoomInput = -Input.GetAxis("Mouse ScrollWheel") * 10f;
        zoomLevel = Mathf.Clamp(zoomLevel + zoomInput, 1f, 10f);

#else
        //Permet de tester l'orientation et le zomm dans le télephone
        if(!sliderForce.GetComponent<SliderTouch>().isPressed)
        {
            if (Input.touchCount == 1)
            {
                touch = Input.GetTouch(0);

                if(touch.phase == TouchPhase.Moved)
                {
                    var mouseMovement = new Vector2(-touch.deltaPosition.y * 3f, touch.deltaPosition.x * 3f);
                    rotationValues += mouseMovement * RotationSensitivity * Time.unscaledDeltaTime;
                    rotationValues = new Vector2(Mathf.Clamp(rotationValues.x, -80f, 80f), rotationValues.y);
                }
            }
            else if (Input.touchCount == 2)
            {
                //Récuperation des deux entrés
                Touch firstTouch = Input.GetTouch(0);
                Touch secondTouch = Input.GetTouch(1);

                //Calcul des différences(savoir si on zoom/dezoom)
                firstTouchPrevPos = firstTouch.position - firstTouch.deltaPosition;
                secondTouchPrevPos = secondTouch.position - secondTouch.deltaPosition;

                touchesPrevPosDifference = (firstTouchPrevPos - secondTouchPrevPos).magnitude;
			    touchesCurPosDifference = (firstTouch.position - secondTouch.position).magnitude;

			    zoomModifier = (firstTouch.deltaPosition - secondTouch.deltaPosition).magnitude * zoomModifierSpeed;
                //Application
			    if (touchesPrevPosDifference > touchesCurPosDifference)
				    zoomLevel += zoomModifier;
			    if (touchesPrevPosDifference < touchesCurPosDifference)
				    zoomLevel-= zoomModifier;

                    zoomLevel = Mathf.Clamp (zoomLevel, 1f, 10f);
             }
         }

#endif
        var curRotation = Quaternion.Euler(rotationValues);
        var lookPosition = transform.position - (curRotation * Vector3.forward * zoomLevel);
        cam.transform.SetPositionAndRotation(lookPosition, curRotation);
    }

    private void OnEnable()
    {
        cam = Camera.main;
        rotationValues = new Vector2(15, 0);
        zoomLevel = 10;
    }

    public void Push()
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.Play();
        lastPosition = transform.position;
        var vec = cam.transform.forward;
        force = sliderForce.value*scaleForce;
        vec = new Vector3(vec.x, 0, vec.z);
        pc.PushBall(vec, force);
       // rb.AddForce(vec * force, ForceMode.Impulse);
        //rb.velocity = vec * force;
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

    public void IgnoreBalls()
    {
        GetComponent<SphereCollider>().excludeLayers = 3;
    }

    public void DontIgnoreBalls()
    {
        GetComponent<SphereCollider>().excludeLayers = 0;
    }

   /* private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(transform.parent.name+ " collide with " + collision.transform.parent.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log(transform.parent.name + " : " + rb.velocity );
            //Debug.Log(collision.transform.parent.name + " : " );
            //Debug.Log("Normal : " + collision.contacts[0].normal);
            //Vector3 dir = collision.contacts[0].point - transform.position;
            //Vector3 forceToChange = rb.velocity ;
            //Debug.Log("Velocity" + rb.velocity);
            //  rb.AddForce(-dir.normalized * collision.transform.GetComponent<Rigidbody>().velocity.magnitude, ForceMode.VelocityChange);
            // Vector3 newVel = (rb.mass * collision.transform.GetComponent<Rigidbody>().velocity)/(rb.mass + collision.transform.GetComponent<Rigidbody>().mass) ;
            //rb.velocity = Vector3.zero;
            //collision.transform.GetComponent<Rigidbody>().AddForce(collision.contacts[0].normal*2, ForceMode.VelocityChange);
            Rigidbody otherRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (otherRigidbody != null)
            {
                Vector3 direction = collision.contacts[0].point - transform.position;
                direction = -direction.normalized;
                float forceRb = rb.velocity.magnitude;
                Debug.Log("Speed " + forceRb);
                otherRigidbody.AddForce(direction * forceRb, ForceMode.Impulse);
                rb.velocity = Vector3.zero;
            }
        }
    }*/
}
