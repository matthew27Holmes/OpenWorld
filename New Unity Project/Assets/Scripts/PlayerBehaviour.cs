using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

    float turnSpeed;
    float moveSpeed;
    float jumpPower;
    float gravity;

    public Transform floor;

    bool alive;
    CharacterController controller;
	// Use this for initialization
	void Start () {
        turnSpeed = 150.0f;
        moveSpeed = 3.0f;
        jumpPower = 8.0f;
        gravity = 1.0f;

        alive = true;
        controller = this.GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {

        if (alive)
        {
            if (checkGrounded())
            {
                Move();
            }
            else
            {
                // Apply gravity
                transform.Translate(0, -gravity * Time.deltaTime, 0);
                Mathf.Clamp(transform.position.y, 0,100);
            }
        }
    }

    void playerStateMachine()
    {

    }

    void Move()
    {
            var x = Input.GetAxis("Horizontal") * Time.deltaTime * turnSpeed;
            var z = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

            transform.Rotate(0, x, 0);
            transform.Translate(0, 0, z);

        // dont jump its dumb
            if (Input.GetButton("Jump"))
            {
                float jumpFroce = jumpPower;
            controller.transform.Translate(0, jumpFroce, 0);
            }
    }

    bool checkGrounded()
    {
        if (transform.position.y <= floor.position.y)
        {
            return true;
        }
        return false;
       
    }
}
