using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class testScripy : MonoBehaviour {
    public Transform moveToPos;
    NavMeshAgent navie;
    // Use this for initialization
    void Start () {
         navie = gameObject.GetComponent<NavMeshAgent>();

    }

    // Update is called once per frame
    void Update () {
        navie.SetDestination(moveToPos.position);
	}
}
