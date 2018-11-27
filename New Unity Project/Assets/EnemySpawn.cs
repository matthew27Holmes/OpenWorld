using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour {

    public float batchSize;
    public GameObject enemey;
    Vector3 spawnPostion;
	// Use this for initialization
	void Start () {
        spawnPostion = this.transform.position;
        batchSize = 3;
        Spawn();
    }
	
    void Spawn()
    {
        for(int i = 0;  i <= batchSize; i++)
        {
           GameObject newEnemey  = Instantiate(enemey, spawnPostion, Quaternion.identity);
            //also need to pass node grid to children
        }
    }
}
