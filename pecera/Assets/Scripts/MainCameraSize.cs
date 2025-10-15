using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class MainCameraSize : MonoBehaviour
{
    [Header("Tamaño fijo deseado")]
    [Tooltip("Altura visible total en unidades del mundo")]
    public float fixedHeight = 34.2f;

    [Tooltip("Ancho visible total en unidades del mundo")]
    public float fixedWidth = 83f;

    private Camera cam;

    void OnEnable()
    {
        cam = GetComponent<Camera>();
        ApplyFixedView();
    }

    void Update()
    {
#if UNITY_EDITOR
        ApplyFixedView(); // Mantener en editor
#endif
    }

    void ApplyFixedView()
    {
        if (cam == null) cam = GetComponent<Camera>();
        cam.orthographic = true;

        float targetAspect = fixedWidth / fixedHeight;
        float windowAspect = (float)Screen.width / Screen.height;

        // Calculamos cómo ajustar según el aspect ratio real
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            // Añade bandas arriba y abajo (letterbox)
            Rect rect = cam.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            cam.rect = rect;
        }
        else
        {
            // Añade bandas a los lados (pillarbox)
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            cam.rect = rect;
        }

        cam.orthographicSize = fixedHeight / 2f;
    }
}
