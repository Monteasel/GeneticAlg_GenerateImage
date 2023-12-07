using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class test : MonoBehaviour
{
    private void Start()
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Plane);
        obj.transform.localPosition = new Vector3(50,1,10);
        obj.GetComponent<MeshRenderer>().material.mainTexture = Texture2D.redTexture;
    }
}
