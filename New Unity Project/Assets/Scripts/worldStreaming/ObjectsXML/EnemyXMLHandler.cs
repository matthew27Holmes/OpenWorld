using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class EnemyXMLHandler : MonoBehaviour {

    [XmlRoot("Streaming_Enemies")]
    public class Node
    {
        [XmlArray("Assets")]
        [XmlArrayItem("Asset")]

        public List<EnemyAsset> Enemies = new List<EnemyAsset>();

        public void Save(string path)
        {
            var serializer = new XmlSerializer(typeof(Node));
            using (var stream = new FileStream(path, FileMode.Create))
            {
                serializer.Serialize(stream, this);
            }
        }

        public static Node Load(string path)
        {
            var serializer = new XmlSerializer(typeof(Node));
            using (var stream = new FileStream(path, FileMode.Open))
            {
                return serializer.Deserialize(stream) as Node;
            }
        }
    }

    public class EnemyAsset
    {
        [XmlAttribute("name")]
        public string Name;
        public Vector3 postion;
        public Vector3 Rotation;
        public Vector3 Scale;
        public List<PatrolPoint> patrol = new List<PatrolPoint>();
    };

    public class PatrolPoint
    {
        [XmlAttribute("PatrolPoint")]
        public Vector3 postion;
    }

    public const string path = "Assets/Scripts/worldStreaming/ObjectsXML/EnemyXML/NODE_";
}


