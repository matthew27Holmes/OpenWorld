using UnityEngine;
using System.Collections;

public class GolbalFlock : MonoBehaviour {

    // Use this for initialization
    public GameObject Fish;
    public static int tankSize=20;
    static int Fishnum = 10;
    public static GameObject[] allFish = new GameObject[Fishnum];
    public static Vector3 goalPos;
    void Start()
    {
        goalPos = new Vector3(transform.position.x,0, transform.position.z);
        for (int i = 0; i < Fishnum; i++)
        {
            Vector3 pos = new Vector3(transform.position.x + Random.Range(-tankSize, tankSize),0.0f, transform.position.z + Random.Range(-tankSize, tankSize));
            allFish[i] = (GameObject)Instantiate(Fish, pos, Quaternion.identity);
            allFish[i].transform.parent = this.transform;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
	//if (Random.Range(0,1000)<50)
 //       {
 //           //goalPos = new Vector3(goalObject.transform.position.x + Random.Range(-tankSize, tankSize), 0.0f, goalObject.transform.position.z + Random.Range(-tankSize, tankSize));
 //           goalPos = goalObject.transform.position;
 //       }
	}
}
