using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace DataStore.Collectibles
{
    [XmlRoot("Collectibles")]
    public class CollectibleData
    {
        [XmlArray("Items")]
        [XmlArrayItem("Item")]
        public List<CollectibleItemData> Items;
    }
}

