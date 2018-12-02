using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour {

    public float batchSize;
    public GameObject enemey;
    Vector3 spawnPostion;
    Transform PatrolRouteParent;

    // Use this for initialization
    void Start () {
        PatrolRouteParent = transform.GetChild(0);
        spawnPostion = PatrolRouteParent.GetChild(0).GetChild(3).position;
        batchSize = 0;
        Spawn();
    }
	
    void Spawn()
    {
        for(int i = 0;  i <= batchSize; i++)
        {
           GameObject newEnemey  = Instantiate(enemey, spawnPostion, Quaternion.identity);

            SkeletonBehaviour newEnemeyBehviour = newEnemey.GetComponent<SkeletonBehaviour>();
            
            newEnemeyBehviour.Spawner = transform;
            for(int j =0; j < PatrolRouteParent.childCount;j++)
            {
                Transform route = PatrolRouteParent.GetChild(j);
                for (int k = 0; k < route.childCount; k++)
                {
                    newEnemeyBehviour.PatrolRoute.Add(route.GetChild(k));
                }
            }
            newEnemey.transform.parent = transform.GetChild(1);
        }
    }
}
