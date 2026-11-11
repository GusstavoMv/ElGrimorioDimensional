using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Piezas")]
    public int piezasTotales = 10;
    public TextMeshProUGUI textoPiezas;

    [Header("Vidas")]
    public int vidasMaximas = 3;
    public Sprite spriteCorazon;
    public Transform contenedorVidas;
    public float tamanoCorazon = 60f;

    int piezas;
    int vidas;
    List<Image> corazones = new List<Image>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        vidas = vidasMaximas;
        CrearCorazones();
        ActualizarUI();
    }

    void CrearCorazones()
    {
        for (int i = 0; i < vidasMaximas; i++)
        {
            GameObject go = new GameObject("Corazon " + i, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(contenedorVidas, false);
            Image img = go.GetComponent<Image>();
            img.sprite = spriteCorazon;
            img.preserveAspect = true;
            img.rectTransform.sizeDelta = new Vector2(tamanoCorazon, tamanoCorazon);
            corazones.Add(img);
        }
    }

    public void AgregarPieza()
    {
        piezas++;
        ActualizarUI();
        if (piezas >= piezasTotales)
            Debug.Log("¡Todas las piezas recogidas!");
    }

    public void PerderVida()
{
    if (vidas <= 0) return;
    vidas--;
    ActualizarUI();
    if (vidas <= 0)
        Invoke(nameof(Reiniciar), 1.5f);
}

void Reiniciar()
{
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}

    void ActualizarUI()
    {
        if (textoPiezas != null)
            textoPiezas.text = "Piezas: " + piezas + "/" + piezasTotales;

        for (int i = 0; i < corazones.Count; i++)
            corazones[i].color = i < vidas ? Color.white : new Color(1, 1, 1, 0.2f);
    }
}