using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletSharp;
using BulletSharp.Math;
using DemoFramework;
using BulletUnity;
public class PhysicsDemo : MonoBehaviour
{

    // create 125 (5x5x5) dynamic objects
    const int ArraySizeX = 5, ArraySizeY = 5, ArraySizeZ = 5;

    // scaling of the objects (0.1 = 20 centimeter boxes )
    const float StartPosX = -5;
    const float StartPosY = -5;
    const float StartPosZ = -3;

    protected CollisionConfiguration CollisionConf;
    protected CollisionDispatcher Dispatcher;
    protected BroadphaseInterface Broadphase;
    protected ConstraintSolver Solver;
    protected DynamicsWorld World;

    public int MaxHeight = 40;
    public Material mat;
    public Material groundMat;
    public List<GameObject> createdObjs = new List<GameObject>();
    public List<CollisionShape> CollisionShapes { get; private set; }

    // Use this for initialization
    void Start()
    {
        CollisionShapes = new List<CollisionShape>();
        if (World == null)
        {
            OnInitializePhysics();
            PostOnInitializePhysics();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            CollisionObject obj = World.CollisionObjectArray[0];
            Matrix startTransform = Matrix.Translation(100, 100, 100);
            obj.WorldTransform = startTransform;
        }
        if (World != null)
        {
            World.StepSimulation(Time.fixedDeltaTime);
        }
    }
    protected void OnInitializePhysics()
    {
        // collision configuration contains default setup for memory, collision setup
        CollisionConf = new DefaultCollisionConfiguration();
        Dispatcher = new CollisionDispatcher(CollisionConf);

        Broadphase = new DbvtBroadphase();

        World = new DiscreteDynamicsWorld(Dispatcher, Broadphase, null, CollisionConf);
        World.Gravity = new BulletSharp.Math.Vector3(0, -10, 0);

        BoxShape groundShape = new BoxShape(10, 0.5f, 10);

        CollisionShapes.Add(groundShape);
        CollisionObject ground = LocalCreateRigidBody(0.0f, Matrix.Identity, groundShape, true);
        ground.UserObject = "Ground";

        BoxShape WallShape1 = new BoxShape(0.5f, 25, 10);

        CollisionShapes.Add(WallShape1);
        CollisionObject wall1 = LocalCreateRigidBody(0.0f, Matrix.Translation(new BulletSharp.Math.Vector3(-10, 25,0)), WallShape1, true);
        wall1.UserObject = "Ground";

        CollisionShapes.Add(WallShape1);
        CollisionObject wall2 = LocalCreateRigidBody(0.0f, Matrix.Translation(new BulletSharp.Math.Vector3(10, 25, 0)), WallShape1, true);
        wall2.UserObject = "Ground";

        BoxShape WallShape2 = new BoxShape(10, 25, 0.5f);
        
        CollisionShapes.Add(WallShape2);
        CollisionObject wall3 = LocalCreateRigidBody(0.0f, Matrix.Translation(new BulletSharp.Math.Vector3(0,25, -10)), WallShape2, true);
        wall3.UserObject = "Ground";

        CollisionShapes.Add(WallShape2);
        CollisionObject wall4 = LocalCreateRigidBody(0.0f, Matrix.Translation(new BulletSharp.Math.Vector3(0, 25, 10)), WallShape2, true);
        wall4.UserObject = "Ground";


        float cubeSize = 0.5f;
        float spacing = cubeSize;
        float mass = 1.0f;
        int size = 8;
        BulletSharp.Math.Vector3 pos = new BulletSharp.Math.Vector3(0.0f, 20, 0.0f);
        float offset = -size / 2 + 0.5f;

        // 3000

        BoxShape blockShape = new BoxShape(cubeSize);
        mass = 2.0f;

        for (int k = 0; k < MaxHeight; k++)
        {
            for (int j = 0; j < size; j++)
            {
                pos[2] = offset + j * (cubeSize * 2.0f);
                for (int i = 0; i < size; i++)
                {
                    pos[0] = offset + i * (cubeSize * 2.0f);
                    /*RigidBody cmbody =*/
                    LocalCreateRigidBody(mass, Matrix.Translation(pos), blockShape);
                }
            }
            pos[1] += (cubeSize + spacing) * 2.0f;
        }

    }
    private float DegreeToRadian(float angle)
    {
        return (float)System.Math.PI * angle / 180.0f;
    }
    public void PostOnInitializePhysics()
    {
        for (int i = 0; i < World.CollisionObjectArray.Count; i++)
        {
            CollisionObject co = World.CollisionObjectArray[i];
            CollisionShape cs = co.CollisionShape;
            GameObject go;
            go = CreateUnityCollisionObjectProxy(co as CollisionObject);
            createdObjs.Add(go);
        }
    }
    public GameObject CreateUnityCollisionObjectProxy(CollisionObject body)
    {
        if (body is GhostObject)
        {
            Debug.Log("ghost obj");
        }
        GameObject go = new GameObject(body.ToString());
        MeshFilter mf = go.AddComponent<MeshFilter>();
        Mesh m = mf.mesh;
        MeshFactory2.CreateShape(body.CollisionShape, m);
        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.sharedMaterial = mat;
        if (body.UserObject != null && body.UserObject.Equals("Ground"))
        {
            mr.sharedMaterial = groundMat;
        }
        BulletRigidBodyProxy rbp = go.AddComponent<BulletRigidBodyProxy>();
        rbp.target = body;
        return go;
    }
    public RigidBody LocalCreateRigidBody(float mass, Matrix startTransform, CollisionShape shape, bool isKinematic = false)
    {
        //rigidbody is dynamic if and only if mass is non zero, otherwise static
        bool isDynamic = (mass != 0.0f);

        BulletSharp.Math.Vector3 localInertia = BulletSharp.Math.Vector3.Zero;
        if (isDynamic)
            shape.CalculateLocalInertia(mass, out localInertia);

        //using motionstate is recommended, it provides interpolation capabilities, and only synchronizes 'active' objects
        DefaultMotionState myMotionState = new DefaultMotionState(startTransform);

        RigidBodyConstructionInfo rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, shape, localInertia);
        RigidBody body = new RigidBody(rbInfo);
        if (isKinematic)
        {
            body.CollisionFlags = body.CollisionFlags | BulletSharp.CollisionFlags.KinematicObject;
            body.ActivationState = ActivationState.ActiveTag;
        }
        rbInfo.Dispose();

        World.AddRigidBody(body);

        return body;
    }
    void OnDestroy()
    {
        for (int i = 0; i < createdObjs.Count; i++)
        {
            Destroy(createdObjs[i]);
        }
        createdObjs.Clear();
        UnityEngine.Debug.Log("ExitPhysics");
        if (World != null)
        {
            //remove/dispose constraints
            int i;
            for (i = World.NumConstraints - 1; i >= 0; i--)
            {
                TypedConstraint constraint = World.GetConstraint(i);
                World.RemoveConstraint(constraint);
                constraint.Dispose();
            }

            //remove the rigidbodies from the dynamics world and delete them
            for (i = World.NumCollisionObjects - 1; i >= 0; i--)
            {
                CollisionObject obj = World.CollisionObjectArray[i];
                RigidBody body = obj as RigidBody;
                if (body != null && body.MotionState != null)
                {
                    body.MotionState.Dispose();
                }
                World.RemoveCollisionObject(obj);
                obj.Dispose();
            }

            //delete collision shapes
            foreach (CollisionShape shape in CollisionShapes)
                shape.Dispose();
            CollisionShapes.Clear();

            World.Dispose();
            Broadphase.Dispose();
            Dispatcher.Dispose();
            CollisionConf.Dispose();
        }

        if (Broadphase != null)
        {
            Broadphase.Dispose();
        }
        if (Dispatcher != null)
        {
            Dispatcher.Dispose();
        }
        if (CollisionConf != null)
        {
            CollisionConf.Dispose();
        }
    }
}
