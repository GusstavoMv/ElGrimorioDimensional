using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal destino;
    public float radioActivacion = 1.5f;
    public float tiempoEspera = 2f;
    public Vector3 desfaseSalida = new Vector3(0, 0.2f, 0);

    public float bloqueadoHasta;
    Transform jugador;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) jugador = p.transform;
    }

    void Update()
    {
        if (jugador == null || destino == null) return;
        if (Time.time < bloqueadoHasta) return;

        Vector3 a = transform.position; a.y = 0;
        Vector3 b = jugador.position; b.y = 0;

        if (Vector3.Distance(a, b) <= radioActivacion)
            Teletransportar();
    }

    void Teletransportar()
    {
        destino.bloqueadoHasta = Time.time + tiempoEspera;
        bloqueadoHasta = Time.time + tiempoEspera;

        CharacterController cc = jugador.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        jugador.position = destino.transform.position + desfaseSalida;

        if (cc != null) cc.enabled = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radioActivacion);
    }
}