using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAnimatorManager : MonoBehaviourPun
{
    [SerializeField]
    private float directionDampTime = 0.25f;
    [SerializeField]
    private float movementSpeed = 3f;
    private Animator animator;

    private void Start() 
    {
        animator = GetComponent<Animator>();
        if (!animator)
        {
            Debug.LogError("No animator in PlayerAnimatorManager", this);
        }    
    }

    private void Update() 
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        if (!animator)
        {
            return;
        }



        //deal with Jumping
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // only allow jumping if we are running.
        if (stateInfo.IsName("Base Layer.Run"))
        {
            // When using trigger parameter
            if (Input.GetButtonDown("Fire2"))
            {
                animator.SetTrigger("Jump");
            }
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // if (transform.position.y < 1000)
        // {
        //     transform.Translate(new Vector3(h * movementSpeed * Time.deltaTime, transform.position.y, v * movementSpeed * Time.deltaTime));
        // }

        
        animator.SetFloat("Speed", h * h + v * v);    
        animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
    }
}
