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
    public class AssetContainer
    {
        [XmlArray("Nodes")]
        [XmlArrayItem("Node")]
        public List<Node> Nodes = new List<Node>();

        public void Save(string path)
        {
            var serializer = new XmlSerializer(typeof(AssetContainer));
            using (var stream = new FileStream(path, FileMode.Create))
            {
                serializer.Serialize(stream, this);
            }
        }

        public static AssetContainer Load(string path)
        {
            var serializer = new XmlSerializer(typeof(AssetContainer));
            using (var stream = new FileStream(path, FileMode.Open))
            {
                return serializer.Deserialize(stream) as AssetContainer;
            }
        }

        //Loads the xml directly from the given string. Useful in combination with www.text.
        public static AssetContainer LoadFromText(string text)
        {
            var serializer = new XmlSerializer(typeof(AssetContainer));
            return serializer.Deserialize(new StringReader(text)) as AssetContainer;
        }
    };

    public class Node
    {
        public string NodeID;
        [XmlArray("Assets")]
        [XmlArrayItem("Asset")]
        public List<StreamingAsset> assets = new List<StreamingAsset>();

    }
    public class StreamingAsset
    {
        [XmlAttribute("name")]
        public string Name;
        public Vector3 postion;
        public Vector3 Rotation;
        public Vector3 Scale;

    };

    public const string path = "Assets/WorldGeneration.XML";
}
