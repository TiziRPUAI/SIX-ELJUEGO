using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Referencias")]
    public GameObject jugador;
    public Transform puntoSpawnInicial;

    [Header("Configuración")]
    public float tiempoReinicioTrasLMuerte = 2f;

    private Vector3 posicionInicialJugador;

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantener entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
        Time.timeScale = 1f;

        Debug.Log("GameManager inicializado - Time.timeScale = 1");
    }

    void Start()
    {
        if (jugador != null)
        {
            if (puntoSpawnInicial != null)
                posicionInicialJugador = puntoSpawnInicial.position;
            else
                posicionInicialJugador = jugador.transform.position;
        }
    }

    public void ReiniciarNivel()
    {
        // Recargar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CargarNivel(string nombreNivel)
    {
        SceneManager.LoadScene(nombreNivel);
    }

    public void CargarNivelSiguiente()
    {
        int indiceSiguiente = SceneManager.GetActiveScene().buildIndex + 1;

        if (indiceSiguiente < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(indiceSiguiente);
        }
        else
        {
            Debug.Log("¡No hay más niveles!");
        }
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void PausarJuego()
    {
        Time.timeScale = 0f;
    }

    public void ReanudarJuego()
    {
        Time.timeScale = 1f;
    }
}