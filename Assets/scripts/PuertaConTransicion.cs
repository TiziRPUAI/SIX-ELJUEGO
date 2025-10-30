using UnityEngine;

public class PuertaConTransicion : MonoBehaviour, IInteractuable
{
    [Header("Configuración de Escena")]
    public string escenaDestino = "Habitacion01";
    public string puntoSpawnDestino = "SpawnHabitacion";

    [Header("Tipo de Puerta")]
    public TipoPuerta tipoPuerta = TipoPuerta.CambioEscena;

    [Header("Requisitos")]
    public bool requiereLlave = false;
    public string tipoLlaveRequerida = "LlaveRoja";

    [Header("Visual")]
    public SpriteRenderer spriteRenderer;
    public Sprite spriteCerrada;
    public Sprite spriteAbierta;
    public Color colorBloqueada = Color.red;
    public Color colorDesbloqueada = Color.green;

    [Header("Audio")]
    public AudioClip sonidoPuertaAbrir;
    public AudioClip sonidoPuertaCerrada;

    [Header("Indicador")]
    public GameObject indicadorInteraccion;

    private bool abierta = false;
    private bool jugadorCerca = false;
    private AudioSource audioSource;

    public enum TipoPuerta
    {
        CambioEscena,
        RetornoEscena
    }

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (indicadorInteraccion != null)
            indicadorInteraccion.SetActive(false);

        ActualizarVisual();
    }

    void Update()
    {
        if (indicadorInteraccion != null)
        {
            indicadorInteraccion.SetActive(jugadorCerca && !abierta);
        }
    }

    public void Interactuar(PlayerController jugador)
    {
        // Si la puerta ya está abierta, pasar directamente
        if (abierta)
        {
            PasarPorPuerta();
            return;
        }

        // Si no requiere llave, abrir directamente
        if (!requiereLlave)
        {
            Abrir();
            PasarPorPuerta();
            return;
        }

        // Verificar si tiene la llave
        if (jugador.TieneLlave(tipoLlaveRequerida))
        {
            jugador.UsarLlave(tipoLlaveRequerida);
            Abrir();
            PasarPorPuerta();

            if (UIManager.Instance != null)
                UIManager.Instance.MostrarMensajeInteraccion("Puerta abierta");
        }
        else
        {
            // No tiene la llave
            ReproducirSonido(sonidoPuertaCerrada);

            string mensaje = "Necesitas: " + tipoLlaveRequerida;
            Debug.Log(mensaje);

            if (UIManager.Instance != null)
                UIManager.Instance.MostrarMensajeInteraccion(mensaje);
        }
    }

    void Abrir()
    {
        abierta = true;
        ActualizarVisual();
        ReproducirSonido(sonidoPuertaAbrir);
        Debug.Log("¡Puerta abierta!");
    }

    void PasarPorPuerta()
    {
        if (SceneTransitionManager.Instance == null)
        {
            Debug.LogError("No hay SceneTransitionManager en la escena");
            return;
        }

        switch (tipoPuerta)
        {
            case TipoPuerta.CambioEscena:
                Debug.Log($"Cambiando a escena: {escenaDestino}");
                SceneTransitionManager.Instance.CambiarEscena(escenaDestino, puntoSpawnDestino);
                break;

            case TipoPuerta.RetornoEscena:
                Debug.Log("Volviendo a escena anterior");
                SceneTransitionManager.Instance.VolverEscenaAnterior(puntoSpawnDestino);
                break;
        }
    }

    void ActualizarVisual()
    {
        if (spriteRenderer == null) return;

        if (abierta)
        {
            if (spriteAbierta != null)
                spriteRenderer.sprite = spriteAbierta;
            spriteRenderer.color = colorDesbloqueada;
        }
        else
        {
            if (spriteCerrada != null)
                spriteRenderer.sprite = spriteCerrada;
            spriteRenderer.color = requiereLlave ? colorBloqueada : Color.white;
        }
    }

    void ReproducirSonido(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }
}