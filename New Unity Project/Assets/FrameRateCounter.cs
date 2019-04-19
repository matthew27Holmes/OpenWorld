using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameRateCounter : MonoBehaviour {

    // Use this for initialization
    Text text;
	void Start () {
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        text.text = ((int)(1.0f / Time.smoothDeltaTime)).ToString();
    }
}
