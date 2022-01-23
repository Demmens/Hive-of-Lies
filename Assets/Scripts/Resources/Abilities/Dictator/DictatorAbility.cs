using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictatorAbility : RoleAbility, IEventListener
{
    void Start()
    {
        events.BindEvent(GameEvents.BeforeInfluenceResult, this);
    }

    public void ReceiveEvent(GameEvents key, IEventListener caller, EventParams param)
    {
        if (key == GameEvents.BeforeInfluenceResult)
        {

        }
    }
}
