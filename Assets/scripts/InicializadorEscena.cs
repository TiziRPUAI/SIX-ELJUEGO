using UnityEngine;

public class InicializadorEscena : MonoBehaviour
{
    void Awake()
    {
        // Asegurarse de que el tiempo est茅 en escala normal
        Time.timeScale = 1f;

        Debug.Log("Escena inicializada - Time.timeScale restaurado a 1");
    }
}
