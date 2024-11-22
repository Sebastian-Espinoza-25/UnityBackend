using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class RouteRequester : MonoBehaviour
{
    string url = "http://127.0.0.1:5000/get_route";  // URL del servidor Flask
    Maze maze;  // Referencia al script Maze

    void Start()
    {
        maze = GetComponent<Maze>();  // Obtener la referencia al script Maze
        StartCoroutine(RequestRoute());
    }

    IEnumerator RequestRoute()
    {
        // Crear el JSON que se enviará
        var jsonData = "{\"start\": {\"latitude\": 19.4326, \"longitude\": -99.1332}, \"destination\": {\"latitude\": 19.4184, \"longitude\": -99.1625}}";

        // Crear la solicitud POST
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            // Esperar la respuesta
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // Obtener la respuesta (lista de coordenadas de la ruta)
                string response = www.downloadHandler.text;
                Debug.Log("Respuesta recibida desde Flask: " + response);  // Verificar la respuesta

                // Deserializar la respuesta JSON en una lista de puntos
                List<Vector3> route = ParseRoute(response);

                // Actualizar la lista de esquinas del laberinto (cornersInPath) con las nuevas coordenadas
                maze.UpdateCornersInPath(route);
                Debug.Log("Ruta actualizada con " + route.Count + " puntos.");
            }
            else
            {
                Debug.LogError("Error en la solicitud: " + www.error);
            }
        }
    }

    // Método para convertir la respuesta JSON en una lista de coordenadas Vector3
    List<Vector3> ParseRoute(string response)
    {
        List<Vector3> routePoints = new List<Vector3>();

        // Usamos JsonUtility para deserializar la respuesta
        RouteData routeData = JsonUtility.FromJson<RouteData>("{\"points\":" + response + "}");

        // Convertir cada punto a Vector3
        foreach (RoutePoint point in routeData.points)
        {
            routePoints.Add(new Vector3(point.latitude, 5, point.longitude));  // Suponiendo que la latitud es X y la longitud es Z
        }

        return routePoints;
    }
}

// Estructura que mapea la respuesta JSON
[System.Serializable]
public class RouteData
{
    public RoutePoint[] points;
}

// Clase para representar un punto de la ruta
[System.Serializable]
public class RoutePoint
{
    public float latitude;
    public float longitude;
}
