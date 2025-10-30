using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadMovimiento = 5f;
    public float fuerzaSalto = 10f;

    [Header("Detecci�n de Suelo")]
    public Transform verificadorSuelo;
    public float radioVerificacion = 0.2f;
    public LayerMask capaSuelo;

    [Header("Inventario")]
    public Dictionary<string, int> llaves = new Dictionary<string, int>();
    public List<string> itemsColeccionados = new List<string>();

    [Header("Audio")]
    public AudioClip sonidoSalto;
    public AudioClip sonidoMuerte;
    public AudioClip sonidoRecogerLlave;

    private Rigidbody2D rb;
    private bool enSuelo;
    private float movimientoHorizontal;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private AudioSource audioSource;
    private bool estaMuerto = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (estaMuerto) return;

        // Movimiento horizontal
        movimientoHorizontal = Input.GetAxisRaw("Horizontal");

        // Verificar si est� en el suelo
        enSuelo = Physics2D.OverlapCircle(verificadorSuelo.position, radioVerificacion, capaSuelo);

        // Saltar
        if (Input.GetButtonDown("Jump") && enSuelo)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
            ReproducirSonido(sonidoSalto);
        }

        // Interactuar con puertas/objetos
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interactuar();
        }

        // Voltear sprite seg�n direcci�n
        if (movimientoHorizontal < 0)
            spriteRenderer.flipX = true;
        else if (movimientoHorizontal > 0)
            spriteRenderer.flipX = false;

        // Animaciones (si tienes animator)
        if (animator != null)
        {
            animator.SetFloat("Velocidad", Mathf.Abs(movimientoHorizontal));
            animator.SetBool("EnSuelo", enSuelo);
            animator.SetFloat("VelocidadY", rb.linearVelocity.y);
        }
    }

    void FixedUpdate()
    {
        if (estaMuerto) return;

        // Aplicar movimiento
        rb.linearVelocity = new Vector2(movimientoHorizontal * velocidadMovimiento, rb.linearVelocity.y);
    }

    void Interactuar()
    {
        // Detectar objetos interactuables cercanos
        Collider2D[] objetosCercanos = Physics2D.OverlapCircleAll(transform.position, 1.5f);

        bool interactuoConAlgo = false;

        foreach (Collider2D obj in objetosCercanos)
        {
            IInteractuable interactuable = obj.GetComponent<IInteractuable>();
            if (interactuable != null)
            {
                interactuable.Interactuar(this);
                interactuoConAlgo = true;
                break; // Solo interactuar con un objeto a la vez
            }
        }

        if (!interactuoConAlgo)
        {
            Debug.Log("No hay nada con qu� interactuar cerca");
        }
    }

    public void AgregarLlave(string tipoLlave)
    {
        if (llaves.ContainsKey(tipoLlave))
            llaves[tipoLlave]++;
        else
            llaves.Add(tipoLlave, 1);

        Debug.Log("Llave obtenida: " + tipoLlave);
        ReproducirSonido(sonidoRecogerLlave);

        if (UIManager.Instance != null)
            UIManager.Instance.MostrarMensajeInteraccion("Llave obtenida: " + tipoLlave);
    }

    public bool TieneLlave(string tipoLlave)
    {
        return llaves.ContainsKey(tipoLlave) && llaves[tipoLlave] > 0;
    }

    public void UsarLlave(string tipoLlave)
    {
        if (TieneLlave(tipoLlave))
        {
            llaves[tipoLlave]--;
            if (llaves[tipoLlave] <= 0)
                llaves.Remove(tipoLlave);

            Debug.Log("Llave usada: " + tipoLlave);
        }
    }

    public void AgregarItem(string nombreItem)
    {
        if (!itemsColeccionados.Contains(nombreItem))
        {
            itemsColeccionados.Add(nombreItem);
            Debug.Log("Item coleccionado: " + nombreItem);

            if (UIManager.Instance != null)
                UIManager.Instance.MostrarMensajeInteraccion("Item coleccionado: " + nombreItem);
        }
    }

    public void Morir()
    {
        if (estaMuerto) return;

        estaMuerto = true;
        Debug.Log("¡Has muerto!");

        // Detener movimiento
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static; // Congelar físicas

        // Sonido de muerte
        ReproducirSonido(sonidoMuerte);

        // Animación de muerte (si tienes)
        if (animator != null)
            animator.SetTrigger("Muerte");

        // Desactivar controles
        enabled = false;

        // Mostrar pantalla de Game Over
        if (UIManager.Instance != null)
        {
            UIManager.Instance.MostrarGameOver();
        }
        else
        {
            // Si no hay UIManager, reiniciar después de 2 segundos
            Invoke("ReiniciarDirecto", 2f);
        }
    }

    void ReiniciarDirecto()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    void ReiniciarNivelDirecto()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ReiniciarNivel();
    }

    void ReproducirSonido(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (verificadorSuelo != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(verificadorSuelo.position, radioVerificacion);
        }

        // Dibujar rango de interacci�n
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }
}