using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace DataStore.Collectibles
{
    [XmlRoot("Collectibles")]
    public class CollectibleStore
    {
        private const string CollectiblesPath = "Data/items";
        
        [XmlArray("Items")]
        [XmlArrayItem("Item")]
        public List<CollectibleItemData> Items;
        
        public static CollectibleStore ReadCollectibles()
        {
            var serializer = new XmlSerializer(typeof(CollectibleStore));
            var xmlText = Resources.Load(CollectiblesPath) as TextAsset;
            if (xmlText == null) 
                throw new FileLoadException($"File can't be found: {CollectiblesPath}");
            var s = serializer.Deserialize(new StringReader(xmlText.text)) as CollectibleStore;
            return s;
        }
    }
}