using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletSharp;
using BulletSharp.Math;
using DemoFramework;
using BulletUnity;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Rendering;

public class PhysicsDemo_BulletECS : MonoBehaviour
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
    protected DynamicsWorld world;
    public Mesh mesh;
    public int MaxHeight = 40;
    public Material mat;
    public Material groundMat;
    public List<GameObject> createdObjs = new List<GameObject>();
    public List<CollisionShape> CollisionShapes { get; private set; }
    EntityManager entityManager;
    public static EntityArchetype CollisionEntity;
    public static List<CollisionObject> CollisionObjects;
    // Use this for initialization
    void Start()
    {
        entityManager = World.Active.GetOrCreateManager<EntityManager>();
        //World.Active.GetOrCreateManager<MovementSystem>();
        CollisionShapes = new List<CollisionShape>();
        CollisionObjects = new List<CollisionObject>();
        CollisionEntity = entityManager.CreateArchetype(
            typeof(Unity.Transforms.Position),
            typeof(Unity.Transforms.Rotation),
            typeof(MeshInstanceRenderer)
            );
        if (world == null)
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
            CollisionObject obj = world.CollisionObjectArray[0];
            Matrix startTransform = Matrix.Translation(100, 100, 100);
            obj.WorldTransform = startTransform;
        }
        if (world != null)
        {
            world.StepSimulation(Time.fixedDeltaTime);
        }
    }
    /*public void SpawnCollisionObject()
    {
        Entity collisionObject = entityManager.CreateEntity(CollisionEntity);
        entityManager.SetSharedComponentData(collisionObject, SphereSmallRendererComponent.Value);
        entityManager.SetComponentData(collisionObject, new Unity.Transforms.Position { Value = startPos });
        entityManager.SetSharedComponentData(sphereRender, new FollowRigidBody { RigidBodyEntity = rigidBody });
    }*/
    protected void OnInitializePhysics()
    {
        // collision configuration contains default setup for memory, collision setup
        CollisionConf = new DefaultCollisionConfiguration();
        Dispatcher = new CollisionDispatcher(CollisionConf);

        Broadphase = new DbvtBroadphase();

        world = new DiscreteDynamicsWorld(Dispatcher, Broadphase, null, CollisionConf);
        world.Gravity = new BulletSharp.Math.Vector3(0, -10, 0);

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
        NativeArray<Entity> entities = new NativeArray<Entity>(world.CollisionObjectArray.Count, Allocator.Temp);
        Entity collisionObject = entityManager.CreateEntity(CollisionEntity);
        entityManager.Instantiate(collisionObject, entities);
        entityManager.DestroyEntity(collisionObject);
        //print(entities.Length);
        for (int i = 0; i < world.CollisionObjectArray.Count; i++)
        {
            CollisionObject co = world.CollisionObjectArray[i];
            CollisionShape cs = co.CollisionShape;
            CollisionObjects.Add(co);
            //GameObject go;
            //go = CreateUnityCollisionObjectProxy(co as CollisionObject);
            //createdObjs.Add(go);
            entityManager.SetComponentData(entities[i], new Unity.Transforms.Position { Value = new float3(0, 0, 0) });
            entityManager.SetComponentData(entities[i], new Unity.Transforms.Rotation { Value = new quaternion(0, 1, 0, 0) });
            //entityManager.AddComponent(entities[i], new CollisionObjectComponent {target=co });
            if (co.UserObject != null && co.UserObject.Equals("Ground"))
            {
                entityManager.SetSharedComponentData(entities[i], new MeshInstanceRenderer { mesh = mesh, material = groundMat });
            }
            else
            {
                entityManager.SetSharedComponentData(entities[i], new MeshInstanceRenderer { mesh = mesh, material = mat });

            }
        }
        //print(CollisionObjects.Count);
        entities.Dispose();
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

        world.AddRigidBody(body);

        return body;
    }
    void OnDestroy()
    {
        UnityEngine.Debug.Log("ExitPhysics");
        if (world != null)
        {
            //remove/dispose constraints
            int i;
            for (i = world.NumConstraints - 1; i >= 0; i--)
            {
                TypedConstraint constraint = world.GetConstraint(i);
                world.RemoveConstraint(constraint);
                constraint.Dispose();
            }

            //remove the rigidbodies from the dynamics world and delete them
            for (i = world.NumCollisionObjects - 1; i >= 0; i--)
            {
                CollisionObject obj = world.CollisionObjectArray[i];
                RigidBody body = obj as RigidBody;
                if (body != null && body.MotionState != null)
                {
                    body.MotionState.Dispose();
                }
                world.RemoveCollisionObject(obj);
                obj.Dispose();
            }

            //delete collision shapes
            foreach (CollisionShape shape in CollisionShapes)
                shape.Dispose();
            CollisionShapes.Clear();

            world.Dispose();
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
