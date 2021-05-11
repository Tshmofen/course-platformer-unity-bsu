using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using UnityEngine;

namespace DataStore.Collectibles
{
    public static class CollectiblesContainer
    {
        private const string CollectiblesPath = "Data/items";
        private static Dictionary<int, CollectibleItemData> _collectibles;
        
        public static CollectibleItemData GetItem(int id)
        {
            return GetCollectibles()[id];
        }

        private static Dictionary<int, CollectibleItemData> GetCollectibles()
        {
            if (_collectibles != null)
            {
                return _collectibles;
            }

            var store = GetCollectiblesFromXML();
            _collectibles = new Dictionary<int, CollectibleItemData>();
            
            foreach (var item in store.Items)
            {
                var regex = new Regex(@"[\s]{2,}");
                item.Description = regex
                    .Replace(item.Description, "")
                    .Replace("\\n", "\n")
                    .Trim();
            }

            foreach (var item in store.Items)
                _collectibles.Add(item.ID, item);

            return _collectibles;
        }
        
        private static CollectibleData GetCollectiblesFromXML()
        {
            var serializer = new XmlSerializer(typeof(CollectibleData));
            var xmlText = Resources.Load(CollectiblesPath) as TextAsset;
            if (xmlText == null) 
                throw new FileLoadException($"File can't be found: {CollectiblesPath}");
            var s = serializer.Deserialize(new StringReader(xmlText.text)) as CollectibleData;
            return s;
        }
    }
}