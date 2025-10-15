using UnityEngine;

public class FishSize : MonoBehaviour
{
    [Header("Size Settings")]
    [Tooltip("Tamaño mínimo aleatorio del pez")]
    public float minSize = 0.4f;

    [Tooltip("Tamaño máximo aleatorio del pez")]
    public float maxSize = 1f;

    [Tooltip("Valor de tamaño actual del pez (usado por IA)")]
    public float size;

    private void Awake()
    {
        //Asignar tamaño aleatorio entre minSize y maxSize
        size = Random.Range(minSize, maxSize);

        //Aplicar la escala visual del objeto
        transform.localScale = new Vector3(size, size, 1f);
    }
}
