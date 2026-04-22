using Photon.Pun;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private float damageAmount = 20f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Photon.Pun.PhotonView view) && view.IsMine)
        {
            var player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.photonView.RPC("TakeDamage", RpcTarget.All, damageAmount);
            }
        }
    }

    // Можно добавить OnTriggerStay с интервалами, если надо наносить урон по времени
}
