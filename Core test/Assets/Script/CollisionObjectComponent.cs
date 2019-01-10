using System;
using Unity.Entities;
using BulletSharp;
using Unity.Collections;
using BulletSharp.Math;
using UnityEngine;
using BulletUnity;
public class CollisionObjectComponent: MonoBehaviour
{
    // Use this for initialization
}
/*public class CollisionObjectSystem : ComponentSystem
{
    struct CollisionObjectGroup
    {
        public ComponentArray<CollisionObjectComponent> CollisionObjects;
        public ComponentArray<Transform> transform;
    }
    [Inject]
    CollisionObjectGroup m_Group;
    struct CollisionObjectGroup
    {
        public CollisionObjectComponent CollisionObjects;
        public Transform transform;
    }
    protected override void OnUpdate()
    {
        foreach (var e in GetEntities<CollisionObjectGroup>())
        {
            Matrix4x4 m = e.CollisionObjects.target.WorldTransform.ToUnity();
            e.transform.position = BSExtensionMethods2.ExtractTranslationFromMatrix(ref m);
            e.transform.rotation = BSExtensionMethods2.ExtractRotationFromMatrix(ref m);
        }
    }
}*/
/*[Serializable]
public struct CollisionObjects : ISharedComponentData
{
    public CollisionObject target;

}

public class CollisionObjectComponent : SharedComponentDataWrapper<CollisionObjects> { }*/