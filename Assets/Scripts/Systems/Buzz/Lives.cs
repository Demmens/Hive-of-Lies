using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Lives : NetworkBehaviour
{
    [SerializeField] IntVariable numLives;
    [SerializeField] GameObject lifePrefab;

    [SerializeField] Color lostLifeColor;

    List<GameObject> lives = new();

    public override void OnStartServer()
    {
        numLives.AfterVariableChanged += UpdateLives;
    }

    [ClientRpc]
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
        for (int i = 0; i < lives.Count - value; i++)
        {
            lives[i].GetComponent<UnityEngine.UI.Image>().color = lostLifeColor;
        }
    }
}
