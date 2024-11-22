using UnityEngine;
using System.Collections.Generic;

public class Maze : MonoBehaviour
{
    // Prefab del objeto que se va a instanciar (asigna desde el Inspector)
    public GameObject carPrefab;  // Variable pública para el prefab
    private GameObject theCar;  // Referencia al objeto instanciado (el prefab)
    
    // Lista de esquinas para el camino
    List<Vector3> cornersInPath;
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
        // Si ya estamos en el último waypoint, no hacemos nada más
        if (currentCorner >= cornersInPath.Count - 1)
        {
            return;
        }

        Vector3 nextCorner = cornersInPath[currentCorner + 1];
        Vector3 direction = VecOps.Normalize(nextCorner - currentPosition);

        // Si estamos girando, rotamos, de lo contrario, movemos
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
        // Instanciar el prefab en lugar de crear un cubo
        theCar = Instantiate(carPrefab, Vector3.zero, Quaternion.identity); // Usar la posición inicial adecuada
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
    }

    private void MoveToFun(Vector3 direction, Vector3 nextCorner)
    {
        // Mover el objeto utilizando el transform y las funciones de VecOps
        theCar.transform.position += direction * 0.1f;  // Movimiento gradual

        // Verificar si hemos llegado al siguiente punto
        if (Vector3.Distance(theCar.transform.position, nextCorner) < 0.1f)
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

        currentPosition = theCar.transform.position;  // Actualiza la posición actual
    }

    private void ApplyTransformations()
    {
        // Sincronizar la rotación del coche con la dirección de movimiento
        Vector3 targetDirection = VecOps.Normalize(cornersInPath[currentCorner + 1] - currentPosition);
        
        // Usamos VecOps para calcular la rotación
        Matrix4x4 rotationMatrix = VecOps.RotateYM(angleForRot);
        
        // Aplicar la rotación a la dirección de movimiento
        theCar.transform.rotation = Quaternion.LookRotation(rotationMatrix * Vector3.forward);  // Rota el objeto hacia la dirección

        // Si tienes algún tipo de escala adicional, puedes aplicarla aquí, por ejemplo:
        theCar.transform.localScale = new Vector3(1, 1, 1);  // Escala predeterminada (puedes ajustarla)
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
