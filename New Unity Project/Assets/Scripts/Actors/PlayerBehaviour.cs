using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

    float turnSpeed;
   public float moveSpeed;
    float jumpPower;

    public Transform floor;

    bool alive;
    public bool grounded;
	// Use this for initialization
	void Start () {
        turnSpeed = 150.0f;
        moveSpeed = 5.0f;
        jumpPower = 8.0f;

        alive = true;
        grounded = true;
	}
	
	// Update is called once per frame
	void Update () {

        if (alive)
        {
           Move();
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
        if (grounded)
        {
            if (Input.GetButton("Jump"))
            {
                float jumpFroce = jumpPower * Time.deltaTime;
                transform.Translate(0, jumpFroce, 0);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "floor")
        {
            grounded = true;
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
