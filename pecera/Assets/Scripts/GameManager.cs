using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia { get; private set; }

    private MaquinaDeEstados maquinaDeEstados;

    [SerializeField] private GameObject perdistePanel;
    [SerializeField] private GameObject ganastePanel;
    [SerializeField] private PlayerUI playerUi;
    [SerializeField] private Button botonAddFish;

    private void Awake()
    {
        if (Instancia != null) Destroy(gameObject);
        else Instancia = this;
    }

    public void ActualizarMaquinaDeEstados(MaquinaDeEstados nuevoEstado)
    {
        maquinaDeEstados = nuevoEstado;

        switch (nuevoEstado)
        {
            case MaquinaDeEstados.Jugando:
                botonAddFish.interactable = true;
                break;
            case MaquinaDeEstados.JuegoTerminado:
                perdistePanel.SetActive(true);
                botonAddFish.interactable = false;
                break;
            case MaquinaDeEstados.JuegoGanado:
                ganastePanel.SetActive(true);
                botonAddFish.interactable = false;
                break;
        }
    }

    public void ActualizarPuntos()
    {
        playerUi.ActualizarPuntos();
    }

    public void ActualizarPeces(int cantidad)
    {
        playerUi.ActualizarPeces(cantidad);
    }

    public MaquinaDeEstados GetMaquinaDeEstados() => maquinaDeEstados;
}

public enum MaquinaDeEstados
{
    Jugando,
    JuegoTerminado,
    JuegoGanado
}
