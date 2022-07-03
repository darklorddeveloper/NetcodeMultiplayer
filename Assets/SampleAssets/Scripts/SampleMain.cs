using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Cinemachine;
using StarterAssets;

namespace Sample
{
    public class SampleMain : MonoBehaviour
    {
        [SerializeField] private RoomUI roomUI;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private float spwanRandomRadius = 1;

        public void Start()
        {
            SetupUI();
            SetupGameEvent();
        }

        private void SetupUI()
        {
            roomUI.hostButton.onClick.AddListener(OnClickedHost);
            roomUI.clientButton.onClick.AddListener(OnClickedClient);
        }

        private void SetupGameEvent()
        {
            GameEvent.Instance.onSpawnedThirdpersonCharacterOnServer += OnSpawnedThridpersonCharacter;
            GameEvent.Instance.onSpawnedOwnerThirdpersonCharacter += OnSpawnedOwnerCharacter;
        }

        private void OnClickedHost()
        {
            if(NetworkManager.Singleton.StartHost() == false)
            {
                //Failed
                return;
            }
            CloseRoomUI();
        }

        private void OnClickedClient()
        {
            if(NetworkManager.Singleton.StartClient() == false)
            {
                //Failed
                return;
            }
            CloseRoomUI();
        }


        private void CloseRoomUI()
        {
            roomUI.gameObject.SetActive(false);
        }

        private void OnSpawnedThridpersonCharacter(ThirdPersonController thirdPersonController)
        {
            Transform targetSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Vector3 randomOffset = new Vector3(Random.Range(-spwanRandomRadius, spwanRandomRadius), 0, Random.Range(-spwanRandomRadius, spwanRandomRadius));

            Vector3 pos = targetSpawnPoint.position + randomOffset;

            thirdPersonController.transform.position = pos;
            thirdPersonController.transform.rotation = targetSpawnPoint.rotation;
        }

        private void OnSpawnedOwnerCharacter(ThirdPersonController thirdPersonController)
        {
            virtualCamera.Follow = thirdPersonController.CinemachineCameraTarget.transform;

            PlayerInput playerInput = thirdPersonController.GetComponent<PlayerInput>();
            playerInput.enabled = true;
        }
    }
}