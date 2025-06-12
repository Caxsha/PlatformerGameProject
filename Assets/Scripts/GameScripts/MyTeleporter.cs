using System.Collections;
using UnityEngine;
using Photon.Pun;

public class MyTeleporter : MonoBehaviour
{
    public MyTeleporter TeleportTo;
    private bool isTeleporting = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isTeleporting || TeleportTo == null)
            return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            StartCoroutine(TeleportRoutine(other.transform, pv));
        }
    }

    private IEnumerator TeleportRoutine(Transform player, PhotonView pv)
    {
        isTeleporting = true;
        TeleportTo.isTeleporting = true;

        yield return new WaitForSeconds(0.05f);

        pv.RPC("TeleportPlayer", RpcTarget.All, TeleportTo.transform.position, TeleportTo.transform.rotation);

        yield return new WaitForSeconds(1f);
        isTeleporting = false;
        TeleportTo.isTeleporting = false;
    }
}
