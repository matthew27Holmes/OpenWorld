using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonBehaviour : MonoBehaviour {

    // Use this for initialization
    public List<Vector3> PatrolRoute = new List<Vector3>();
    public int patrolPoint;
    List<Vector3> TempPatrolPoints;
    public Vector3 LastPlayerPostion;
    public bool playerSpotted;

    //two bools fo player in sightand spotteed 
    //means you can set enemy up to move to players last know postion allows flanking 
   // bool playerInSight;
    public bool Attacking;
   // public Transform Spawner = null;
    NavMeshAgent navMeshAgent;
    Animator anim;
    public int damage;
    public int health;
    public bool alive;

    bool GenerateTempPatrol;

    public int BirthNodeID = 0;
    public int NodeID = 0;
    public string HashID;
    WorldGeneration GM;

    void Start () {
        gameObject.GetComponent<NavMeshAgent>().Warp(transform.position);
        playerSpotted = false;
        //playerInSight = false;
        Attacking = false;
        GenerateTempPatrol = false;

        LastPlayerPostion = new Vector3(0,0,0);
        navMeshAgent = GetComponent<NavMeshAgent>();
        patrolPoint = 0;

        anim = GetComponent<Animator>();

        damage = 5;
        health = 100;
        alive = true;

        GM = GameObject.FindGameObjectWithTag("GameManger").GetComponent<WorldGeneration>();
    }

    // Update is called once per frame
    void Update () {

        if (alive)
        {
            if (playerSpotted)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (Vector3.Distance(player.transform.position,transform.position) <= 3)
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


                if (GM.isCellLoaded(BirthNodeID))
                {
                    if (GenerateTempPatrol)
                    {
                        GenerateTempPatrol = false;
                        patrolPoint = 0;
                    }

                    Patrol(PatrolRoute);
                }
                else
                {
                    if (!GenerateTempPatrol)
                    {
                        TempPatrol();// need to test this 
                        GenerateTempPatrol = true;
                        patrolPoint = 0;
                    }
                    Patrol(TempPatrolPoints);
                }
            }
        }
        else
        {
            WorldGeneration GM = GameObject.FindGameObjectWithTag("GameManger").GetComponent<WorldGeneration>();
            GM.ActiveEnemies.Remove(this.gameObject);
            Destroy(gameObject);
        }
    }

    void Patrol(List<Vector3> Patrol)
    {
        navMeshAgent.speed = 2;
        navMeshAgent.SetDestination(Patrol[patrolPoint]);
        if(Vector3.Distance(this.transform.position , Patrol[patrolPoint]) <= 3.0f)//this is hacking
        {
            if (Patrol.Count > 1)
            {
                patrolPoint++;
                if (patrolPoint >= Patrol.Count)
                {
                    patrolPoint = 0;
                }
            }
            else
            {
                anim.SetBool("Idel", true);
                anim.SetBool("Patroling", false);
            }
        }
    }

    // handle patrol points being deleted 
    void TempPatrol()
    {
        TempPatrolPoints = new List<Vector3>();
        int PatrolPointNum = Random.Range(2, 5);
        Transform cell = GM.getCellCube(NodeID).transform;

        for (int i = 0; i < PatrolPointNum; i++)
        {
            float randW = Random.Range(cell.localScale.x / 2, cell.localScale.x / 2 * -1);
            float randD = Random.Range(cell.localScale.z / 2, cell.localScale.z / 2 * -1);

            Vector3 randomDirection = new Vector3(randW, transform.position.y, randD);

            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 1, 9);
            TempPatrolPoints.Add(hit.position);
        } 
    }

    void moveToPlayer()
    {

        navMeshAgent.speed = 5;
        navMeshAgent.SetDestination(LastPlayerPostion);

        anim.SetBool("Idel", false);
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
        if (health <= 0)
        {
            anim.SetInteger("Health", health);
            alive = false;
            // remove self from world generations active Enemies List 
            GM.ActiveEnemies.Remove(this.gameObject);
        }
    }

    public void PlayerSighted(Transform PlayerPostion)
    {
        anim.SetBool("PlayerSpotted", true);
        //playerInSight = true;
        playerSpotted = true;
        LastPlayerPostion = PlayerPostion.position;
    }

    public void LostPlayer()
    {
        LastPlayerPostion = new Vector3(0,0,0);
        //playerInSight = false;
        playerSpotted = false;
        anim.SetBool("PlayerSpotted", false);
        anim.SetBool("AttackPlayer", false);
        anim.SetBool("ChasePlayer", false);
        anim.SetBool("Idel", false);
        anim.SetBool("Patroling", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Cell")
        {
             int.TryParse(other.name, out NodeID);
        }
    }
}
