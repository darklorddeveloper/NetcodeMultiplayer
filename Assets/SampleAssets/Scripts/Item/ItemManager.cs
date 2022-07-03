using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using StarterAssets;
namespace Sample
{
    public class ItemEffectQueue
    {
        public ItemType itemType;
        public ThirdPersonController thirdPersonController;
        public float effectPeriod;
    }

    public class ItemManager : NetworkBehaviour
    {
        [SerializeField] private ItemComponent[] itemPrefabs;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float spawnItemInterval = 3;
        private List<Transform> validSpawnPoints = new List<Transform>();
        private List<ItemEffectQueue> itemEffectQueues = new List<ItemEffectQueue>();
        private float spawnItemTimeCount = 0;
        private int spawnedItemNumbers = 0;
        public float SpeedBoosterRate = 3.5f;
        public float SuperJumperRate = 5;
        private void Awake()
        {
            enabled = false;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer == false)
            {
                return;
            }

            enabled = true;
            validSpawnPoints.AddRange(spawnPoints);
            GameEvent.Instance.onCharacterTriggeredOther += OnCharacterTriggeredOther;
        }


        public void Update()
        {
            UpdateItemEffect();
            UpdateSpawnItem();
        }

        private void UpdateItemEffect()
        {
            int count = itemEffectQueues.Count;
            float deltaTime = Time.deltaTime;
            for (int i = count - 1; i >= 0; i--)
            {
                itemEffectQueues[i].effectPeriod -= deltaTime;
                if (itemEffectQueues[i].effectPeriod > 0)
                {
                    continue;
                }
                RemoveEffectFromCharacter(itemEffectQueues[i].thirdPersonController, itemEffectQueues[i].itemType);
                itemEffectQueues.RemoveAt(i);
            }
        }

        private void UpdateSpawnItem()
        {
            spawnItemTimeCount += Time.deltaTime;
            if (spawnItemTimeCount < spawnItemInterval)
            {
                return;
            }
            spawnItemTimeCount = 0;
            SpawnItem();
        }

        public void SpawnItem()
        {
            if (validSpawnPoints.Count <= 0)
            {
                return;
            }
            int index = spawnedItemNumbers % itemPrefabs.Length;
            ItemComponent itemInstance = Instantiate<ItemComponent>(itemPrefabs[index]);
            int lastSpawnPointIndex = validSpawnPoints.Count - 1;
            itemInstance.transform.position = validSpawnPoints[lastSpawnPointIndex].position;
            itemInstance.spawnPoint = validSpawnPoints[lastSpawnPointIndex];
            validSpawnPoints.RemoveAt(lastSpawnPointIndex);

            NetworkObject networkObject = itemInstance.gameObject.GetComponent<NetworkObject>();
            networkObject.Spawn();
            spawnedItemNumbers++;
        }


        private void OnCharacterTriggeredOther(ThirdPersonController thirdPersonController, Collider other)
        {
            ItemComponent itemComponent = other.gameObject.GetComponent<ItemComponent>();
            if (itemComponent == null)
            {
                return;
            }

            if (itemComponent.IsSpawned == false)//prevent first frame triggers
            {
                return; //TODO queuing delay pickup
            }

            validSpawnPoints.Add(itemComponent.spawnPoint);
            AddEffectQueue(thirdPersonController, itemComponent);
            ApplyItemEffectToCharacter(thirdPersonController, itemComponent.itemType);

            //powerup
            itemComponent.GetComponent<NetworkObject>().Despawn();
            GameObject.Destroy(itemComponent.gameObject);
        }

        private void AddEffectQueue(ThirdPersonController thirdPersonController, ItemComponent itemComponent)
        {
            int count = itemEffectQueues.Count;
            for (int i = 0; i < count; i++)
            {
                if (itemEffectQueues[i].thirdPersonController == thirdPersonController && itemComponent.itemType == itemEffectQueues[i].itemType)
                {
                    itemEffectQueues[i].effectPeriod = Mathf.Max(itemComponent.effectPeriod, itemEffectQueues[i].effectPeriod);
                    return;
                }
            }
            ItemEffectQueue queue = new ItemEffectQueue();
            queue.thirdPersonController = thirdPersonController;
            queue.itemType = itemComponent.itemType;
            queue.effectPeriod = itemComponent.effectPeriod;
            itemEffectQueues.Add(queue);
        }

        private void ApplyItemEffectToCharacter(ThirdPersonController thirdPersonController, ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.SpeedBooster:
                    thirdPersonController.MoveSpeed = thirdPersonController.OriginalMoveSpeed * SpeedBoosterRate;
                    thirdPersonController.SprintSpeed = thirdPersonController.OriginalSprintSpeed * SpeedBoosterRate;
                    break;
                case ItemType.SuperJumper:
                    thirdPersonController.JumpHeight = thirdPersonController.OriginalJumpHeight * SuperJumperRate;
                    break;
            }
        }

        private void RemoveEffectFromCharacter(ThirdPersonController thirdPersonController, ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.SpeedBooster:
                    thirdPersonController.MoveSpeed = thirdPersonController.OriginalMoveSpeed;
                    thirdPersonController.SprintSpeed = thirdPersonController.OriginalSprintSpeed;
                    break;
                case ItemType.SuperJumper:
                    thirdPersonController.JumpHeight = thirdPersonController.OriginalJumpHeight;
                    break;
            }
        }
    }
}