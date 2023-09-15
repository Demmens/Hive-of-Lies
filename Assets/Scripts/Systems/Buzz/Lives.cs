using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Lives : NetworkBehaviour
{
    [SerializeField] IntVariable numLives;
    [SerializeField] GameObject lifePrefab;

    List<GameObject> lives = new();

    [Server]
    public override void OnStartServer()
    {
        numLives.AfterVariableChanged += UpdateLives;
    }

    [Client]
    public void CreateLives()
    {
        for (int i = 0; i < numLives.Value; i++)
        {
            GameObject life = Instantiate(lifePrefab);
            life.transform.SetParent(transform);
            lives.Add(life);
        }
    }

    [ClientRpc]
    void UpdateLives(int value)
    {
        int diff = value - lives.Count;

        //If our lives have gone up
        if (diff > 0)
        {
            for (int i = 0; i < diff; i++)
            {
                GameObject life = Instantiate(lifePrefab);
                life.transform.SetParent(transform);
                lives.Add(life);
            }
        }
        
        //If our lives have gone down
        if (diff < 0)
        {
            int curLives = lives.Count;
            for (int i = curLives - 1; i >= curLives + diff && i >= 0; i--)
            {
                GameObject life = lives[i];
                lives.Remove(life);
                Destroy(life);
            }
        }
    }
}
