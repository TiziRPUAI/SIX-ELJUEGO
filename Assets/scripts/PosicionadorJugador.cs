using UnityEngine;

public class PosicionadorJugador : MonoBehaviour
{
    void Start()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            SpawnPoint spawn = FindObjectOfType<SpawnPoint>();
            if (spawn != null)
            {
                jugador.transform.position = spawn.transform.position;
                Debug.Log("Jugador posicionado en: " + spawn.gameObject.name);
            }
        }
    }
}