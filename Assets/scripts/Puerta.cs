//using UnityEngine;

//public class Puerta : MonoBehaviour, IInteractuable
//{
//    [Header("Configuración")]
//    public string tipoLlaveRequerida = "LlaveRoja";
//    public bool requiereLlave = true;
//    public GameObject habitacionDestino; // Punto de spawn al entrar
//    public bool esPuertaFinal = false;

//    [Header("Visual")]
//    public SpriteRenderer spriteRenderer;
//    public Sprite spriteCerrada;
//    public Sprite spriteAbierta;
//    public Color colorBloqueada = Color.red;
//    public Color colorDesbloqueada = Color.green;

//    [Header("Audio")]
//    public AudioClip sonidoPuertaCerrada;
//    public AudioClip sonidoPuertaAbrir;

//    [Header("Indicador Visual")]
//    public GameObject indicadorInteraccion;

//    private bool abierta = false;
//    private AudioSource audioSource;
//    private bool jugadorCerca = false;

//    void Start()
//    {
//        if (spriteRenderer == null)
//            spriteRenderer = GetComponent<SpriteRenderer>();

//        audioSource = GetComponent<AudioSource>();
//        if (audioSource == null)
//            audioSource = gameObject.AddComponent<AudioSource>();

//        ActualizarVisual();

//        if (indicadorInteraccion != null)
//            indicadorInteraccion.SetActive(false);
//    }

//    void Update()
//    {
//        // Mostrar indicador si el jugador está cerca
//        if (indicadorInteraccion != null)
//            indicadorInteraccion.SetActive(jugadorCerca);
//    }

//    public void Interactuar(PlayerController jugador)
//    {
//        if (abierta)
//        {
//            // Entrar a la habitación o ganar el juego
//            if (esPuertaFinal)
//            {
//                Debug.Log("¡HAS GANADO EL JUEGO!");
//                if (UIManager.Instance != null)
//                    UIManager.Instance.MostrarVictoria();
//                return;
//            }

//            if (habitacionDestino != null)
//            {
//                jugador.transform.position = habitacionDestino.transform.position;
//                Debug.Log("Entrando a la habitación...");
//            }
//            return;
//        }

//        if (!requiereLlave)
//        {
//            Abrir();
//            return;
//        }

//        if (jugador.TieneLlave(tipoLlaveRequerida))
//        {
//            jugador.UsarLlave(tipoLlaveRequerida);
//            Abrir();

//            if (UIManager.Instance != null)
//                UIManager.Instance.MostrarMensajeInteraccion("Puerta abierta");
//        }
//        else
//        {
//            string mensaje = "Necesitas: " + tipoLlaveRequerida;
//            Debug.Log(mensaje);

//            ReproducirSonido(sonidoPuertaCerrada);

//            if (UIManager.Instance != null)
//                UIManager.Instance.MostrarMensajeInteraccion(mensaje);
//        }
//    }

//    void Abrir()
//    {
//        abierta = true;
//        ActualizarVisual();
//        ReproducirSonido(sonidoPuertaAbrir);
//        Debug.Log("¡Puerta abierta!");
//    }

//    void ActualizarVisual()
//    {
//        if (spriteRenderer != null)
//        {
//            if (abierta)
//            {
//                if (spriteAbierta != null)
//                    spriteRenderer.sprite = spriteAbierta;
//                spriteRenderer.color = colorDesbloqueada;
//            }
//            else
//            {
//                if (spriteCerrada != null)
//                    spriteRenderer.sprite = spriteCerrada;
//                spriteRenderer.color = colorBloqueada;
//            }
//        }
//    }

//    void ReproducirSonido(AudioClip clip)
//    {
//        if (audioSource != null && clip != null)
//        {
//            audioSource.PlayOneShot(clip);
//        }
//    }

//    void OnTriggerEnter2D(Collider2D other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            jugadorCerca = true;
//        }
//    }

//    void OnTriggerExit2D(Collider2D other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            jugadorCerca = false;
//        }
//    }
//}
using UnityEngine;
using UnityEngine.SceneManagement;

public class Puerta : MonoBehaviour
{
    [Header("Configuración")]
    public string escenaDestino = "Habitacion01";
    public bool requiereLlave = true;
    public string tipoLlaveRequerida = "LlaveRoja";

    [Header("Visual")]
    public SpriteRenderer spriteRenderer;
    public Color colorCerrada = Color.red;
    public Color colorAbierta = Color.green;

    private bool jugadorCerca = false;
    private bool abierta = false;

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        ActualizarColor();
    }

    void Update()
    {
        // Si el jugador está cerca y presiona E
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("=== TECLA E PRESIONADA ===");
            IntentarAbrir();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Algo entró al trigger: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log(">>> JUGADOR CERCA DE LA PUERTA <<<");
            jugadorCerca = true;
            MostrarMensaje("Presiona E para interactuar");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jugador se alejó");
            jugadorCerca = false;
        }
    }

    void IntentarAbrir()
    {
        Debug.Log("IntentarAbrir() llamado");

        // Si ya está abierta
        if (abierta)
        {
            Debug.Log("Puerta ya abierta, cambiando escena...");
            CambiarEscena();
            return;
        }

        // Si no requiere llave
        if (!requiereLlave)
        {
            Debug.Log("Puerta sin llave, abriendo...");
            Abrir();
            CambiarEscena();
            return;
        }

        // Buscar jugador
        GameObject jugadorObj = GameObject.FindGameObjectWithTag("Player");
        if (jugadorObj == null)
        {
            Debug.LogError("NO SE ENCONTRÓ EL JUGADOR");
            return;
        }

        PlayerController jugador = jugadorObj.GetComponent<PlayerController>();
        if (jugador == null)
        {
            Debug.LogError("Jugador no tiene PlayerController");
            return;
        }

        // Verificar llave
        if (jugador.TieneLlave(tipoLlaveRequerida))
        {
            Debug.Log("✓ Jugador tiene la llave: " + tipoLlaveRequerida);
            jugador.UsarLlave(tipoLlaveRequerida);
            Abrir();
            MostrarMensaje("Puerta abierta");

            // Esperar un poco antes de cambiar
            Invoke("CambiarEscena", 0.5f);
        }
        else
        {
            Debug.Log("✗ Jugador NO tiene la llave");
            MostrarMensaje("Necesitas: " + tipoLlaveRequerida);
        }
    }

    void Abrir()
    {
        abierta = true;
        ActualizarColor();
        Debug.Log("*** PUERTA ABIERTA ***");
    }

    void CambiarEscena()
    {
        Debug.Log("╔════════════════════════════════╗");
        Debug.Log("║ CAMBIANDO A ESCENA: " + escenaDestino);
        Debug.Log("╚════════════════════════════════╝");

        SceneManager.LoadScene(escenaDestino);
    }

    void ActualizarColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = abierta ? colorAbierta : colorCerrada;
        }
    }

    void MostrarMensaje(string mensaje)
    {
        Debug.Log("MENSAJE: " + mensaje);

        // Intentar mostrar en UI si existe
        if (UIManager.Instance != null)
        {
            UIManager.Instance.MostrarMensajeInteraccion(mensaje);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualizar área de detección
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}