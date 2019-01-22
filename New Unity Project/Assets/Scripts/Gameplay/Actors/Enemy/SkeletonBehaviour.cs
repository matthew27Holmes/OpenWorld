using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonBehaviour : MonoBehaviour {

    // Use this for initialization
    public List<Vector3> PatrolRoute = new List<Vector3>();
    public int patrolPoint;
    public Vector3 LastPlayerPostion;
    public bool playerSpotted;

    //two bools fo player in sightand spotteed 
    //means you can set enemy up to move to players last know postion allows flanking 
    bool playerInSight;
    bool playerInRange;
    public bool Attacking;
   // public Transform Spawner = null;
    NavMeshAgent navMeshAgent;
    Animator anim;
    public int damage;
    int health;
    bool alive;

    public int BirthNodeID = 0;
    public int NodeID = 0;
    
	void Start () {

        playerSpotted = false;
        playerInSight = false;
        playerInRange = false;
        Attacking = false;
        LastPlayerPostion = new Vector3(0,0,0);
        navMeshAgent = GetComponent<NavMeshAgent>();
        patrolPoint = 0;

        anim = GetComponent<Animator>();

        damage = 5;
        health = 100;
        alive = true;
	}
	
	// Update is called once per frame
	void Update () {

        if (alive)
        {
            if (playerSpotted)
            {
                if (playerInRange)
                {
                    AttackPlayer();
                }
                else
                {
                    moveToPlayer();
                }
            }
            else
            {
               // GameObject parentSpwner = GameObject.Find(Spawner.gameObject.name);
                anim.SetBool("Patroling", true);
                anim.SetBool("ChasePlayer", false);
                anim.SetBool("AttackPlayer", false);
                Attacking = false;


              //  if (parentSpwner != null)
               // {
                    Patrol();
              //  }
              //  else
              //  {
                    //return home and wait
                 //   navMeshAgent.SetDestination(Spawner.position);
               // }
            }
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Patrol()
    {
        navMeshAgent.speed = 2;
        navMeshAgent.SetDestination(PatrolRoute[patrolPoint]);
        if(Vector3.Distance(this.transform.position , PatrolRoute[patrolPoint]) <= 3.0f)//this is hacking
        {
            if (PatrolRoute.Count > 1)
            {
                patrolPoint++;
                if (patrolPoint >= PatrolRoute.Count)
                {
                    patrolPoint = 0;
                }
            }
            else
            {
                anim.SetBool("Ide", true);
                anim.SetBool("Patroling", false);
            }
        }
    }

    void moveToPlayer()
    {

        navMeshAgent.speed = 5;
        navMeshAgent.SetDestination(LastPlayerPostion);

        anim.SetBool("Ide", false);
        anim.SetBool("Patroling", false);
        anim.SetBool("AttackPlayer", false);
        Attacking = false;
        anim.SetBool("ChasePlayer", true);
    }
    void AttackPlayer()
    { 
        navMeshAgent.SetDestination(transform.position);
        anim.SetBool("AttackPlayer", true);
        Attacking = true;
    }
    
    public void TakeDamge(int Damage,bool kicked)
    {
        health =- Damage;
        anim.SetBool("Hit", true);
        anim.SetBool("Kicked", kicked);
    }

    void Die()
    {
        if(health <= 0 )
        {
            anim.SetInteger("Health", health);
            alive = false;
        }
    }


    public void PlayerSighted(Transform PlayerPostion)
    {
        anim.SetBool("PlayerSpotted", true);
        playerInSight = true;
        playerSpotted = true;
        LastPlayerPostion = PlayerPostion.position;
    }


    public void LostPlayer()
    {
        LastPlayerPostion = new Vector3(0,0,0);
        playerInSight = false;
        playerSpotted = false;
        anim.SetBool("PlayerSpotted", false);
        anim.SetBool("AttackPlayer", false);
        anim.SetBool("ChasePlayer", false);
        anim.SetBool("Ide", false);
        anim.SetBool("Patroling", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Cell")
        {
             int.TryParse(other.name, out NodeID);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInRange = false;
        }
    }
}
