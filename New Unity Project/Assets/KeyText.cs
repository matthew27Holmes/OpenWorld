using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyText : MonoBehaviour {
    public Text winText;
    public PlayerBehaviour player;
    Text count;
	// Use this for initialization
	void Start () {
        count = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        count.text = "Keys : " + player.CollectableNum.ToString();
        if (player.CollectableNum >= 4)
        {
            winText.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
