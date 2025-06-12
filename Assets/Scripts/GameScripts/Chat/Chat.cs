using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Chat : MonoBehaviour
{
    public TMP_InputField inputField;
    public GameObject Message;
    public GameObject Content;

    private List<GameObject> messageList = new List<GameObject>();
    private int maxMessages = 30;

    private bool isChatOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!isChatOpen)
            {
                isChatOpen = true;
                inputField.gameObject.SetActive(true);
                inputField.ActivateInputField();
                EventSystem.current.SetSelectedGameObject(inputField.gameObject);
            }
            else
            {
                TrySendMessage();
                isChatOpen = false;
                inputField.DeactivateInputField();
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }

    void TrySendMessage()
    {
        string text = inputField.text.Trim();
        if (string.IsNullOrEmpty(text)) return;

        GetComponent<PhotonView>().RPC("GetMessage", RpcTarget.All, PhotonNetwork.NickName + " : " + text);
        inputField.text = "";
    }

    [PunRPC]
    public void GetMessage(string ReceiveMessage)
    {
        GameObject M = Instantiate(Message, Vector3.zero, Quaternion.identity, Content.transform);
        M.GetComponent<Message>().MyMessage.text = ReceiveMessage;

        M.transform.SetSiblingIndex(0);

        messageList.Add(M);
        if (messageList.Count > maxMessages)
        {
            Destroy(messageList[messageList.Count - 1]);
            messageList.RemoveAt(messageList.Count - 1);
        }

        Canvas.ForceUpdateCanvases();
        Content.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 1f; // 1 Ч это вверх
    }

}
