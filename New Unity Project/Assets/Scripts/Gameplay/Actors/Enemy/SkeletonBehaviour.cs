using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonBehaviour : MonoBehaviour {

    // Use this for initialization
    public List<Transform> PatrolRoute = new List<Transform>();
    public int patrolPoint;
    public Vector3 LastPlayerPostion;
    public Transform Spawner = null;
    NavMeshAgent navMeshAgent;
    Animator anim;
    int health;
    bool alive;
    
	void Start () {
        LastPlayerPostion = new Vector3(0,0,0);
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        health = 100;
        alive = true;
        patrolPoint = 0;
	}
	
	// Update is called once per frame
	void Update () {

        if (alive)
        {
            if (LastPlayerPostion == new Vector3(0, 0, 0))
            {
                GameObject parentSpwner = GameObject.Find(Spawner.gameObject.name);
                anim.SetBool("ChasePlayer", false);
                anim.SetBool("Patroling", true);

                if (parentSpwner != null)
                {
                    Patrol();
                }
                else
                {
                    //return home and wait 
                    navMeshAgent.SetDestination(Spawner.position);
                }
            }
            //else
            //{
            //    moveToPlayer();// this migth be wrong
           // }
        }
        else
        {
            Destroy(gameObject);
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
        anim.SetBool("PlayerSpotted", true);
        Vector3 offset = new Vector3(1, 0, 1);
        LastPlayerPostion = PlayerPostion.position;
        //LastPlayerPostion =- offset;moveToPlayer()
        moveToPlayer();
   }

    void moveToPlayer()
    {
        navMeshAgent.SetDestination(LastPlayerPostion);
        anim.SetBool("Patroling", false);
        anim.SetBool("AttackPlayer", false);
        anim.SetBool("ChasePlayer", true);
    }
    void AttackPlayer()
    {
        navMeshAgent.SetDestination(transform.position);
        //if (Vector3.Distance(this.transform.position, LastPlayerPostion) <= 3.0f)//this is hacking
        //{
        anim.SetBool("AttackPlayer", true);
        //}
    }

    public void TakeDamge(int Damge,bool kicked)
    {
        health =- Damge;
        anim.SetBool("Hit", true);
        anim.SetBool("Kicked", kicked);
    }

    void Die()
    {
        if(health <= 0 )
        {
            alive = false;
            anim.SetInteger("Health", health);
        }
    }

    void LosePlayer()
    {
        LastPlayerPostion = new Vector3(0,0,0);
        anim.SetBool("PlayerSptted", false);
        anim.SetBool("ChasePlayer", false);
        anim.SetBool("Patroling", true);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //PlayerSighted(other.transform);
            AttackPlayer();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            moveToPlayer();
        }
    }
}
