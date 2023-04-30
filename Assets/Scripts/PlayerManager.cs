using UnityEngine;
using UnityEngine.EventSystems;

using Photon.Pun;

using System.Collections;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        
        if (other.name.Contains("Kill"))
        {
            GameManager.Instance.LeaveRoom();
        }
    }
}
