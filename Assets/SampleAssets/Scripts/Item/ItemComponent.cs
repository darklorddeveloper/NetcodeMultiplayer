
using UnityEngine;
using Unity.Netcode;
namespace Sample
{
    public class ItemComponent : NetworkBehaviour
    {
        public ItemType itemType = ItemType.SpeedBooster;
        public float effectPeriod = 5;
        [System.NonSerialized]public Transform spawnPoint;
    }
}