using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "Spawn Mission Effect Behaviour", menuName = "Missions/Effects/General/Spawn Mission Effect Behaviour")]
public class SpawnMissionEffectBehaviour : MissionEffect
{
    [SerializeField] GameObject missionEffectBehaviour;

    public override void TriggerEffect()
    {
        Debug.Log("Effect triggered");
        GameObject obj = Instantiate(missionEffectBehaviour);
        NetworkServer.Spawn(obj);
        obj.GetComponent<MissionEffectBehaviour>().OnEffectEnded += EndEffect;
    }
}