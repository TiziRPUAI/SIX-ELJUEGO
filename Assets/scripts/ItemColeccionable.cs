using UnityEngine;

public class ItemColeccionable : MonoBehaviour, IInteractuable
{
    [Header("Información")]
    public string nombreItem = "Documento";
    [TextArea(3, 10)]
    public string descripcion = "Un documento misterioso...";

    [Header("Efectos")]
    public GameObject efectoRecoleccion;
    public AudioClip sonidoRecoger;

    [Header("Visual")]
    public bool brillar = true;
    public float velocidadBrillo = 2f;

    private SpriteRenderer spriteRenderer;
    private Color colorOriginal;
    private AudioSource audioSource;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            colorOriginal = spriteRenderer.color;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        // Efecto de brillo
        if (brillar && spriteRenderer != null)
        {
            float brillo = Mathf.PingPong(Time.time * velocidadBrillo, 1f);
            spriteRenderer.color = Color.Lerp(colorOriginal, Color.white, brillo * 0.3f);
        }
    }

    public void Interactuar(PlayerController jugador)
    {
        if (!jugador.itemsColeccionados.Contains(nombreItem))
        {
            jugador.AgregarItem(nombreItem);

            Debug.Log("─────────────────────");
            Debug.Log(nombreItem);
            Debug.Log(descripcion);
            Debug.Log("─────────────────────");

            // Efecto visual
            if (efectoRecoleccion != null)
            {
                Instantiate(efectoRecoleccion, transform.position, Quaternion.identity);
            }

            // Sonido
            if (sonidoRecoger != null)
            {
                AudioSource.PlayClipAtPoint(sonidoRecoger, transform.position);
            }

            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController jugador = other.GetComponent<PlayerController>();
            if (jugador != null)
            {
                Interactuar(jugador);
            }
        }
    }
}