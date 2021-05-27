using System.Collections.Generic;
using System.Xml.Serialization;

namespace DataStore.Collectibles
{
    public class CollectibleItemData
    {
        [XmlElement] public int ID;
        [XmlElement] public string Name;
        [XmlElement] public string Description;
        [XmlElement] public string PathSpriteFull;
        [XmlElement] public string PathSpriteMini;
        
        [XmlArray("Types")]
        [XmlArrayItem("Type")]
        public List<string> Types;
    }
}

