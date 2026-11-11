using UnityEngine;

public class AlienScript : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;
    public Animator animator;

    [Header("Velocidades")]
    public float velocidadPatrulla = 1.5f;
    public float velocidadPersecucion = 3.5f;
    public float velocidadGiro = 5f;

    [Header("Deteccion y ataque")]
    public float radioDeteccion = 10f;
    public float distanciaAtaque = 2f;
    public float tiempoEntreAtaques = 1.5f;

    [Header("Patrulla")]
    public float radioPatrulla = 15f;
    public float tiempoEspera = 2f;

    [Header("Vida")]
    public int vida = 3;

    Vector3 puntoInicial;
    Vector3 destino;
    float esperaActual;
    float proximoAtaque;
    bool muerto;

    void Start()
    {
        puntoInicial = transform.position;
        if (animator == null) animator = GetComponentInChildren<Animator>();

        if (jugador == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) jugador = p.transform;
        }
        ElegirDestino();
    }

    void Update()
    {
        if (muerto || jugador == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia <= distanciaAtaque)
        {
            Atacar();
        }
        else if (distancia <= radioDeteccion)
        {
            Mover(jugador.position, velocidadPersecucion);
        }
        else
        {
            Patrullar();
        }
    }

    void Patrullar()
    {
        float d = Vector3.Distance(transform.position, destino);
        if (d < 1f)
        {
            esperaActual += Time.deltaTime;
            if (esperaActual >= tiempoEspera)
            {
                ElegirDestino();
                esperaActual = 0;
            }
        }
        else
        {
            Mover(destino, velocidadPatrulla);
        }
    }

    void ElegirDestino()
    {
        Vector2 r = Random.insideUnitCircle * radioPatrulla;
        destino = new Vector3(puntoInicial.x + r.x, transform.position.y, puntoInicial.z + r.y);
    }

    void Mover(Vector3 objetivo, float velocidad)
    {
        Vector3 dir = objetivo - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude < 0.01f) return;

        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, velocidadGiro * Time.deltaTime);
        transform.position += transform.forward * velocidad * Time.deltaTime;
    }

void Atacar()
{
    Vector3 dir = jugador.position - transform.position;
    dir.y = 0;
    if (dir.sqrMagnitude > 0.01f)
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), velocidadGiro * Time.deltaTime);

    if (Time.time >= proximoAtaque)
    {
        proximoAtaque = Time.time + tiempoEntreAtaques;
        animator.SetTrigger("Attack");
        GameManager.Instance.PerderVida();
    }
}

    public void RecibirDano(int cantidad)
    {
        if (muerto) return;
        vida -= cantidad;
        if (vida <= 0) Morir();
    }

    void Morir()
    {
        muerto = true;
        animator.SetTrigger("Die");
        Destroy(gameObject, 5f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }
}