using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RockSpawn : MonoBehaviour
{
    [Header("Prefab de roca")]
    [SerializeField] private GameObject spawnPrefab;

    [Header("Configuración")]
    [SerializeField] private Transform bordersParent; // Objeto padre "Borders"
    [Range(0f, 0.2f)][SerializeField] private float margenInferior = 0.05f; // Qué tan lejos del borde superior (5%)

    private float minX, maxX, ySpawn;
    private float spawnTime;

    private void Start()
    {
        CalcularLimites();
        spawnTime = Random.Range(1f, 4f);
    }

    private void Update()
    {
        if (GameManager.Instancia == null)
            return;

        if (!(GameManager.Instancia.GetMaquinaDeEstados() == MaquinaDeEstados.JuegoTerminado ||
              GameManager.Instancia.GetMaquinaDeEstados() == MaquinaDeEstados.JuegoGanado))
        {
            spawnTime -= Time.deltaTime;
            if (spawnTime <= 0f)
            {
                Instantiate(spawnPrefab, GetSpawnPosition(), Quaternion.identity);
                spawnTime = Random.Range(3f, 5f);
            }
        }
    }

    private void CalcularLimites()
    {
        if (bordersParent == null)
        {
            return;
        }

        minX = float.MaxValue;
        maxX = float.MinValue;
        float maxY = float.MinValue;

        foreach (Transform border in bordersParent)
        {
            SpriteRenderer sr = border.GetComponent<SpriteRenderer>();
            if (sr == null)
                continue;

            Bounds b = sr.bounds;
            minX = Mathf.Min(minX, b.min.x);
            maxX = Mathf.Max(maxX, b.max.x);
            maxY = Mathf.Max(maxY, b.max.y);
        }

        // spawn justo debajo del borde superior
        float altoTotal = maxY - GetMinY(bordersParent);
        ySpawn = maxY - (altoTotal * margenInferior);

    }

    private float GetMinY(Transform parent)
    {
        float minY = float.MaxValue;
        foreach (Transform border in parent)
        {
            SpriteRenderer sr = border.GetComponent<SpriteRenderer>();
            if (sr != null)
                minY = Mathf.Min(minY, sr.bounds.min.y);
        }
        return minY;
    }

    private Vector3 GetSpawnPosition()
    {
        float xPos = Random.Range(minX, maxX);
        return new Vector3(xPos, ySpawn, 0);
    }
}
