using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using StarterAssets;
[RequireComponent(typeof(ThirdPersonController))]
public class OnSpawnCharacterEventComponent : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if(IsOwner == false)
        {
            return;
        }
        ThirdPersonController thirdPersonController = GetComponent<ThirdPersonController>();
        if(IsServer)
        {
            GameEvent.Instance.onSpawnedThirdpersonCharacterOnServer?.Invoke(thirdPersonController);
        }

        if (IsClient && IsOwner)
        {
            GameEvent.Instance.onSpawnedOwnerThirdpersonCharacter?.Invoke(thirdPersonController);
        }
    }
}
