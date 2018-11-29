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
        batchSize = 0;
        Spawn();
    }
	
    void Spawn()
    {
        for(int i = 0;  i <= batchSize; i++)
        {
           GameObject newEnemey  = Instantiate(enemey, spawnPostion, Quaternion.identity);

            SkeletonBehaviour newEnemeyBehviour = newEnemey.GetComponent<SkeletonBehaviour>();

            for(int j =0; j < transform.childCount;j++)
            {
                Transform route = transform.GetChild(j);
                for (int k = 0; k < route.childCount; k++)
                {
                    newEnemeyBehviour.PatrolRoute.Add(route.GetChild(k));
                }
            }
            
        }
    }
}
