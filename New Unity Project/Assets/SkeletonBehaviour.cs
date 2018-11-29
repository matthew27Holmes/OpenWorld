using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonBehaviour : MonoBehaviour {

    // Use this for initialization
    public List<Transform> PatrolRoute = new List<Transform>();
    public int patrolPoint;
    public Transform LastPlayerPostion;
    NavMeshAgent navMeshAgent;
    int health;
    
	void Start () {
        LastPlayerPostion = null;
        navMeshAgent = GetComponent<NavMeshAgent>();
        health = 100;
        patrolPoint = 0;
	}
	
	// Update is called once per frame
	void Update () {

        if (LastPlayerPostion == null)
        {
            Patrol();
        }
        else
        {
            AttackPlayer();
        }

    }

    void Patrol()
    {
        navMeshAgent.SetDestination(PatrolRoute[patrolPoint].position);
        if(Vector3.Distance(this.transform.position , PatrolRoute[patrolPoint].position) <= 3.0f)//this is hacking
        {
            patrolPoint++;
            if(patrolPoint >= PatrolRoute.Count)
            {
                patrolPoint = 0;
            }
        }

    }
    
   public void PlayerSighted(Transform PlayerPostion)
   {
        LastPlayerPostion = PlayerPostion;
   }

    void AttackPlayer()
    {
        navMeshAgent.SetDestination(LastPlayerPostion.position);
    }
    void LosePlayer()
    {
        LastPlayerPostion = null;
    }
}
