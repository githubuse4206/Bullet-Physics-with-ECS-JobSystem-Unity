using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicDemo_PhysX : MonoBehaviour {
    public Material mat;
    public Material groundMat;
    public int MaxHeight=40;
    void Awake()
    {


    }
    // Use this for initialization
    void Start () {
        OnInitializePhyX();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    protected void OnInitializePhyX()
    {
        CreateObject(new Vector3(0, 0, 0), new Vector3(20, 1, 20), true, groundMat);


        CreateObject(new Vector3(0, 25, -10), new Vector3(20, 50, 1), true, groundMat);
        CreateObject(new Vector3(0, 25, 10), new Vector3(20, 50, 1), true, groundMat);
        CreateObject(new Vector3(10, 25, 0), new Vector3(1, 50, 20), true, groundMat);
        CreateObject(new Vector3(-10, 25, 0), new Vector3(1, 50, 20), true, groundMat);

        


        float cubeSize = 1.0f;
        float spacing = cubeSize;
        int size = 8;
        UnityEngine.Vector3 pos = new UnityEngine.Vector3(0.0f, 20, 0.0f);
        float offset = -size/2+0.5f;
        for (int k = 0; k < MaxHeight; k++)
        {
            for (int j = 0; j < size; j++)
            {
                pos[2] = offset+ j * (cubeSize);
                for (int i = 0; i < size; i++)
                {
                    pos[0] = offset + i * (cubeSize);
                    CreateObject(pos, new Vector3(cubeSize, cubeSize, cubeSize), false, mat);
                }
            }
            pos[1] += (cubeSize+ spacing);
        }

    }
    void CreateObject(Vector3 pos,Vector3 scale,bool isKinematic,Material material)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = pos;
        cube.transform.localScale = scale;
        cube.AddComponent<Rigidbody>();
        cube.GetComponent<Rigidbody>().isKinematic = isKinematic;
        cube.GetComponent<MeshRenderer>().sharedMaterial = material;

    }
}
