using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DataStore.Collectibles
{
    public static class CollectiblesContainer
    {
        private static Dictionary<int, CollectibleItemData> _collectibles;
        
        private static Dictionary<int, CollectibleItemData> Collectibles
        {
            get
            {
                if (_collectibles == null)
                {
                    var store = CollectibleStore.ReadCollectibles();
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
                }
                return _collectibles;
            }
        }
        
        public static CollectibleItemData GetItem(int id)
        {
            return Collectibles[id];
        }
    }
}