using UnityEngine;

public class CamaraSeguir : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform jugador;

    [Header("Configuración")]
    public float suavizado = 5f;
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Límites")]
    public bool usarLimites = false;
    public float limiteIzquierdo = -10f;
    public float limiteDerecho = 10f;
    public float limiteArriba = 5f;
    public float limiteAbajo = -5f;

    [Header("Zona Muerta (Deadzone)")]
    public bool usarZonaMuerta = false;
    public float anchoCamaraZonaMuerta = 2f;
    public float altoCamaraZonaMuerta = 1f;

    private Vector3 velocidad = Vector3.zero;

    void Start()
    {
        if (jugador == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            if (obj != null)
                jugador = obj.transform;
        }
    }

    void LateUpdate()
    {
        if (jugador == null) return;

        Vector3 posicionObjetivo = jugador.position + offset;

        // Sistema de zona muerta (la cámara solo se mueve si el jugador sale de cierta área)
        if (usarZonaMuerta)
        {
            Vector3 posicionCamara = transform.position;
            float deltaX = jugador.position.x - posicionCamara.x;
            float deltaY = jugador.position.y - posicionCamara.y;

            if (Mathf.Abs(deltaX) > anchoCamaraZonaMuerta)
            {
                posicionObjetivo.x = jugador.position.x + offset.x;
            }
            else
            {
                posicionObjetivo.x = posicionCamara.x;
            }

            if (Mathf.Abs(deltaY) > altoCamaraZonaMuerta)
            {
                posicionObjetivo.y = jugador.position.y + offset.y;
            }
            else
            {
                posicionObjetivo.y = posicionCamara.y;
            }
        }

        // Aplicar límites
        if (usarLimites)
        {
            posicionObjetivo.x = Mathf.Clamp(posicionObjetivo.x, limiteIzquierdo, limiteDerecho);
            posicionObjetivo.y = Mathf.Clamp(posicionObjetivo.y, limiteAbajo, limiteArriba);
        }

        // Suavizar el movimiento
        transform.position = Vector3.SmoothDamp(transform.position, posicionObjetivo, ref velocidad, 1f / suavizado);
    }

    void OnDrawGizmosSelected()
    {
        // Dibujar límites de la cámara
        if (usarLimites)
        {
            Gizmos.color = Color.yellow;

            Vector3 esquinaSuperiorIzq = new Vector3(limiteIzquierdo, limiteArriba, 0);
            Vector3 esquinaSuperiorDer = new Vector3(limiteDerecho, limiteArriba, 0);
            Vector3 esquinaInferiorIzq = new Vector3(limiteIzquierdo, limiteAbajo, 0);
            Vector3 esquinaInferiorDer = new Vector3(limiteDerecho, limiteAbajo, 0);

            Gizmos.DrawLine(esquinaSuperiorIzq, esquinaSuperiorDer);
            Gizmos.DrawLine(esquinaSuperiorDer, esquinaInferiorDer);
            Gizmos.DrawLine(esquinaInferiorDer, esquinaInferiorIzq);
            Gizmos.DrawLine(esquinaInferiorIzq, esquinaSuperiorIzq);
        }

        // Dibujar zona muerta
        if (usarZonaMuerta && jugador != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 centro = transform.position;
            centro.z = 0;
            Gizmos.DrawWireCube(centro, new Vector3(anchoCamaraZonaMuerta * 2, altoCamaraZonaMuerta * 2, 0));
        }
    }
}