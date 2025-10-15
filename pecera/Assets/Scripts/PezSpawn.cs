using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class PezSpawn : MonoBehaviour
{
    [SerializeField] private GameObject spawnPrefab1;
    [SerializeField] private GameObject spawnPrefab2;
    // [SerializeField] private GameObject spawnPrefab3;
    [SerializeField] private int numPeces;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Button btnGeneraPeces;
    void Awake()
    {
        btnGeneraPeces.onClick.AddListener(() =>
        {
            Debug.Log("Ejemplo");
        });
    }
    private void Start()
    {
        for (int i = 0; i < numPeces; i++)
        {
            int numero = Random.Range(1, 3); // Devuelve 1, 2 o 3

            GameObject prefabSeleccionado = null;

            switch (numero)
            {
                case 1:
                    prefabSeleccionado = spawnPrefab1;
                    break;
                case 2:
                    prefabSeleccionado = spawnPrefab2;
                    break;
                // case 3:
                //     prefabSeleccionado = spawnPrefab3;
                //     break;
            }

            if (prefabSeleccionado != null)
            {
                Instantiate(prefabSeleccionado, GetSpawnPosition(), Quaternion.identity);
                GameManager.Instancia.ActualizarPeces(+1);
            }
        }
    }
    public void CrearPez()
    {
        int numero = Random.Range(1, 3); // Devuelve 10 2
        GameObject prefabSeleccionado = null;

        switch (numero)
        {
            case 1:
                prefabSeleccionado = spawnPrefab1;
                break;
            case 2:
                prefabSeleccionado = spawnPrefab2;
                break;
        }

        if (prefabSeleccionado != null)
        {
            Instantiate(prefabSeleccionado, GetSpawnPosition(), Quaternion.identity);
            GameManager.Instancia.ActualizarPeces(+1);
        }
    }

    private Vector3 GetSpawnPosition()
    {
        Vector2 limitesPantalla = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        float aleatorioVertical = Random.Range(-limitesPantalla.y, limitesPantalla.y);
        float aleatorioHorizontal = Random.Range(0, 2) == 0 ? -limitesPantalla.x - 0.5f : aleatorioHorizontal = limitesPantalla.x + 0.5f;

        return new Vector3(aleatorioHorizontal, aleatorioVertical, 0);
    }
        // Update is called once per frame
    // private void Update()
    // {
    //     spawnTime = spawnTime - Time.deltaTime;
    //     if (spawnTime <= 0) {
    //         Instantiate(spawnPrefab, GetSpawnPosition(), Quaternion.identity);
    //         spawnTime = Random.Range(1, 4);
    //     }
    // }

}
