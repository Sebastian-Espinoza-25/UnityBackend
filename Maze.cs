using UnityEngine;
using System.Collections.Generic;

public class Maze : MonoBehaviour
{
    // Listas de vectores para geometría y esquinas del camino
    List<Vector3> geometry;
    List<Vector3> cornersInPath;
    Matrix4x4 baseTransform;
    GameObject theCar;
    float hSide = 10.0f / 2.0F;
    Vector3 currentPosition;
    int currentCorner = 0;
    bool isRotating = false;
    float angleForRot = 0f;

    void Start()
    {
        InitializeTheCar();

        // Inicializar la lista de esquinas (puede estar vacía al principio)
        cornersInPath = new List<Vector3>();
        
    }

    void Update()
    {
        // Si ya estamos en el último waypoint, mantenemos el color verde y no hacemos nada más
        if (currentCorner >= cornersInPath.Count - 1)
        {
            theCar.GetComponent<MeshRenderer>().material.color = Color.green;
            return;
        }

        Vector3 nextCorner = cornersInPath[currentCorner + 1];
        Vector3 direction = VecOps.Normalize(nextCorner - currentPosition);

        // Depurar si la posición del cubo cambia
        Debug.Log("Posición actual: " + currentPosition);  // Agrega esto para verificar la posición del cubo

        if (isRotating)
        {
            RotationFun(direction, nextCorner);
        }
        else
        {
            MoveToFun(direction, nextCorner);
        }

        ApplyTransformations();
    }

    private void InitializeTheCar()
    {
        // Inicializar el cubo y agregarle el MeshFilter
        theCar = new GameObject("TheCar");
        MeshFilter mf = theCar.AddComponent<MeshFilter>();

        // Definición de los vértices 
        geometry = new List<Vector3> {
            new Vector3(-1, -1, 1), 
            new Vector3(1, -1, 1), 
            new Vector3(1, 1, 1),
            new Vector3(-1, 1, 1), 
            new Vector3(1, -1, -1), 
            new Vector3(1, 1, -1),
            new Vector3(-1, -1, -1), 
            new Vector3(-1, 1, -1)
        };

        // Definición de la topología 
        List<int> topology = new List<int> {
            1, 2, 3, 
            1, 3, 4, 
            2, 5, 3, 
            5, 6, 3, 
            5, 7, 6, 
            7, 8, 6,
            7, 1, 8, 
            1, 4, 8, 
            4, 3, 8, 
            3, 6, 8, 
            2, 1, 7, 
            2, 7, 5
        };

        for (int i = 0; i < topology.Count; i++) topology[i]--;

        //------------------
        Matrix4x4 scaleMatrix = VecOps.ScaleM(new Vector4(hSide, hSide, hSide, 1));
        Matrix4x4 translateMatrix = VecOps.TranslateM(new Vector3(hSide * 1, hSide * 1, hSide * 1));
        baseTransform = translateMatrix * scaleMatrix;
        List<Vector3> BeginningMatrix = VecOps.ApplyTransform(geometry, baseTransform);

        mf.mesh = new Mesh();
        mf.mesh.vertices = BeginningMatrix.ToArray();
        mf.mesh.triangles = topology.ToArray();
        mf.mesh.RecalculateNormals();

        MeshRenderer mr = theCar.AddComponent<MeshRenderer>();
        mr.material.color = Color.blue;
    }

    private void RotationFun(Vector3 direction, Vector3 nextCorner)
    {
        // Solo rotar si hay al menos dos esquinas más adelante en el camino
        if (currentCorner < cornersInPath.Count - 2)
        {
            // Obtener el próximo punto y calcular la dirección
            Vector3 nextPosition = cornersInPath[currentCorner + 2];
            Vector3 nextDirection = VecOps.Normalize(nextPosition - nextCorner);

            // Calcular el producto cruzado para ver hacia donde va
            Vector3 cross = VecOps.CrossProduct(direction, nextDirection);

            // Determinar si gira a la izquierda o a la derecha
            if (cross.y < 0)
            {
                angleForRot -= 1f; // izquierda
            }
            else
            {
                angleForRot += 1f; // derecha
            }

            // Detener la rotación después de un giro de 90 grados
            if (Mathf.Abs(angleForRot) >= 90f)
            {
                angleForRot = 0f;
                isRotating = false;
                currentCorner++;
            }
        }

        // Cambiar el color a magenta mientras el cubo está girando
        theCar.GetComponent<MeshRenderer>().material.color = Color.magenta;
    }

    private void MoveToFun(Vector3 direction, Vector3 nextCorner)
    {
        currentPosition += direction * 0.1f;

        if (Vector3.Distance(currentPosition, nextCorner) < 0.1f)
        {
            if (currentCorner < cornersInPath.Count - 2)
            {
                isRotating = true;
            }
            else
            {
                currentCorner++;
            }
        }
        else
        {
            theCar.GetComponent<MeshRenderer>().material.color = currentCorner > cornersInPath.Count - 3 ? Color.green : Color.blue;
        }
    }

    private void ApplyTransformations()
    {
        Matrix4x4 translationMatrix = VecOps.TranslateM(currentPosition);
        Matrix4x4 rotationMatrix = VecOps.RotateYM(angleForRot);
        Matrix4x4 scalingMatrix = VecOps.ScaleM(new Vector4(hSide, hSide, hSide, 1));
        Matrix4x4 MultiTransform = translationMatrix * rotationMatrix * scalingMatrix;

        List<Vector3> CarVertices = VecOps.ApplyTransform(geometry, MultiTransform);
        theCar.GetComponent<MeshFilter>().mesh.vertices = CarVertices.ToArray();

        // Verificar que la posición se aplica correctamente
        Debug.Log("Aplicando transformación. Nueva posición: " + currentPosition);  // Verifica la nueva posición
    }

    // Método para actualizar la ruta (cornersInPath) con los puntos recibidos
    public void UpdateCornersInPath(List<Vector3> newCorners)
    {
        Debug.Log("Actualizando cornersInPath con " + newCorners.Count + " puntos.");
        cornersInPath = newCorners;
        currentCorner = 0;  // Reiniciar el índice del waypoint para comenzar desde el inicio
        if (cornersInPath.Count > 0)
        {
            currentPosition = cornersInPath[0];  // Asegurarse de que la posición inicial esté actualizada
            theCar.transform.position = currentPosition;  // Mover el cubo a la posición inicial (esto evita que se teletransporte)
        }
    }
}
