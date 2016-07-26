using UnityEngine;
using System.Collections;

public class MessageSender : MonoBehaviour {

    public GameObject target;
    public string message;

    void OnClick()
    {
        target.SendMessage(message, SendMessageOptions.DontRequireReceiver);
    }
}
