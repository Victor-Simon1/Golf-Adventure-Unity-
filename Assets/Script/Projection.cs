using Services;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projection : MonoRegistrable
{
    [SerializeField] private LineRenderer _line;
    [SerializeField] private int _maxPhysicsFrameIterations = 10;
    [SerializeField] private Transform _obstaclesParent;
    [SerializeField] private float multiplier = 1;

    private Scene _simulationScene;
    private PhysicsScene _physicsScene;
    private readonly Dictionary<Transform, Transform> _spawnedObjects = new Dictionary<Transform, Transform>();

    [SerializeField] private GameObject ballPrefab;

    private BallControler ballControler;

    private void Awake()
    {
        ServiceLocator.Register<Projection>(this, false);
    }

    private void Start()
    {
        CreatePhysicsScene();
        var playerController = ServiceLocator.Get<GameManager>().GetLocalPlayer();
        _line = playerController.GetLineRenderer();
        _line.alignment = LineAlignment.TransformZ;
        ballControler = playerController.GetBall();
    }

    private void Update()
    {
        foreach (var item in _spawnedObjects)
        {
            item.Value.position = item.Key.position;
            item.Value.rotation = item.Key.rotation;
        }

    }

    private void CreatePhysicsScene()
    {
        _simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _physicsScene = _simulationScene.GetPhysicsScene();

        foreach (Transform obj in _obstaclesParent)
        {
            var ghostObj = Instantiate(obj.gameObject, obj.position, obj.rotation);
            Renderer[] ghostRenderers = ghostObj.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in ghostRenderers) { renderer.enabled = false; }
            SceneManager.MoveGameObjectToScene(ghostObj, _simulationScene);
            if (!ghostObj.isStatic) _spawnedObjects.Add(obj, ghostObj.transform);
        }
    }

    public void SimulateTrajectory(Vector3 pos, Vector3 dir, float force)
    {
        var ghostObj = Instantiate(ballPrefab, pos, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(ghostObj.gameObject, _simulationScene);
        var ghostBall = ghostObj.GetComponent<SimpleBallController>();
        ghostBall.SimplePush(dir, force*multiplier, true);

        _line.positionCount = _maxPhysicsFrameIterations;

        for (var i = 0; i < _maxPhysicsFrameIterations; i++)
        {
            _physicsScene.Simulate(Time.fixedDeltaTime);

            float x = ghostObj.transform.position.x;
            float y = ghostObj.transform.position.y;
            float z = ghostObj.transform.position.z;
            _line.SetPosition(i, new Vector3(x, y - 0.049f, z));
        }

        Destroy(ghostObj.gameObject);
    }
}
