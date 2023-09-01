using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class StingReticle : NetworkBehaviour
{
    [SerializeField] GameObject reticle;
    [SerializeField] List<GameObject> splines;
    public HivePlayer Owner;
    [SyncVar(hook =nameof(OnMouseMoved))] Vector3 ownerMousePos;
    [SyncVar] public Vector3 ownerButtonPos;

    [Client]
    public override void OnStartAuthority()
    {
        StartCoroutine(StickToCursor(ownerButtonPos));
        Cursor.visible = false;
    }

    [Client]
    public override void OnStopAuthority()
    {
        Cursor.visible = true;
    }

    [ClientRpc]
    public void SetActiveOnClients(bool active)
    {
        reticle.SetActive(active);
        foreach (GameObject spline in splines) spline.SetActive(active);
    }

    [Command]
    public void SetMousePosOnServer(Vector3 pos)
    {
        ownerMousePos = pos;
    }

    [Client]
    IEnumerator StickToCursor(Vector3 origin)
    {
        while (hasAuthority)
        {
            SetMousePosOnServer(Mouse.current.position.ReadValue());
            SetReticlePosition(Mouse.current.position.ReadValue());
            yield return null;
        }
    }

    [Client]
    void OnMouseMoved(Vector3 oldPos, Vector3 newPos)
    {
        SetReticlePosition(newPos);
    }

    [Client]
    void SetReticlePosition(Vector3 mousePos)
    {
        float mouseX = mousePos.x;
        float mouseY = mousePos.y;
        reticle.transform.position = new Vector3(mouseX, mouseY, 1);
        reticle.transform.Rotate(new Vector3(0, 0, 0.1f));

        for (int i = 0; i < splines.Count; i++)
        {
            float t = (i + 1f) / (splines.Count + 2f);

            float splineX = ownerButtonPos.x + t * Easing.GradualStart(1 - t) * (mouseX - ownerButtonPos.x);
            float splineY = ownerButtonPos.y + Easing.GradualEnd(t) * (mouseY - ownerButtonPos.y);

            splines[i].transform.position = new Vector3(splineX, splineY, ownerButtonPos.z);
        }
    }
}
