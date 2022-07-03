using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
namespace Sample
{
    [RequireComponent(typeof(ThirdPersonController))]
    public class CharacterTriggerComponent : MonoBehaviour
    {
        private ThirdPersonController thirdPersonController;

        private void Awake()
        {
            thirdPersonController = GetComponent<ThirdPersonController>();
        }

        public void OnTriggerEnter(Collider other)
        {
            if(thirdPersonController == null)
            {
                return;
            }
            GameEvent.Instance.onCharacterTriggeredOther?.Invoke(thirdPersonController, other);
        }
    }
}