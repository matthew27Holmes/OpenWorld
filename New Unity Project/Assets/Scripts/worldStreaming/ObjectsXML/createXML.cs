using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;



public class createXML : MonoBehaviour
{
    [XmlRoot("Streaming_GameObjects")]
    public class Node
    {
        [XmlArray("Assets")]
        [XmlArrayItem("Asset")]

        public List<StreamingAsset> assets = new List<StreamingAsset>();

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

        //Loads the xml directly from the given string. Useful in combination with www.text.
        public static Node LoadFromText(string text)
        {
            var serializer = new XmlSerializer(typeof(Node));
            return serializer.Deserialize(new StringReader(text)) as Node;
        }
    }

    public class StreamingAsset
    {
        [XmlAttribute("name")]
        public string Name;
        public Vector3 postion;
        public Vector3 Rotation;
        public Vector3 Scale;
        public bool IsActive;
    };

    public const string path = "Assets/Scripts/worldStreaming/ObjectsXML/XML/NODE_";
}
