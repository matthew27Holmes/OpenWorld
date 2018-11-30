using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour {

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            transform.parent.gameObject.GetComponent<SkeletonBehaviour>().PlayerSighted(other.transform);
        }
    }
}
