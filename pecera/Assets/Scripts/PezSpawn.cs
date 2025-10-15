using UnityEngine;
using UnityEngine.UI;

public class PezSpawn : MonoBehaviour
{
    [Header("Prefabs de peces")]
    [SerializeField] private GameObject spawnPrefab1;
    [SerializeField] private GameObject spawnPrefab2;
    [SerializeField] private int numPeces = 10;

    [Header("Referencias")]
    [SerializeField] private Button btnGeneraPeces;
    [SerializeField] private Transform bordersParent; // El objeto "Borders"

    [Header("Configuración del área de spawn")]
    [Range(0f, 0.4f)][SerializeField] private float margenInterno = 0.10f; // 10% del área visible

    private float minX, maxX, minY, maxY;

    void Awake()
    {
        if (btnGeneraPeces != null)
            btnGeneraPeces.onClick.AddListener(CrearPez);

        CalcularLimites();
    }

    private void Start()
    {
        for (int i = 0; i < numPeces; i++)
        {
            CrearPez();
        }
    }

    public void CrearPez()
    {
        int numero = Random.Range(1, 3);
        GameObject prefabSeleccionado = numero == 1 ? spawnPrefab1 : spawnPrefab2;

        if (prefabSeleccionado != null)
        {
            Instantiate(prefabSeleccionado, GetSpawnPosition(), Quaternion.identity);
            GameManager.Instancia.ActualizarPeces(+1);
        }
    }

    private void CalcularLimites()
    {
        if (bordersParent == null)
        {
            return;
        }

        // Reiniciamos los valores de los límites
        minX = float.MaxValue;
        maxX = float.MinValue;
        minY = float.MaxValue;
        maxY = float.MinValue;

        //  SpriteRenderer dentro del objeto Borders
        foreach (Transform border in bordersParent)
        {
            SpriteRenderer sr = border.GetComponent<SpriteRenderer>();
            if (sr == null)
                continue;

            Bounds b = sr.bounds; // límites del sprite en el mundo

            minX = Mathf.Min(minX, b.min.x);
            maxX = Mathf.Max(maxX, b.max.x);
            minY = Mathf.Min(minY, b.min.y);
            maxY = Mathf.Max(maxY, b.max.y);
        }

        // Aplicamos un margen interno proporcional
        float ancho = maxX - minX;
        float alto = maxY - minY;
        float margenX = ancho * margenInterno;
        float margenY = alto * margenInterno;

        minX += margenX;
        maxX -= margenX;
        minY += margenY;
        maxY -= margenY;

    }

    private Vector3 GetSpawnPosition()
    {
        Vector2 limitesPantalla = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        float aleatorioVertical = Random.Range(minY, maxY);
        float aleatorioHorizontal = Random.Range(0, 2) == 0 ? minX + 1f : maxX - 1f;

        return new Vector3(aleatorioHorizontal, aleatorioVertical, 0);
    }
}
