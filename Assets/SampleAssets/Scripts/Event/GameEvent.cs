using System;
using UnityEngine;
using StarterAssets;
public class GameEvent
{
    private static GameEvent instance = new GameEvent();
    public static GameEvent Instance
    {
        get
        {
            return instance;
        }
    }

    public Action<ThirdPersonController> onSpawnedThirdpersonCharacterOnServer;
    public Action<ThirdPersonController> onPlayerJoined;
    public Action<ThirdPersonController> onSpawnedOwnerThirdpersonCharacter;
    public Action<ThirdPersonController, Collider> onCharacterTriggeredOther;
}
