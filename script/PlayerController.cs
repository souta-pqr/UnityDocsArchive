using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject attackHit = null;

    [SerializeField] ColliderCallReceiver footColliderCall = null;

    [SerializeField] floot jumpPower = 20f;

    Animator animator = null;

    Rigidbody rigid = null;

    bool isAttack = false;

    bool isGround = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        attackHit.SetActive(false);
        footColliderCall.TriggerStayEvent.AddListener((OnFootTriggerStay));
        footColliderCall.TriggerExitEvent.AddListener((OnFootTriggerExit));
    }

    void Update()
    {
    }

    public void OnAttackButtonClicked() {
        if(isAttack == false){
            animator.SetTrigger("isAttack");
            isAttack = true;
        }
    }

    public void OnJumpButtonClicked() 
    {
        if (isGround == true) {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }

    void OnFootTriggerStay(Collider col)
    {
        if(col.gameObject.tag == "ground") {
            if(isGround == false){
                isGround = true;
            }
        }
    }

    void OnFootTriggerExit(Collider col)
    {
        if(col.gameObject.tag == "Ground")
        {
            isGround = false;
            animator.SetBool("isGround", false);
        }
    }

    void Anim_AttackHit() {
        Debug.log("Hit");
        attackHit.SetActive(true);
    }

    void Anim_AttackEnd() {
        Debug.log("End");
        attackHit.SetActive(false);
        isAttack = false;
    }
}