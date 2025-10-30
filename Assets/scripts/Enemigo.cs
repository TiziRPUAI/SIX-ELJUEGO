using UnityEngine;

public class Enemigo : MonoBehaviour
{
    [Header("Patrulla")]
    public Transform puntoA;
    public Transform puntoB;
    public float velocidad = 2f;
    public float tiempoEspera = 1f;

    [Header("Detección")]
    public float rangoDeteccion = 5f;
    public LayerMask capaJugador;
    public bool perseguirJugador = true;

    [Header("Audio")]
    public AudioClip sonidoDeteccion;
    public AudioClip sonidoAtaque;

    [Header("Visual")]
    public Color colorNormal = Color.white;
    public Color colorAlerta = Color.red;

    private Transform objetivo;
    private bool moviendoHaciaB = true;
    private SpriteRenderer spriteRenderer;
    private Transform jugador;
    private bool enAlerta = false;
    private float tiempoEsperaActual = 0f;
    private bool esperando = false;
    private AudioSource audioSource;
    private Animator animator;

    void Start()
    {
        objetivo = puntoB;
        spriteRenderer = GetComponent<SpriteRenderer>();
        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (spriteRenderer != null)
            spriteRenderer.color = colorNormal;
    }

    void Update()
    {
        if (esperando)
        {
            tiempoEsperaActual -= Time.deltaTime;
            if (tiempoEsperaActual <= 0)
            {
                esperando = false;
            }
            return;
        }

        // Verificar si el jugador está cerca
        if (perseguirJugador && jugador != null)
        {
            float distancia = Vector2.Distance(transform.position, jugador.position);

            if (distancia < rangoDeteccion)
            {
                if (!enAlerta)
                {
                    enAlerta = true;
                    if (spriteRenderer != null)
                        spriteRenderer.color = colorAlerta;
                    ReproducirSonido(sonidoDeteccion);
                }

                // Perseguir al jugador
                Perseguir();
                return;
            }
            else if (enAlerta && distancia >= rangoDeteccion * 1.5f)
            {
                // Volver a patrullar
                enAlerta = false;
                if (spriteRenderer != null)
                    spriteRenderer.color = colorNormal;
            }
        }

        // Patrullar entre dos puntos
        Patrullar();
    }

    void Patrullar()
    {
        if (objetivo == null || puntoA == null || puntoB == null) return;

        // Moverse hacia el objetivo
        Vector2 posicionActual = transform.position;
        Vector2 posicionObjetivo = objetivo.position;

        transform.position = Vector2.MoveTowards(posicionActual, posicionObjetivo, velocidad * Time.deltaTime);

        // Voltear sprite
        float direccion = posicionObjetivo.x - posicionActual.x;
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direccion < 0;
        }

        // Animación
        if (animator != null)
        {
            animator.SetBool("Caminando", true);
        }

        // Cambiar objetivo al llegar
        if (Vector2.Distance(transform.position, objetivo.position) < 0.1f)
        {
            esperando = true;
            tiempoEsperaActual = tiempoEspera;

            if (animator != null)
            {
                animator.SetBool("Caminando", false);
            }

            if (moviendoHaciaB)
            {
                objetivo = puntoA;
                moviendoHaciaB = false;
            }
            else
            {
                objetivo = puntoB;
                moviendoHaciaB = true;
            }
        }
    }

    void Perseguir()
    {
        if (jugador == null) return;

        // Moverse hacia el jugador
        Vector2 direccion = (jugador.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, jugador.position, velocidad * 1.5f * Time.deltaTime);

        // Voltear sprite
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direccion.x < 0;
        }

        // Animación
        if (animator != null)
        {
            animator.SetBool("Caminando", true);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("¡ENEMIGO TOCÓ AL JUGADOR!");

            PlayerController jugador = collision.gameObject.GetComponent<PlayerController>();

            if (jugador != null)
            {
                // Efecto visual (opcional: empujar al jugador)
                Rigidbody2D rbJugador = collision.gameObject.GetComponent<Rigidbody2D>();
                if (rbJugador != null)
                {
                    Vector2 direccion = (collision.transform.position - transform.position).normalized;
                    rbJugador.AddForce(direccion * 5f, ForceMode2D.Impulse);
                }

                // Sonido de ataque
                ReproducirSonido(sonidoAtaque);

                // Matar al jugador
                jugador.Morir();
            }
        }
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
        // Dibujar línea de patrulla
        if (puntoA != null && puntoB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(puntoA.position, puntoB.position);
            Gizmos.DrawWireSphere(puntoA.position, 0.3f);
            Gizmos.DrawWireSphere(puntoB.position, 0.3f);
        }

        // Dibujar rango de detección
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
    }
}