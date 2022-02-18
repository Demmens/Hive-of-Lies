using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandOrPassUI : MonoBehaviour
{
    [SerializeField] GameObject UI;

    private void Start()
    {
        //Listen for server sending information over
    }

    public void CreateUI()
    {
        Instantiate(UI);
    }

    public void ClickStand()
    {
        //Send stand info to server
        //Literally just need an empty send, and then on server side determine which Player class the sender is
        Destroy(UI);
    }

    public void ClickPass()
    {
        //Send pass info to server
        //Literally just need an empty send, and then on server side determine which Player class the sender is
        Destroy(UI);
    }
}
