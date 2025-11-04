using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGate : MonoBehaviour
{
    public GameObject[] walls;   // przypisz w Inspectorze Wall1–Wall12
    public GameObject gatePrefab; // przypisz prefab Gate

    void Start()
    {
        SpawnGate();
    }

    void SpawnGate()
    {
        if (walls.Length == 0 || gatePrefab == null)
        {
            Debug.LogWarning("Fuck Yourself");
            return;
        }

        // Losujemy jedn¹ œcianê
        int randomIndex = Random.Range(0, walls.Length);
        GameObject chosenWall = walls[randomIndex];

        // Zapisz pozycjê i rotacjê œciany
        Vector3 pos = chosenWall.transform.position;
        Quaternion rot = chosenWall.transform.rotation;

        // Usuñ œcianê
        Destroy(chosenWall);

        // Utwórz Gate w tym samym miejscu
        Instantiate(gatePrefab, pos, rot);
    }
}
