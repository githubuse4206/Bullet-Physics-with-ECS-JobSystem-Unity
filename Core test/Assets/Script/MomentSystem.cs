using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Jobs;
using BulletSharp;
using BulletSharp.Math;
using BulletUnity;



//public class MovementSystem : ComponentSystem
public class MovementSystem : JobComponentSystem
{
    struct CollisionObjectGroup
    {
        public ComponentDataArray<Position> positions;
        public ComponentDataArray<Rotation> rotations;
        public readonly int Length;
    }
    [Inject]
    CollisionObjectGroup m_Group;
    /*struct CollisionObjectGroup
    {
        public Transform transform;
    }*/
    /*protected override void OnUpdate()
    {
        for (int i=0;i<GetEntities<CollisionObjectGroup>().Length;i++)
        {

                Matrix4x4 m = PhysicsDemo_BulletECS.CollisionObjects[i].WorldTransform.ToUnity();
                GetEntities<CollisionObjectGroup>()[i].transform.position = BSExtensionMethods2.ExtractTranslationFromMatrix(ref m);
                GetEntities<CollisionObjectGroup>()[i].transform.rotation = BSExtensionMethods2.ExtractRotationFromMatrix(ref m);
        }
    }*/
    [BurstCompile]
    struct MovementJob : IJobParallelFor
    {
        public ComponentDataArray<Position> positions;
        public ComponentDataArray<Rotation> rotations;
        public void Execute(int i)
        {
            Matrix4x4 m = PhysicsDemo_BulletECS.CollisionObjects[i].WorldTransform.ToUnity();
            positions[i] = new Position { Value = BSExtensionMethods2.ExtractTranslationFromMatrix(ref m) };
            rotations[i] = new Rotation {Value= BSExtensionMethods2.ExtractRotationFromMatrix(ref m)};
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        MovementJob moveJob = new MovementJob();
        moveJob.positions = m_Group.positions;
        moveJob.rotations = m_Group.rotations;
        JobHandle moveHandle = moveJob.Schedule(m_Group.Length, 64, inputDeps);
        return moveHandle;
    }
}

