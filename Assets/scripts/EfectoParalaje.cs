using UnityEngine;

public class EfectoParalaje : MonoBehaviour
{
    [Header("Configuración")]
    public Transform camara;
    public float velocidadParalaje = 0.5f;

    [Header("Repetición Infinita")]
    public bool repetirInfinito = true;
    public float anchoSprite = 20f;

    private Vector3 posicionInicialCamara;
    private Vector3 posicionInicial;

    void Start()
    {
        if (camara == null)
            camara = Camera.main.transform;

        posicionInicialCamara = camara.position;
        posicionInicial = transform.position;
    }

    void LateUpdate()
    {
        // Calcular distancia que se movió la cámara
        Vector3 distanciaCamara = camara.position - posicionInicialCamara;

        // Aplicar paralaje
        Vector3 nuevaPosicion = posicionInicial + distanciaCamara * velocidadParalaje;
        transform.position = nuevaPosicion;

        // Repetición infinita
        if (repetirInfinito)
        {
            float distanciaRelativa = camara.position.x * (1 - velocidadParalaje);

            if (distanciaRelativa > posicionInicial.x + anchoSprite)
            {
                posicionInicial.x += anchoSprite;
            }
            else if (distanciaRelativa < posicionInicial.x - anchoSprite)
            {
                posicionInicial.x -= anchoSprite;
            }
        }
    }
}