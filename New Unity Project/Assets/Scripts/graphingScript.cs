using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class graphingScript : MonoBehaviour {
    float dataTime= 0.0f, dataTimer=0.05f,dataStoreTime= 0.0f;
    List<float> frameTimes = new List<float>();
    // Update is called once per frame
    void Update () {
		if(dataTime >= dataTimer)
        {
            FrameDatacollection();
            dataTime = 0;
        }
        dataTime += Time.deltaTime;
        dataStoreTime += Time.deltaTime;

        if(dataStoreTime>5)
        {
            string path = "Assets/Resources/FrameData.txt";
            StreamWriter writer = new StreamWriter(path, true);
            foreach(float data in frameTimes)
            {
                writer.WriteLine(data.ToString());
            }
            writer.Close();
            dataStoreTime = -10000;
        }
	}
    void FrameDatacollection()
    {
        frameTimes.Add(UnityStats.renderTime * 10000);
    }

}
