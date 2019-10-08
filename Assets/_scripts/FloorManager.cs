using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    public GameObject FloorOne;
    public GameObject FloorTwo;

    public int gridSize = 5;
    // Start is called before the first frame update
    void Start()
    {
        int tileSize = 10;
        Vector3 floorSpawn = new Vector3(0, -50, 0);
        Vector3 wallSpawn = new Vector3(0, 0, gridSize * tileSize);
        for (int i = -gridSize; i < gridSize; i++)
        {
            for (int j = -gridSize; j < gridSize; j++)
            {
                wallSpawn.x = i * tileSize;
                wallSpawn.y = j * tileSize;

                floorSpawn.x = i * tileSize;
                floorSpawn.z = j * tileSize;

                GameObject floorTile = null;
                GameObject wallTile = null;

                //Quaternion wallRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
                var spawnWall = j + tileSize >= 0;
                spawnWall = false;

                if (Mathf.Abs(i) % 2 == Mathf.Abs(j) % 2)
                {
                    if (spawnWall)
                    {
                        wallTile = GameObject.Instantiate(FloorOne, wallSpawn, Quaternion.identity);
                    }
                    floorTile = GameObject.Instantiate(FloorOne, floorSpawn, Quaternion.identity);
                }
                else
                {
                    if (spawnWall)
                    {
                        wallTile = GameObject.Instantiate(FloorTwo, wallSpawn, Quaternion.identity);
                    }
                    floorTile = GameObject.Instantiate(FloorTwo, floorSpawn, Quaternion.identity);
                }
                if (spawnWall)
                {
                    wallTile.transform.Rotate(270, 0, 0, Space.World);
                    wallTile.transform.parent = gameObject.transform;
                }
                floorTile.transform.parent = gameObject.transform;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
