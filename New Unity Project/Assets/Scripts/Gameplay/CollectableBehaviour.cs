using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableBehaviour : MonoBehaviour
{

    public float rotaionRate = 10.0f;
    float fademax, fadeMin;
    float fadeRate = 1.0f;
    bool fadeUP;
    // Use this for initialization
    public Light pointLight;
    void Start()
    {
        fademax = 5.0f;
        fadeMin = 1.0f;
        fadeUP = true;

    }
    void Rotate()
    {
        transform.Rotate(new Vector3(0, rotaionRate * Time.deltaTime, 0));
    }


    // Update is called once per frame
    void Update()
    {
        Rotate();
        fade();
        if (pointLight.intensity >= fademax)
        {
            fadeUP = false;
        }
        else if (pointLight.intensity <= fadeMin)
        {
            fadeUP = true;
        }
    }
    void fade()
    {
        if (fadeUP)
        {
            pointLight.intensity += fadeRate * Time.deltaTime;

        }
        else
        {
            pointLight.intensity -= fadeRate * Time.deltaTime;

        }
    }
}
