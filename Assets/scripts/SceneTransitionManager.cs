using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("Configuración")]
    public string puntoSpawnDestino = "SpawnPoint"; // Tag del punto de spawn

    [Header("Transición Visual")]
    public GameObject panelFade;
    public float tiempoFade = 1f;

    private CanvasGroup canvasGroupFade;
    private string escenaAnterior;
    private string puntoRetorno;

    void Awake()
    {
        Debug.Log("SceneTransitionManager Awake llamado"); // AGREGAR

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("SceneTransitionManager creado y marcado como DontDestroyOnLoad"); // AGREGAR
        }
        else
        {
            Debug.Log("Ya existe un SceneTransitionManager, destruyendo duplicado"); // AGREGAR
            Destroy(gameObject);
            return;
        }

        if (panelFade != null)
        {
            canvasGroupFade = panelFade.GetComponent<CanvasGroup>();
            if (canvasGroupFade == null)
                canvasGroupFade = panelFade.AddComponent<CanvasGroup>();

            DontDestroyOnLoad(panelFade.transform.root.gameObject); // Asegurarse de que el Canvas también persista
            Debug.Log("PanelFade configurado"); // AGREGAR
        }
        else
        {
            Debug.LogError("¡NO HAY PANEL FADE ASIGNADO!"); // AGREGAR
        }
    }

    void Start()
    {
        if (canvasGroupFade != null)
        {
            canvasGroupFade.alpha = 0;
            panelFade.SetActive(false);
        }
    }

    // Cambiar a otra escena
    public void CambiarEscena(string nombreEscena, string puntoSpawn = "")
    {
        // Guardar información para retorno
        escenaAnterior = SceneManager.GetActiveScene().name;

        StartCoroutine(TransicionEscena(nombreEscena, puntoSpawn));
    }

    // Volver a la escena anterior
    public void VolverEscenaAnterior(string puntoSpawn = "")
    {
        if (!string.IsNullOrEmpty(escenaAnterior))
        {
            StartCoroutine(TransicionEscena(escenaAnterior, puntoSpawn));
        }
        else
        {
            Debug.LogWarning("No hay escena anterior guardada");
        }
    }

    IEnumerator TransicionEscena(string nombreEscena, string puntoSpawn)
    {
        // Fade out
        if (panelFade != null)
        {
            panelFade.SetActive(true);
            yield return StartCoroutine(FadeOut());
        }

        // Guardar punto de spawn
        if (!string.IsNullOrEmpty(puntoSpawn))
        {
            PlayerPrefs.SetString("PuntoSpawn", puntoSpawn);
        }

        // Cargar nueva escena
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nombreEscena);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Posicionar jugador
        yield return new WaitForSeconds(0.1f);
        PosicionarJugador();

        // Fade in
        if (panelFade != null)
        {
            yield return StartCoroutine(FadeIn());
            panelFade.SetActive(false);
        }
    }

    IEnumerator FadeOut()
    {
        float tiempo = 0;

        while (tiempo < tiempoFade)
        {
            tiempo += Time.deltaTime;
            canvasGroupFade.alpha = Mathf.Lerp(0, 1, tiempo / tiempoFade);
            yield return null;
        }

        canvasGroupFade.alpha = 1;
    }

    IEnumerator FadeIn()
    {
        float tiempo = 0;

        while (tiempo < tiempoFade)
        {
            tiempo += Time.deltaTime;
            canvasGroupFade.alpha = Mathf.Lerp(1, 0, tiempo / tiempoFade);
            yield return null;
        }

        canvasGroupFade.alpha = 0;
    }

    void PosicionarJugador()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador == null) return;

        // Buscar punto de spawn específico
        string puntoSpawnBuscado = PlayerPrefs.GetString("PuntoSpawn", "");

        GameObject puntoSpawn = null;

        if (!string.IsNullOrEmpty(puntoSpawnBuscado))
        {
            puntoSpawn = GameObject.Find(puntoSpawnBuscado);
        }

        // Si no se encuentra, buscar por tag
        if (puntoSpawn == null)
        {
            puntoSpawn = GameObject.FindGameObjectWithTag("SpawnPoint");
        }

        // Si aún no hay, buscar cualquier SpawnPoint
        if (puntoSpawn == null)
        {
            SpawnPoint[] spawns = FindObjectsOfType<SpawnPoint>();
            if (spawns.Length > 0)
                puntoSpawn = spawns[0].gameObject;
        }

        if (puntoSpawn != null)
        {
            jugador.transform.position = puntoSpawn.transform.position;
            Debug.Log("Jugador posicionado en: " + puntoSpawn.name);
        }
        else
        {
            Debug.LogWarning("No se encontró ningún punto de spawn");
        }

        // Limpiar preferencia
        PlayerPrefs.DeleteKey("PuntoSpawn");
    }

    public string GetEscenaAnterior()
    {
        return escenaAnterior;
    }
}