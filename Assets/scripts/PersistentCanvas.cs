using UnityEngine;

public class PersistentCanvas : MonoBehaviour
{
    private static PersistentCanvas instance;

    void Awake()
    {
        // Si ya existe una instancia, destruir este duplicado
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Marcar este objeto como persistente
        instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("Canvas marcado como persistente (no se destruirá entre escenas)");
    }
}