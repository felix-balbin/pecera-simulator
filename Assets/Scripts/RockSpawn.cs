using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class RockSpawn : MonoBehaviour
{
    [SerializeField] private GameObject spawnPrefab;
    private float spawnTime;

    private void Start()
    {
        spawnTime = Random.Range(1f, 4f);
    }

    private void Update()
    {
        if (!(GameManager.Instancia.GetMaquinaDeEstados() == MaquinaDeEstados.JuegoTerminado || GameManager.Instancia.GetMaquinaDeEstados() == MaquinaDeEstados.JuegoGanado))
        {
            spawnTime -= Time.deltaTime;
            if (spawnTime <= 0f)
            {
                Instantiate(spawnPrefab, GetSpawnPosition(), Quaternion.identity);
                spawnTime = Random.Range(3f, 5f);
            } 
        }

    }

    private Vector3 GetSpawnPosition()
    {
        Vector2 limitesPantalla = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        float xPos = Random.Range(-limitesPantalla.x, limitesPantalla.x);

        float yPos = limitesPantalla.y + 0.5f;

        return new Vector3(xPos, yPos, 0);
    }
}
