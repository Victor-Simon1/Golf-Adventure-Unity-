using Services;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projection : MonoRegistrable
{
    [SerializeField] private LineRenderer _line;
    [SerializeField] private int _maxPhysicsFrameIterations = 100;
    [SerializeField] private Transform _obstaclesParent;

    private Scene _simulationScene;
    private PhysicsScene _physicsScene;
    private readonly Dictionary<Transform, Transform> _spawnedObjects = new Dictionary<Transform, Transform>();

    [SerializeField] private GameObject ballPrefab;

    private void Awake()
    {
        ServiceLocator.Register<Projection>(this);
    }

    private void Start()
    {
        CreatePhysicsScene();
        _line = ServiceLocator.Get<GameManager>().GetLocalPlayer().GetLineRenderer();
        _line.alignment = LineAlignment.TransformZ;
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

    private void Update()
    {
        foreach (var item in _spawnedObjects)
        {
            item.Value.position = item.Key.position;
            item.Value.rotation = item.Key.rotation;
        }
    }

    public void SimulateTrajectory(Vector3 pos, Vector3 dir, float force)
    {
        var ghostObj = Instantiate(ballPrefab, pos, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(ghostObj.gameObject, _simulationScene);
        var ghostBall = ghostObj.GetComponent<SimpleBallController>();
        ghostBall.SimplePush(dir, force, true);

        _line.positionCount = _maxPhysicsFrameIterations;

        for (var i = 0; i < _maxPhysicsFrameIterations; i++)
        {
            _physicsScene.Simulate(Time.fixedDeltaTime);
            _line.SetPosition(i, ghostBall.GetLinePosition());
        }

        Destroy(ghostObj.gameObject);
    }
}
