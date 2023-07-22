using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class StingReticle : NetworkBehaviour
{
    [SerializeField] GameObject reticle;
    [SerializeField] List<GameObject> splines;
    [SerializeField] HoLPlayerDictionary playersByConnection;
    public override void OnStartAuthority()
    {
        Debug.Log("Authority started");
        StartCoroutine(StickToCursor());
    }

    IEnumerator StickToCursor()
    {
        while (true)
        {
            reticle.transform.position = new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 1);

            for (int i = 0; i < splines.Count; i++)
            {
                float t = (float) i / splines.Count;
                GetHeightOnPath(reticle.transform.position.x);
            }

            yield return null;
        }
    }

    float GetHeightOnPath(float t)
    {
        return 0.1f;
    }
}
