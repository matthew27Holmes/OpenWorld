using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

    float turnSpeed;
   public float moveSpeed;
    float jumpPower;

    public Transform floor;

    bool alive;
    int health;
    int CollectableNum;
    public bool grounded;
    enum ChcaterStates {Idel,Dead,Punching,Kicking,Hit,Move};
    Animator animator;

    ChcaterStates PlayerState;
	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        turnSpeed = 150.0f;
        moveSpeed = 5.0f;
        jumpPower = 4.0f;

        alive = true;
        grounded = true;
	}

    // Update is called once per frame
    void Update() {

        if (alive)
        {
            if (Input.GetButton("Kick"))
            {
                animator.SetBool("AttackKickTrigger", true);
                animator.SetInteger("Action", 2);
                animator.SetBool("Moving", false);
                animator.SetInteger("Jumping", 0);
            }
            else if (Input.GetButton("Punch"))
            {
                animator.SetBool("AttackTrigger", true);
                animator.SetInteger("Action", 1);
                animator.SetBool("Moving", false);
                animator.SetInteger("Jumping", 0);
            }
            else if (grounded)
            {
                Move();
                if (Input.GetButton("Jump"))
                {
                    grounded = false;
                    float jumpFroce = jumpPower * Time.deltaTime;
                    transform.Translate(0, jumpFroce, 0);
                    animator.SetInteger("Jumping", 1);
                    animator.SetBool("JumpTrigger", true);
                    animator.SetBool("Moving", false);
                }
            }
        }
        else {
            animator.SetBool("Death1Trigger", true);
        }
    }

    void Move()
    {
        animator.SetBool("Moving", true);
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * turnSpeed;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        if(x >0)
        {
            animator.SetBool("TurnRightTrigger", true);
            animator.SetBool("TurnLeftTrigger", false);
        }
        else if(x <0)
        {
            animator.SetBool("TurnRightTrigger", false);
            animator.SetBool("TurnLeftTrigger", true);
        }
        else
        {
            animator.SetBool("TurnRightTrigger", false);
            animator.SetBool("TurnLeftTrigger", false);
        }
        animator.SetFloat("Velocity Z", z *100);
        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);
    }

    public void TakeDamge(int damage)
    {
        animator.SetBool("GetHitTrigger", true);
        health =- damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Collectable")
        {
            CollectableNum++;
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "sword")
        {
            SkeletonBehaviour Enemy = other.transform.parent.parent.gameObject.GetComponent<SkeletonBehaviour>();
            if (Enemy.Attacking)
            {
                TakeDamge(Enemy.damage);
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "floor")
        {
            grounded = true;
            animator.SetInteger("Jumping", 0);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "floor")
        {
            grounded = false;
        }
    }
}
