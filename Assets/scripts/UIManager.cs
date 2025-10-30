using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Referencias UI")]
    public TextMeshProUGUI textoLlaves;
    public TextMeshProUGUI textoItems;
    public GameObject panelGameOver;
    public GameObject panelVictoria;
    public TextMeshProUGUI textoInteraccion;
    public GameObject panelPausa;

    [Header("Elementos Game Over")]
    public TextMeshProUGUI textoGameOver;
    public TextMeshProUGUI textoSubtituloGameOver;
    public Button botonReintentar;
    public Button botonMenu;

    [Header("Configuración Game Over")]
    public string mensajeGameOver = "HAS MUERTO";
    public string subtituloGameOver = "Los horrores del hotel te han consumido...";

    private PlayerController jugador;
    private bool juegoEnPausa = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // IMPORTANTE: Asegurar que el tiempo est茅 normal al inicio
        Time.timeScale = 1f;

        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
        if (objJugador != null)
            jugador = objJugador.GetComponent<PlayerController>();

        // Ocultar paneles al inicio
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(false);
            Debug.Log("Panel Game Over ocultado al inicio");
        }

        if (panelVictoria != null)
            panelVictoria.SetActive(false);

        if (textoInteraccion != null)
            textoInteraccion.gameObject.SetActive(false);

        if (panelPausa != null)
            panelPausa.SetActive(false);

        // Configurar botones
        ConfigurarBotones();
    }

    void ConfigurarBotones()
    {
        if (botonReintentar != null)
        {
            // Limpiar listeners anteriores
            botonReintentar.onClick.RemoveAllListeners();
            // Agregar nuevo listener
            botonReintentar.onClick.AddListener(ReiniciarJuego);
            Debug.Log("Bot贸n Reintentar configurado");
        }
        else
        {
            Debug.LogWarning("Bot贸n Reintentar no est谩 asignado");
        }

        if (botonMenu != null)
        {
            botonMenu.onClick.RemoveAllListeners();
            botonMenu.onClick.AddListener(IrAlMenuPrincipal);
            Debug.Log("Bot贸n Menu configurado");
        }
        else
        {
            Debug.LogWarning("Bot贸n Menu no est谩 asignado");
        }
    }

    void Update()
    {
        ActualizarInventario();

        // Pausar con ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoEnPausa)
                ReanudarJuego();
            else
                PausarJuego();
        }
    }

    void ActualizarInventario()
    {
        if (jugador == null) return;

        if (textoLlaves != null)
        {
            string infoLlaves = "🔑 LLAVES:\n";

            if (jugador.llaves.Count == 0)
            {
                infoLlaves += "Ninguna";
            }
            else
            {
                foreach (var llave in jugador.llaves)
                {
                    infoLlaves += $"• {llave.Key}: x{llave.Value}\n";
                }
            }

            textoLlaves.text = infoLlaves;
        }

        if (textoItems != null)
        {
            string infoItems = "📦 ITEMS:\n";

            if (jugador.itemsColeccionados.Count == 0)
            {
                infoItems += "Ninguno";
            }
            else
            {
                foreach (var item in jugador.itemsColeccionados)
                {
                    infoItems += $"• {item}\n";
                }
            }

            textoItems.text = infoItems;
        }
    }

    public void MostrarGameOver()
    {
        Debug.Log("UIManager: Mostrando Game Over");

        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);

            // Actualizar textos
            if (textoGameOver != null)
                textoGameOver.text = mensajeGameOver;

            if (textoSubtituloGameOver != null)
                textoSubtituloGameOver.text = subtituloGameOver;

            // Pausar el juego
            Time.timeScale = 0f;

            Debug.Log("Panel Game Over activado - Time.timeScale = 0");
        }
        else
        {
            Debug.LogError("隆Panel Game Over no est谩 asignado en UIManager!");
        }
    }


    public void MostrarVictoria()
    {
        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void MostrarMensajeInteraccion(string mensaje)
    {
        if (textoInteraccion != null)
        {
            textoInteraccion.text = mensaje;
            textoInteraccion.gameObject.SetActive(true);
            CancelInvoke("OcultarMensajeInteraccion");
            Invoke("OcultarMensajeInteraccion", 2f);
        }
    }

    void OcultarMensajeInteraccion()
    {
        if (textoInteraccion != null)
            textoInteraccion.gameObject.SetActive(false);
    }

    public void ReiniciarJuego()
    {
        Debug.Log("=== REINICIANDO JUEGO ===");

        // 1. Restaurar el tiempo
        Time.timeScale = 1f;
        Debug.Log("Time.timeScale restaurado a 1");

        // 2. Ocultar paneles
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(false);
            Debug.Log("Panel Game Over ocultado");
        }

        if (panelPausa != null)
        {
            panelPausa.SetActive(false);
        }

        // 3. Recargar escena
        string escenaActual = SceneManager.GetActiveScene().name;
        Debug.Log("Recargando escena: " + escenaActual);
        SceneManager.LoadScene(escenaActual);
    }

    public void PausarJuego()
    {
        juegoEnPausa = true;
        Time.timeScale = 0f;
        if (panelPausa != null)
            panelPausa.SetActive(true);
    }

    public void ReanudarJuego()
    {
        juegoEnPausa = false;
        Time.timeScale = 1f;
        if (panelPausa != null)
            panelPausa.SetActive(false);
    }

    public void IrAlMenuPrincipal()
    {
        Debug.Log("Volviendo al menú principal...");

        // Restaurar el tiempo
        Time.timeScale = 1f;

        // Ocultar paneles
        if (panelGameOver != null)
            panelGameOver.SetActive(false);

        // Cargar men煤 (escena 0)
        SceneManager.LoadScene(0);
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
public void CargarSiguienteNivel()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
            GameManager.Instance.CargarNivelSiguiente();
    }
}