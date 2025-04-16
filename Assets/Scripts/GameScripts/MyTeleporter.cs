using System.Collections;
using UnityEngine;
using Photon.Pun;

public class MyTeleporter : MonoBehaviour
{
    public MyTeleporter TeleportTo; // Ссылка на целевой телепорт
    public bool isBeingTeleportedTo { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (!isBeingTeleportedTo)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            PhotonView pv = other.GetComponent<PhotonView>();

            if (player != null && pv != null && pv.IsMine)
            {
                Debug.Log($"Trigger entered by: {other.gameObject.name}, starting teleport");
                StartCoroutine(TeleportWithDelay(player, pv));
            }
            else
            {
                Debug.LogWarning($"No PlayerController or PhotonView found on {other.gameObject.name}, or not local player");
            }
        }
    }

    private IEnumerator TeleportWithDelay(PlayerController player, PhotonView pv)
    {
        // Ждем 2 секунды
        yield return new WaitForSeconds(2f);

        // Проверяем, что игрок все еще существует и не телепортирован
        if (player != null && !isBeingTeleportedTo)
        {
            // Вызываем RPC для телепортации на всех клиентов
            pv.RPC("TeleportPlayer", RpcTarget.All, TeleportTo.transform.position, TeleportTo.transform.rotation);
            Debug.Log($"Teleporting {player.gameObject.name} to {TeleportTo.transform.position}");

            TeleportTo.isBeingTeleportedTo = true;
        }

        // Сбрасываем флаги через короткое время
        yield return new WaitForSeconds(0.1f);
        isBeingTeleportedTo = false;
        TeleportTo.isBeingTeleportedTo = false;
    }
}