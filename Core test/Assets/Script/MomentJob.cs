using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using UnityEngine.Jobs;
using System;
using BulletSharp;
using BulletSharp.Math;
using BulletUnity;
[BurstCompile]
public struct MomentJob : IJobParallelForTransform {
    //public NativeArray<Matrix4x4> m;

    public void Execute(int i, TransformAccess transform)
    {
        Matrix4x4 m = PhysicsDemo_BulletJobs.CollisionObjects[i].WorldTransform.ToUnity();
        transform.position = BSExtensionMethods2.ExtractTranslationFromMatrix(ref m);
        transform.rotation= BSExtensionMethods2.ExtractRotationFromMatrix(ref m);

    }
}
