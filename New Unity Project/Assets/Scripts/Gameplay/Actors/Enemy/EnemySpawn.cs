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
        
        Spawn();
    }
	
    void Spawn()
    {
        for(int i = 0; i <= batchSize; i++)
        {
            Transform EnemyRoute = PatrolRouteParent.GetChild(i);
            spawnPostion = EnemyRoute.GetChild(0).position;

            GameObject newEnemey  = Instantiate(enemey, spawnPostion, Quaternion.identity);

            SkeletonBehaviour newEnemeyBehviour = newEnemey.GetComponent<SkeletonBehaviour>();
            //newEnemeyBehviour.Spawner = EnemyRoute.GetChild(0);

            for (int j = 0; j < EnemyRoute.childCount; j++)
            {
                newEnemeyBehviour.PatrolRoute.Add(EnemyRoute.GetChild(j).position);
            }
            newEnemey.transform.parent = transform.GetChild(1);
        }
    }
}
