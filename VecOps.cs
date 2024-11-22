using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VecOps : MonoBehaviour {
    public static float DotProduct(Vector3 a, Vector3 b) {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }

    
    public static Vector3 CrossProduct(Vector3 a, Vector3 b) {
       
        return new Vector3(
            a.y * b.z - a.z * b.y,  // x
            a.z * b.x - a.x * b.z,  // y
            a.x * b.y - a.y * b.x   // z
        );
    }

    public static Vector3 Normalize (Vector3 v) {
        float mag = Mathf.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
        return new Vector3(v.x / mag, v.y / mag, v.z / mag);
    }

    public static float Angle(Vector3 a, Vector3 b) {
        
        float angle =  Mathf.Acos(DotProduct(Normalize(a), Normalize(b)));
        return angle * Mathf.Rad2Deg; 
    }

    public static Matrix4x4 TranslateM(Vector3 dt) {
        Matrix4x4 m = Matrix4x4.identity;
        m[0, 3] = dt.x;
        m[1, 3] = dt.y;
        m[2, 3] = dt.z;
        return m;
    }

    public static Matrix4x4 ScaleM(Vector4 s) {
        Matrix4x4 m = Matrix4x4.identity;
        m[0, 0] = s.x;
        m[1, 1] = s.y;
        m[2, 2] = s.z;
        return m;
    }

    public static Matrix4x4 RotateXM(float degrees) {
        float radians = Mathf.Deg2Rad * degrees;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        Matrix4x4 m = Matrix4x4.identity;
        m[1, 1] = cos;
        m[1, 2] = -sin;
        m[2, 1] = sin;
        m[2, 2] = cos;
        return m;
    }

    public static Matrix4x4 RotateYM(float degrees) {
        float radians = Mathf.Deg2Rad * degrees;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        Matrix4x4 m = Matrix4x4.identity;
        m[0, 0] = cos;
        m[0, 2] = sin;
        m[2, 0] = -sin;
        m[2, 2] = cos;
        return m;
    }

    public static Matrix4x4 RotateZM(float degrees) {
        float radians = Mathf.Deg2Rad * degrees;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        Matrix4x4 m = Matrix4x4.identity;
        m[0, 0] = cos;
        m[0, 1] = -sin;
        m[1, 0] = sin;
        m[1, 1] = cos;
        return m;
    }

    public static List<Vector3> ApplyTransform(List<Vector3> originals, Matrix4x4 m) {
        List<Vector3> result = new List<Vector3>();
        foreach (Vector3 v in originals) {
            Vector4 v4 = new Vector4(v.x, v.y, v.z, 1);
            Vector4 v4t = m * v4;
            result.Add(new Vector3(v4t.x, v4t.y, v4t.z));
        }
        return result;
    }
}