using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "Spawn Mission Effect Behaviour", menuName = "Missions/Effects/Generic/Spawn Mission Effect Behaviour")]
public class SpawnMissionEffectBehaviour : MissionEffect
{
    [SerializeField] GameObject missionEffectBehaviour;

    public override void TriggerEffect()
    {
        GameObject obj = Instantiate(missionEffectBehaviour);
        obj.GetComponent<MissionEffectBehaviour>().OnEffectEnded += EndEffect;
        NetworkServer.Spawn(obj);
    }
}