using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text textoPuntos;
    [SerializeField] private TMP_Text textoPeces;

    private int puntos = 0;
    private int peces = 0;

    public void ActualizarPuntos()
    {
        puntos++;
        textoPuntos.text = "Puntos: " + puntos.ToString();
    }

    public void ActualizarPeces(int cantidad)
    {
        peces += cantidad;

        // Evitamos que los peces sean negativos
        if (peces < 0)
            peces = 0;

        textoPeces.text = "Peces: " + peces.ToString();
    }

    public int GetPuntos() => puntos;
}
