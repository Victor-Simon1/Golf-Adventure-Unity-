using UnityEngine;
using UnityEngine.UI;
using Services;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
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
    public bool hasFinishHole = false;

    [SerializeField] private bool isOutOfLimit = false;
    [SerializeField] private bool isOnGreen = true;
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
    [SerializeField] private SliderTouch sliderTouch;
    [Header("Gameplay")]
    private Vector3 lastPosition;//Position avant de tirer afin de pouvoir replacé la balle en cas de sortie de terrain
    private bool endFirstPut;//Pour remettre les collisions entre les balles
    [SerializeField] private LayerMask ballLayer;
    [SerializeField] private float timeOutLimit = 0f;
    private float MaxTimeOutOfLimit = 5f;
    [Header("Movement")]
    private float limitForce = 0.5f;

    [SerializeField] private PlayerController pc;
    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource goodHoleSound;
    [SerializeField] private AudioSource badHoleSound;
    [Header("UI")]
    [SerializeField] private GameObject resultHoleText;
    [Header("Slope")]
    public Transform rearRayPos;
    public Transform frontRayPos;
    public LayerMask layerMask;

    public float surfaceAngle;
    public bool uphill;
    public bool flatSurface;

    void Start()
    {
    }
    private void OnEnable()
    {
        sp = transform.position;
        sr = transform.rotation;

        var temp = GameObject.Find("ButtonPush");
        if (temp == null)
            Debug.LogError("Error the variable temp is not assigned");

        bPush = temp.GetComponent<Button>();
        if (bPush == null)
            Debug.LogError("Error the variable bPush is not assigned");
        bPush.onClick.AddListener(Push);

        sliderForce = GameObject.Find("SliderForce").GetComponent<Slider>();
        if (sliderForce == null)
            Debug.LogError("Error the variable sliderForce is not assigned");

        sliderTouch = GameObject.Find("SliderForce").GetComponent<SliderTouch>();
        if (sliderTouch == null)
            Debug.LogError("Error the variable sliderTouch is not assigned");

        cam = transform.parent.parent.GetChild(0).GetComponent<Camera>();
        if (cam == null)
            Debug.LogError("Error the variable cam is not assigned");

        resultHoleText = GameObject.Find("ResultHole").transform.GetChild(0).gameObject;
        if (resultHoleText == null)
            Debug.LogError("Error the variable resultHoleText is not assigned");
        rotationValues = new Vector2(15, 0);
        zoomLevel = 10;
    }
    // Update is called once per frame
    void Update()
    {
        //Si on est en dehors des limits, tp dans le temps donné
        if (isOutOfLimit)
            timeOutLimit += Time.deltaTime;
        if(timeOutLimit>MaxTimeOutOfLimit)
        {
            TpToLastLocation();
        }
        //Speed
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
        if(!sliderTouch.isPressed)
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
        if (cam)
            cam.transform.SetPositionAndRotation(lookPosition, curRotation);

        //Detecting slope
        float camX = cam.transform.forward.x/7f;
        float camZ = cam.transform.forward.z/7f;

        frontRayPos.position = transform.position + new Vector3(camX, 0, camZ);
        rearRayPos.position= transform.position + new Vector3(-camX, 0, -camZ);
        {
            RaycastHit rearHit;
            if (Physics.Raycast(rearRayPos.position, rearRayPos.TransformDirection(-Vector3.up), out rearHit, 1f, layerMask))
            {
                surfaceAngle = Vector3.Angle(rearHit.normal, Vector3.up);
            }
            else
            {
                uphill = false;
                flatSurface = false;
            }

            RaycastHit frontHit;
            Vector3 frontRayStartPos = new Vector3(frontRayPos.position.x, frontRayPos.position.y, frontRayPos.position.z);
            if (Physics.Raycast(frontRayStartPos, frontRayPos.TransformDirection(-Vector3.up), out frontHit, 1f, layerMask))
            {
            }
            else
            {
                uphill = true;
                flatSurface = false;
            }
            if(Mathf.Abs(rearHit.distance - frontHit.distance) < 0.02f)
            {
                flatSurface = true;
                uphill = false;
            }
            else if (frontHit.distance < rearHit.distance)
            {
                uphill = true;
                flatSurface = false;
            }
            else if (frontHit.distance > rearHit.distance)
            {
                uphill = false;
                flatSurface = false;
            }
       }
       
    }

    public void Push()
    {
        if (hasFinishHole || isOutOfLimit)
            return;
        DoSound();
        lastPosition = transform.position;
        var vec = cam.transform.forward;
        force = sliderForce.value*scaleForce;
        float sensY = rb.velocity.normalized.y;
        if(uphill)
            sensY = Mathf.Abs(sensY);
        if (flatSurface)
            sensY = 0f;
        vec = new Vector3(vec.x, sensY, vec.z);
        pc.PushBall(vec, force);
        moving = true;
    }
    private void DoSound()
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.Play();
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
        if(isOnGreen)
        {
            moving = false;
            Debug.Log("Ball stopped");
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        if(isOutOfLimit) 
        {
            TpToLastLocation();
        }
    }

    public void SetLastPosition(Vector3 position)
    {
        lastPosition = position;
    }
    private void TpToLastLocation()
    {
        Debug.Log("En dehors des limites... Tps vers la dernieres positions");
        pc.TpToLocation(lastPosition);
        isOutOfLimit = false;
        timeOutLimit = 0f;
    }
    public void IgnoreBalls()
    {
        Debug.Log("J'ignore les autres balls");
        GetComponent<SphereCollider>().excludeLayers = ballLayer;
    }

    public void DontIgnoreBalls()
    {
        GetComponent<SphereCollider>().excludeLayers = 0;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            isOutOfLimit = true;
        }
        if (collision.transform.CompareTag("Green"))
        {
            isOnGreen = true;
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Une balle est rentré :" + pc.GetName());
        HoleBehavior hole = other.transform.parent.GetComponent<HoleBehavior>();
        if (hole != null)
        {
            
            pc.hasFinishHole = true;
            if (ServiceLocator.Get<GameManager>().GetLocalPlayer().netIdentity == pc.netIdentity)
            {
                goodHoleSound.Play();
                StartCoroutine(CouroutineShowResultHole(pc.GetActualStrokes(), hole.maxStrokes));
            }
            StartCoroutine(ServiceLocator.Get<GameManager>().GoNextHole());
        }
     
    }
    private string GetTextResultHole(int actualStrokes, int maxStrokes)
    {
        int result = actualStrokes- maxStrokes;

        if (actualStrokes == 1)
           return "HOLE IN ONE";
        else if (result == -4)
            return "CONDOR";
        else if (result == -3)
            return "ALBATROS";
        else if (result == -2)
            return "EAGLE";
        else if (result == -1)
            return "BIRDIE";
        else if (result == 0)
            return "PAR";
        else if (result == 1)
            return "BOGEY";
        else if (result == 2)
            return "DOUBLE BOGEY";
        else
            return "X BOGEY";
    }
    private IEnumerator CouroutineShowResultHole(int actualStrokes,int maxStrokes)
    {
        yield return null;
        string result = GetTextResultHole(actualStrokes, maxStrokes);
        resultHoleText.GetComponent<TextMeshProUGUI>().text = result;
        resultHoleText.SetActive(true);
        resultHoleText.GetComponent<Animator>().Play("New Animation");
        yield return new WaitForSeconds(1f);
        resultHoleText.SetActive(false);
    }

}
