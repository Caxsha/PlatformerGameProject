using UnityEngine;
using Photon.Pun;

public class FinishTrigger : MonoBehaviour
{
    private bool victoryDeclared = false;

    private void OnTriggerEnter(Collider other)
    {
        if (victoryDeclared)
            return;

        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            PhotonView pv = player.GetComponent<PhotonView>();

            if (pv != null && pv.IsMine)
            {
                victoryDeclared = true;
                string winnerName = pv.Owner.NickName;
                pv.RPC("DeclareVictory", RpcTarget.All, winnerName);
            }
        }
    }
}