using UnityEngine;

public class FishSize : MonoBehaviour
{
    [Header("Size Settings")]
    [Tooltip("Tama�o m�nimo aleatorio del pez")]
    public float minSize = 0.4f;

    [Tooltip("Tama�o m�ximo aleatorio del pez")]
    public float maxSize = 1f;

    [Tooltip("Valor de tama�o actual del pez (usado por IA)")]
    public float size;

    private void Awake()
    {
        //Asignar tama�o aleatorio entre minSize y maxSize
        size = Random.Range(minSize, maxSize);

        //Aplicar la escala visual del objeto
        transform.localScale = new Vector3(size, size, 1f);
    }
}
