using UnityEngine;

public class Llave : MonoBehaviour, IInteractuable
{
    [Header("Configuración")]
    public string tipoLlave = "LlaveRoja";

    [Header("Efectos")]
    public GameObject efectoRecoleccion;
    public AudioClip sonidoRecoger;

    [Header("Rotación")]
    public bool rotarConstantemente = true;
    public float velocidadRotacion = 100f;

    [Header("Flotación")]
    public bool flotar = true;
    public float amplitudFlotacion = 0.3f;
    public float velocidadFlotacion = 2f;

    private Vector3 posicionInicial;
    private AudioSource audioSource;

    void Start()
    {
        posicionInicial = transform.position;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        // Rotar constantemente
        if (rotarConstantemente)
        {
            transform.Rotate(Vector3.forward * velocidadRotacion * Time.deltaTime);
        }

        // Efecto de flotación
        if (flotar)
        {
            float nuevaY = posicionInicial.y + Mathf.Sin(Time.time * velocidadFlotacion) * amplitudFlotacion;
            transform.position = new Vector3(transform.position.x, nuevaY, transform.position.z);
        }
    }

    public void Interactuar(PlayerController jugador)
    {
        jugador.AgregarLlave(tipoLlave);

        // Efecto visual
        if (efectoRecoleccion != null)
        {
            Instantiate(efectoRecoleccion, transform.position, Quaternion.identity);
        }

        // Sonido
        if (sonidoRecoger != null && audioSource != null)
        {
            AudioSource.PlayClipAtPoint(sonidoRecoger, transform.position);
        }

        Destroy(gameObject);
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