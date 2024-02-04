using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathUtils : MonoBehaviour
{
    // you can't directly serialize a Vector3, so make a struct to store the relevant info
    [System.Serializable]
    public struct SerializedVector3
    {
        public float x;
        public float y;
        public float z;
    }
// this method converts a Vector3 into a SerializedVector3
    public static SerializedVector3 SerializeVector3(Vector3 v3)
    {
        SerializedVector3 sv3;
        sv3.x = v3.x;
        sv3.y = v3.y;
        sv3.z = v3.z;
        return sv3;
    }
// this method converts a SerializedVector3 into a Vector3
    public static Vector3 DeserializeVector3(SerializedVector3 sv3)
    {
        Vector3 v3;
        v3.x = sv3.x;
        v3.y = sv3.y;
        v3.z = sv3.z;
        return v3;
    }
}
