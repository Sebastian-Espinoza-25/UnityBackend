using UnityEngine;
using System.Collections.Generic;

public class Maze : MonoBehaviour
{
    public GameObject carPrefab;
    private GameObject theCar;
    
    List<Vector3> cornersInPath;
    Vector3 currentPosition;
    int currentCorner = 0;
    bool isRotating = false;
    float targetAngle = 0f;
    float currentAngle = 0f;
    Vector3 currentDirection;
    Matrix4x4 accumulatedRotation = Matrix4x4.identity;

    void Start()
    {
        InitializeTheCar();
        cornersInPath = new List<Vector3>();
    }

    void Update()
    {
        if (cornersInPath.Count < 2 || currentCorner >= cornersInPath.Count - 1) return;

        Vector3 nextCorner = cornersInPath[currentCorner + 1];
        Vector3 directionToNext = VecOps.Normalize(nextCorner - currentPosition);

        if (isRotating)
        {
            PerformRotation(directionToNext);
        }
        else
        {
            MoveToNextCorner(directionToNext, nextCorner);
        }
    }

    private void PerformRotation(Vector3 targetDirection)
    {
        // Rotar 1 grado por frame
        float rotationStep = 1f;
        
        if (Mathf.Abs(currentAngle) < Mathf.Abs(targetAngle))
        {
            // Actualizar el ángulo actual
            if (targetAngle > 0)
                currentAngle += rotationStep;
            else
                currentAngle -= rotationStep;

            // Crear y aplicar la matriz de rotación
            Matrix4x4 rotationMatrix = VecOps.RotateYM(rotationStep * Mathf.Sign(targetAngle));
            accumulatedRotation = rotationMatrix * accumulatedRotation;

            // Actualizar la dirección actual
            Vector4 newDir4 = rotationMatrix * new Vector4(currentDirection.x, currentDirection.y, currentDirection.z, 0);
            currentDirection = VecOps.Normalize(new Vector3(newDir4.x, newDir4.y, newDir4.z));

            // Aplicar la rotación al objeto
            Vector4 forward4 = accumulatedRotation * new Vector4(0, 0, 1, 0);
            Vector3 forward = VecOps.Normalize(new Vector3(forward4.x, forward4.y, forward4.z));
            theCar.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }
        else
        {
            // Completar la rotación
            isRotating = false;
            currentAngle = 0f;
            targetAngle = 0f;
            currentCorner++;
        }
    }

    private void MoveToNextCorner(Vector3 direction, Vector3 nextCorner)
    {
        // Avanzar 0.1 unidades por frame
        float step = 0.1f;
        Matrix4x4 translation = VecOps.TranslateM(direction * step);
        Vector4 newPos4 = translation * new Vector4(currentPosition.x, currentPosition.y, currentPosition.z, 1);
        Vector3 newPosition = new Vector3(newPos4.x, newPos4.y, newPos4.z);

        // Verificar si llegamos a la esquina
        if (Vector3.Distance(newPosition, nextCorner) < 0.1f)
        {
            currentPosition = nextCorner;
            
            // Si hay una siguiente esquina, preparar la rotación
            if (currentCorner < cornersInPath.Count - 2)
            {
                Vector3 nextDirection = VecOps.Normalize(cornersInPath[currentCorner + 2] - nextCorner);
                Vector3 cross = VecOps.CrossProduct(direction, nextDirection);
                
                // Determinar dirección de rotación basada en el producto cruz
                targetAngle = (cross.y < 0) ? -90f : 90f;
                isRotating = true;
            }
        }
        else
        {
            currentPosition = newPosition;
        }

        // Actualizar la posición del objeto
        theCar.transform.position = currentPosition;
    }

    private void InitializeTheCar()
    {
        theCar = Instantiate(carPrefab, Vector3.zero, Quaternion.identity);
        accumulatedRotation = Matrix4x4.identity;
        currentDirection = Vector3.forward;
    }

    public void UpdateCornersInPath(List<Vector3> newCorners)
    {
        cornersInPath = newCorners;
        currentCorner = 0;
        isRotating = false;
        currentAngle = 0f;
        targetAngle = 0f;
        accumulatedRotation = Matrix4x4.identity;

        if (cornersInPath.Count > 0)
        {
            currentPosition = cornersInPath[0];
            theCar.transform.position = currentPosition;
            theCar.transform.rotation = Quaternion.identity;
        }
    }
}