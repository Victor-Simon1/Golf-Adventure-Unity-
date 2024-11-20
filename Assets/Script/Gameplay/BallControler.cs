using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class BallControler : MonoBehaviour
{
    [Header("Control")]
    [Range(0.0f, 100.0f)]
    [SerializeField] private float scaleForce = 25f;
    [SerializeField] private float force;
    [SerializeField] private float AbsMagn;
    [SerializeField] private Rigidbody rb;
    private Vector3 sp;
    private Quaternion sr;
    public bool hasFinishHole = false;

    private bool isVisualized = false;

    [SerializeField] private Vector3 offset;

    public bool moving;

    [SerializeField] private bool magnHasChanged;

    private Touch touch;

    [Header("Camera")]
    [SerializeField] private Camera cam;

    [Header("Behaviour")]
    [SerializeField] private MeshRenderer mr;

    [SerializeField] private Transform lineVisual;

    [SerializeField] private Vector2 rotationValues = new Vector2(0,0);
    //La sensibilit� n'est pas la m�me dans l'�diteur que sur t�l�phone
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
    [SerializeField] private Button bPush;

    [Header("Slider")]
    [SerializeField] private Slider sliderForce;
    [SerializeField] private SliderTouch sliderTouch;

    [Header("Gameplay")]
    [SerializeField] private Vector3 lastPosition;//Position avant de tirer afin de pouvoir replac� la balle en cas de sortie de terrain
    private bool endFirstPut;//Pour remettre les collisions entre les balles
    [SerializeField] private LayerMask ballLayer;
    [SerializeField] private float timeOutLimit = 0f;
    private float MaxTimeOutOfLimit = 3f;

    [SerializeField] private bool isOutOfLimit = false;
    [SerializeField] private bool isOnGreen = true;

    private int maxStrokes = 12;
    private int penalityStrokes = 3;
    private bool hasAddPenality = false;

    public Coroutine timeLimitCoroutine;
    [Header("Movement")]
    [SerializeField] private float limitForce = 0.35f;
    [SerializeField] private float maxAngularVelocity = 0.9f;
    [SerializeField] private float coeffAngularVelocity = 0.9f;
    [SerializeField] private PlayerController pc;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource badHoleSound;

    [Header("Slope")]
    public Transform rearRayPos;
    public Transform frontRayPos;
    public LayerMask layerMask;

    public float surfaceAngle;
    public bool uphill;
    public bool flatSurface;

    private bool firstEnable = true;

    #region UNITY_FUNCTION
    void Start()
    {
        rb.maxAngularVelocity = 1000;
        rotationValues = new Vector2(15, 0);
        //rb.inertiaTensor = new Vector3(0, 0, 0);
        //rb.inertiaTensorRotation = new Quaternion(0.1f, 0.1f, 0.1f, 0f);
    }

    private void OnEnable()
    {
        sp = transform.position;
        sr = transform.rotation;
        if(pc.isLocalPlayer)
        {
            lineVisual.gameObject.SetActive(true);
            if (!hasFinishHole && firstEnable)
            {


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
                firstEnable = false;
            }
        }
        
        rotationValues = new Vector2(15, 0);
        
        zoomLevel = 10;

        InvokeRepeating("DetectSlope", 0.1f, 0.3f);
    }
    private void OnDisable()
    {
        if (lineVisual.gameObject.activeSelf)
            lineVisual.gameObject.SetActive(false);
        CancelInvoke("DetectSlope");
    }
    // Update is called once per frame
    void Update()
    {
        if (isVisualized)
        {
            //Si on est en dehors des limits, tp dans le temps donn�
            if (isOutOfLimit)
            {
                timeOutLimit += Time.deltaTime;
                if (timeOutLimit > MaxTimeOutOfLimit)
                {
                    TpToLastLocation();
                }
            }
          

#if UNITY_EDITOR
            //Permet de tester l'orientation et le zomm dans l'�diteur
            if (Input.GetMouseButton(1))
            {
                var mouseMovement = new Vector2(-Input.GetAxis("Mouse Y") * 3f, Input.GetAxis("Mouse X") * 3f);
                rotationValues += mouseMovement * RotationSensitivity * Time.unscaledDeltaTime;
                rotationValues = new Vector2(Mathf.Clamp(rotationValues.x, /*-80f*/10f, 80f), rotationValues.y);
            }
            var zoomInput = -Input.GetAxis("Mouse ScrollWheel") * 10f;
            zoomLevel = Mathf.Clamp(zoomLevel + zoomInput, 1f, 10f);

#else
        //Permet de tester l'orientation et le zomm dans le t�lephone
        if(pc.isLocalPlayer ? !sliderTouch.isPressed : true)
        {
            if (Input.touchCount == 1)
            {
                //Debug.Log("Screen Touched");
                touch = Input.GetTouch(0);

                if(touch.phase == TouchPhase.Moved)
                {
                    var mouseMovement = new Vector2(-touch.deltaPosition.y * 3f, touch.deltaPosition.x * 3f);
                    rotationValues += mouseMovement * RotationSensitivity * Time.unscaledDeltaTime;
                    rotationValues = new Vector2(Mathf.Clamp(rotationValues.x, /*-80f*/10f, 80f),rotationValues.y);
                }
            }
            else if (Input.touchCount == 2)
            {
                //R�cuperation des deux entr�s
                Touch firstTouch = Input.GetTouch(0);
                Touch secondTouch = Input.GetTouch(1);

                //Calcul des diff�rences(savoir si on zoom/dezoom)
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
            if (lineVisual)
            {
                var pos = transform.position;
                //lineVisual.SetPositionAndRotation(new Vector3(pos.x, pos.y - 0.0499f, pos.z), Quaternion.Euler(new Vector3(90, rotationValues.y, 0)));

            }
        }
    }
    private void FixedUpdate()
    {
        //Speed
        AbsMagn = Vector3.Magnitude(rb.velocity);
        moving = (AbsMagn > 0.05);
        if (magnHasChanged && !moving && !hasAddPenality  && pc.GetActualStrokes() == maxStrokes)
        {
            //Debug.Log("Player is over the limit of strokes");
            pc.RpcAddXStroke(penalityStrokes);
            pc.GetTimer().StopTimer();
            //StopCoroutine(timeLimitCoroutine);
            pc.OnOutofStrokes();
            hasAddPenality = true;
        }
        if(pc.isLocalPlayer)
        {
            if (!lineVisual.gameObject.activeSelf && !moving && !pc.hasFinishHole)
                lineVisual.gameObject.SetActive(true);
            else if (lineVisual.gameObject.activeSelf && moving && pc.hasFinishHole)
                lineVisual.gameObject.SetActive(false);
        }
        
        if (!magnHasChanged && AbsMagn > 0.1)
            magnHasChanged = true;
        if (magnHasChanged && AbsMagn == 0)
            magnHasChanged = false;

    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground") && !isOnGreen)
        {
            //Debug.Log("Hors limit... " + collision.gameObject.name + "(" + collision.contacts[0].point + ")");
            isOutOfLimit = true;
        }
        if (collision.transform.CompareTag("Green"))
        {
            isOnGreen = true;
        }
    }
    public void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("Green"))
        {
            isOnGreen = false;
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        HoleBehavior hole = other.transform.parent.GetComponent<HoleBehavior>();
        if (hole != null)
        {
            if(pc.isLocalPlayer)
                pc.GetTimer().StopTimer();
            //StopCoroutine(timeLimitCoroutine);
            pc.OnHoleEntered(hole.maxStrokes);
        }
    }

    #endregion

    #region COROUTINE

    #endregion
    #region PUBLIC_FUNCTION
    public void Push()
    {
        //When the ball is moving and under the limit,it dont pass to false so its reactiviting the stopped function
        if (hasFinishHole || isOutOfLimit || !isOnGreen || pc.GetActualStrokes()+1 > maxStrokes)
            return;
        magnHasChanged = false;
        if(lineVisual.gameObject.activeSelf)
            lineVisual.gameObject.SetActive(false);
        //Debug.Log("Push the ball: " + pc.GetName());
      
        DoSound();
        lastPosition = transform.position;
        var vec = cam.transform.forward;
        force = sliderForce.value*scaleForce;
        float sensY = rb.velocity.normalized.y;
        if(uphill)
            sensY = Mathf.Abs(sensY);
        if (flatSurface)
            sensY = 0f;
        vec = new Vector3(vec.x, sensY /*0*/, vec.z);
        //rb.AddForce(vec * force, ForceMode.Impulse);
        pc.PushBall(vec, force);
    }
    public void TpStart()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.rotation = sr;
        transform.position = sp;
    }

    public void SetRotationValueY(float y)
    {
        rotationValues.x = 15;
        rotationValues.y = y;
    }
    public void SetLastPosition(Vector3 position)
    {
        lastPosition = position;
    }
    public void IgnoreBalls()
    {
        GetComponent<SphereCollider>().excludeLayers = ballLayer;
    }

    public void DontIgnoreBalls()
    {
        GetComponent<SphereCollider>().excludeLayers = 0;
    }

    public void Spawn(bool b)
    {
        mr.enabled = b;
        //rb.useGravity = b;
    }

    public void FreezeBall()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void UnfreezeBall()
    {
        rb.constraints = RigidbodyConstraints.None;
    }

    #endregion

    #region PRIVATE_FUNCTION

    private void DetectSlope()
    {
        if (cam)
        {
            float camX = cam.transform.forward.x / 7f;
            float camZ = cam.transform.forward.z / 7f;

            frontRayPos.position = transform.position + new Vector3(camX, 0, camZ);
            rearRayPos.position = transform.position + new Vector3(-camX, 0, -camZ);
        }
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
        if (Mathf.Abs(rearHit.distance - frontHit.distance) < 0.02f)
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

    private void DoSound()
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.Play();
    }
   
    private void TpToLastLocation()
    {
        //Debug.Log("En dehors des limites... Tps vers la dernieres positions :" + lastPosition);
        pc.TpToLocation(lastPosition);
        isOutOfLimit = false;
        timeOutLimit = 0f;
    }
    #endregion


    #region GETTER_SETTER
    public int GetPenalityStrokes()
    {
        return penalityStrokes;
    }
    public int GetMaxStrokes()
    {
        return maxStrokes;
    }
    public PlayerController GetPlayer()
    {
        return pc;
    }

    public void SetAddPenality(bool add)
    {
        hasAddPenality = add;
    }
    public void SetIsVisualized(bool b)
    {
        isVisualized = b;
    }

    public void SetFirstEnable(bool b)
    {
        firstEnable = b;
    }

    public Vector3 GetDir()
    {
        var vec = cam.transform.forward;
        float sensY = rb.velocity.normalized.y;
        if (uphill)
            sensY = Mathf.Abs(sensY);
        if (flatSurface)
            sensY = 0f;
        return new Vector3(vec.x, sensY /*0*/, vec.z);
    }

    public float GetForce()
    {
        return sliderForce.value * scaleForce;
    }
    #endregion
}
