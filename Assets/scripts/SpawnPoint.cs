using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("Identificación")]
    public string idSpawn = "SpawnPoint01";

    [Header("Visual (Solo en Editor)")]
    public Color colorGizmo = Color.green;
    public float tamañoGizmo = 0.5f;

    void OnDrawGizmos()
    {
        Gizmos.color = colorGizmo;
        Gizmos.DrawWireSphere(transform.position, tamañoGizmo);

        // Dibujar dirección
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.right * tamañoGizmo * 2);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, tamañoGizmo);
    }
}