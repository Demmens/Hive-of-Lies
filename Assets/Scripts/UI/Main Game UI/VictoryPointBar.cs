using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class VictoryPointBar : NetworkBehaviour
{
    [SerializeField] Transform waspFactBar;
    [SerializeField] Transform honeyStolenBar;

    [SerializeField] Sprite segmentSprite;
    [SerializeField] Color waspFactSegmentColour;
    [SerializeField] Color honeyStolenSegmentColour;

    [Tooltip("How many wasp facts are needed for the Bees to win")]
    [SerializeField] IntVariable waspFactsNeeded;

    [Tooltip("How much honey stolen is needed for the Wasps to win")]
    [SerializeField] IntVariable honeyStolenNeeded;

    [SyncVar] int wfNeeded;
    [SyncVar] int hsNeeded;

    public override void OnStartServer()
    {
        //Set these now so the client can grab them immediately
        wfNeeded = waspFactsNeeded;
        hsNeeded = honeyStolenNeeded;
    }

    [ClientRpc]
    public override void OnStartClient()
    {
        CreateSegments(wfNeeded, waspFactBar, waspFactSegmentColour);
        CreateSegments(hsNeeded, honeyStolenBar, honeyStolenSegmentColour);
    }

    void CreateSegments(int num, Transform parent, Color colour)
    {
        //loop until i < num - 1 because we already have one segment placed by default
        for (int i = 0; i < num - 1; i++)
        {
            GameObject segment = new GameObject("Segment");
            Image image = segment.AddComponent<Image>();
            image.sprite = segmentSprite;
            image.color = colour;
            image.type = Image.Type.Sliced;

            segment.transform.SetParent(parent);
        }
    }
}
