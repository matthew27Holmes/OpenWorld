using UnityEngine;
using System.Collections;

public class PreyManger : MonoBehaviour
{

    public float speed = 5f;
    float rotationSpeed = 4.0f;
    Vector3 aveHeading;
    Vector3 avePostion;
    public int groupSize;
    public GameObject[] gos;
    float neighbourDis = 10.0f;
    bool turning = false;

    // Use this for initialization
    void Start()
    {
        speed = Random.Range(1f, 3f);
        gos = GolbalFlock.allFish;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.localPosition, Vector3.zero) >= GolbalFlock.tankSize)//||transform.position.y>0||transform.position.y<0
        {
            turning = true;
        }
        else
            turning = false;

        if (turning)
        {
            Vector3 direction = Vector3.zero - transform.localPosition;
            transform.localRotation = Quaternion.Slerp(transform.localRotation,
                                      Quaternion.LookRotation(direction),
                                      rotationSpeed * Time.deltaTime);
            speed = Random.Range(1f, 3f);
        }
        else
        {
            if (Random.Range(0, 5) < 1)
            {
                ApplyRules();
            }
        }
        transform.Translate(0, 0, Time.deltaTime * speed);
        transform.position = new Vector3(transform.position.x, 0.7f, transform.position.z);
        transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y, transform.eulerAngles.z);
       
    }
    void ApplyRules()
    {
        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.1f;

        Vector3 goalPos = GolbalFlock.goalPos;

        float dist;

        groupSize = 0;
        foreach (GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                dist = Vector3.Distance(go.transform.localPosition, this.transform.localPosition);
                if (dist <= neighbourDis)//is it less then max neigbour distances
                {
                    vcentre += go.transform.localPosition;// head towards neighbour
                    groupSize++;
                    if (dist < 1.0f)
                    {
                        vavoid = vavoid + (this.transform.localPosition - go.transform.localPosition);// move away form neighbour
                    }
                    PreyManger anotherFlock = go.GetComponent<PreyManger>();
                    gSpeed = gSpeed + anotherFlock.speed; // swim at average flock speed 
                }
            }
        }

        if (groupSize > 0)
        {
            vcentre = vcentre / groupSize + (goalPos - this.transform.localPosition);
            speed = gSpeed / groupSize;
            vavoid = vavoid / groupSize + (goalPos - this.transform.localPosition);

            Vector3 direction = (vcentre + vavoid) - transform.localPosition;
            if (direction != Vector3.zero)
            {

                transform.localRotation = Quaternion.Slerp(transform.localRotation,
                                     Quaternion.LookRotation(direction),
                                     rotationSpeed * Time.deltaTime);
            }
        }
    }
}
