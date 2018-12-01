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
                animator.SetFloat("Action", 2);
            }
            else if (Input.GetButton("Punch"))
            {
                animator.SetBool("AttackKickTrigger", true);
                animator.SetFloat("Action", 1);
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

        animator.SetFloat("Velocity Z", z *100);
        animator.SetFloat("Velocity X", x * 100);
        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);
    }

    void PlayerHit()
    {
        animator.SetBool("GetHitTrigger", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Collectable")
        {
            CollectableNum++;
            other.gameObject.SetActive(false);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "floor")
        {
            grounded = true;
            animator.SetInteger("Jumping", 0);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "floor")
        {
            grounded = false;
        }
    }
}
