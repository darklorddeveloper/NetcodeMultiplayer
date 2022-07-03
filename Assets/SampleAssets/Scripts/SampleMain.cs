using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Sample
{
    public class SampleMain : MonoBehaviour
    {
        [SerializeField] private RoomUI roomUI;

        public void Start()
        {
            SetupUI();
        }

        private void SetupUI()
        {
            roomUI.hostButton.onClick.AddListener(OnClickedHost);
            roomUI.clientButton.onClick.AddListener(OnClickedClient);
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
    }
}