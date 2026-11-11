using UnityEngine;

public class Pieza : MonoBehaviour
{
    public float radioRecogida = 2f;
    public bool girar = true;
    public float velocidadGiro = 90f;

    Transform jugador;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) jugador = p.transform;
    }

    void Update()
    {
        if (girar)
            transform.Rotate(0, velocidadGiro * Time.deltaTime, 0, Space.World);

        if (jugador == null) return;

        if (Vector3.Distance(transform.position, jugador.position) <= radioRecogida)
        {
            GameManager.Instance.AgregarPieza();
            Destroy(gameObject);
        }
    }
}