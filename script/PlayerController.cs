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
    
    float horizontalKeyInput = 0;

    float verticalKeyInput = 0;

    bool isTouch = false;

    Vector2 leftStartTouch = new Vector2();

    vector2 leftTouchInput = new vevtor2();

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
        if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0)
            {
                isTouch == true;
                Touch[] touches = Input.touches;
                foreach(var touch in touches)
                {
                    bool isLeftTouch = false;
                    bool isRightTouch = false;
                    if(touch.position.x > 0 && touch.position.x < Screen.width / 2)
                    {
                        isLeftTouch = true;
                    }
                    else if(touch.position.x > Screen.width / 2 && touch.position.x < Screen.width)
                    {
                        isRightTouch = true;
                    }

                    if(isLeftTouch == true)
                    {
                        if(touch.phase == touchphase.Began)
                        {
                            Debug.log("Left Touch Began");
                            leftStartTouch = touch.position;
                        }
                        else if(touch.phase == touchPhase.Moved || touch.phase == touchPhase.Stationary)
                        {
                            Debug.log("Left Touch Moved");
                            Vector2 position = touch.position;
                            leftTouchInput = position - leftStartTouch;
                        }
                        else if(touch.phase == touchPhase.Ended)
                        {
                            Debug.log("Left Touch Ended");
                            leftTouchInput = Vector2.zero;
                        }
                    }
                    if(isRightTouch == true)
                    {
                        cameraController.UpdateRightTouch(touch);
                    }
                }
            }
            else
            {
                isTouch = false;
            }
        horizontalKeyInput = Input.GetAxis("Horizontal");
        vertivalKeyInput = Input.GetAxis("vertical");

        bool isKeyInput = (horizontalKeyInput != 0 || verticalKeyInput != 0 leftTouchInput != Vector2.zero);
        if (isKeyInput == true && isAttack == false) {
            bool currentIsRun = animator.GetBool("isRun");
            if (currentIsRun == false) {
                animator.SetBool("isRun", true);
            }
            vevtor3 dir = rigid.velocity.normalized;
            dir.y = 0;
            this.transform.forward = dir;
            else
            {
                bool currentIsRun = animator.GetBool("isRun");
                if (currentIsRun == true) {
                    animator.SetBool("isRun", false);
                }
            }
        }

        Debug.log(horizontalKeyInput + "." + verticalKeyInput);
    }

    void FixedUpdate()
    {
        CameraController.FixedUpdateCameraPostion(this.transorm);

        if(isAttack == false)
        {
            Vector3 input = new Vector3();
            vector3 move = new Vector3();
            if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                input = new Vector3(leftTouchInput.x, 0, leftTouchInput.y);
                move = input.normalized * 2f;
            }
            else
            {
                input = new Vector3(horizontalKeyInput, 0, verticalKeyInput);
                move = input.normalized * 2f;
            }

            Vector3 cameraMove = Camara.main.gameObject.transform.rotation * move;
            cameraMove.y = 0;
            Vector3 currentRigidVelocity = rigid.velocity;
            currentRiggidVelocity.y = 0;

            rigid.AddForce(cameraMove - currentRigidVelocity, ForceMode.VelocityChange);
        }
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