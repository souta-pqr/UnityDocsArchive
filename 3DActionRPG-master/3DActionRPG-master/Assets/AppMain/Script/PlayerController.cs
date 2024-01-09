using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // -------------------------------------------------------
    /// <summary>
    /// �X�e�[�^�X.
    /// </summary>
    // -------------------------------------------------------
    [System.Serializable]
    public class Status
    {
        // �̗�.
        public int Hp = 10;
        // �U����.
        public int Power = 1;
    }

    // �U��Hit�I�u�W�F�N�g��ColliderCall.
    [SerializeField] ColliderCallReceiver attackHitCall = null;
    // ��{�X�e�[�^�X.
    [SerializeField] Status DefaultStatus = new Status();
    // ���݂̃X�e�[�^�X.
    public Status CurrentStatus = new Status();

    // �U������p�I�u�W�F�N�g.
    [SerializeField] GameObject attackHit = null;
    // �ݒu����pColliderCall.
    [SerializeField] ColliderCallReceiver footColliderCall = null;
    // �^�b�`�}�[�J�[.
    [SerializeField] GameObject touchMarker = null;
    // �W�����v��.
    [SerializeField] float jumpPower = 20f;

    // �J�����R���g���[���[.
    [SerializeField] PlayerCameraController cameraController = null;

    // ���g�̃R���C�_�[.
    [SerializeField] Collider myCollider = null;
    // �U����H������Ƃ��̃p�[�e�B�N���v���n�u.
    [SerializeField] GameObject hitParticlePrefab = null;
    // �p�[�e�B�N���I�u�W�F�N�g�ۊǗp���X�g.
    List<GameObject> particleObjectList = new List<GameObject>();

    //! HP�o�[�̃X���C�_�[.
    [SerializeField] Slider hpBar = null;

    //! �Q�[���I�[�o�[���C�x���g.
    public UnityEvent GameOverEvent = new UnityEvent();
    // �J�n���ʒu.
    Vector3 startPosition = new Vector3();
    // �J�n���p�x.
    Quaternion startRotation = new Quaternion();

    // �A�j���[�^�[.
    Animator animator = null;
    // ���W�b�h�{�f�B.
    Rigidbody rigid = null;
    //! �U���A�j���[�V�������t���O.
    bool isAttack = false;
    // �ڒn�t���O.
    bool isGround = false;

    // PC�L�[����������.
    float horizontalKeyInput = 0;
    // PC�L�[�c��������.
    float verticalKeyInput = 0;

    bool isTouch = false;

    // �������^�b�`�X�^�[�g�ʒu.
    Vector2 leftStartTouch = new Vector2();
    // �������^�b�`����.
    Vector2 leftTouchInput = new Vector2();

    // Start is called before the first frame update
    void Start()
    {
        // Animator���擾���ۊ�.
        animator = GetComponent<Animator>();
        // Rigidbody�̎擾.
        rigid = GetComponent<Rigidbody>();
        // �U������p�I�u�W�F�N�g���\����.
        attackHit.SetActive(false);

        // FootSphere�̃C�x���g�o�^.
        footColliderCall.TriggerStayEvent.AddListener(OnFootTriggerStay);
        footColliderCall.TriggerExitEvent.AddListener(OnFootTriggerExit);

        // �U������p�R���C�_�[�C�x���g�o�^.
        attackHitCall.TriggerEnterEvent.AddListener(OnAttackHitTriggerEnter);
        // ���݂̃X�e�[�^�X�̏�����.
        CurrentStatus.Hp = DefaultStatus.Hp;
        CurrentStatus.Power = DefaultStatus.Power;

        // �J�n���̈ʒu��]��ۊ�.
        startPosition = this.transform.position;
        startRotation = this.transform.rotation;

        // �X���C�_�[��������.
        hpBar.maxValue = DefaultStatus.Hp;
        hpBar.value = CurrentStatus.Hp;
    }

    // Update is called once per frame
    void Update()
    {
        // �J�������v���C���[�Ɍ�����. 
        cameraController.UpdateCameraLook(this.transform);

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            // �X�}�z�^�b�`����.
            // �^�b�`���Ă���w�̐����O��葽��.
            if(Input.touchCount > 0)
            {
                isTouch = true;
                // �^�b�`�������ׂĎ擾.
                Touch[] touches = Input.touches;
                // �S���̃^�b�`���J��Ԃ��Ĕ���.
                foreach (var touch in touches)
                {
                    bool isLeftTouch = false;
                    bool isRightTouch = false;
                    // �^�b�`�ʒu��X���������X�N���[���̍���.
                    if(touch.position.x > 0 && touch.position.x < Screen.width / 2)
                    {
                        isLeftTouch = true;

                    }
                    // �^�b�`�ʒu��X���������X�N���[���̉E��.
                    else if (touch.position.x > Screen.width / 2 && touch.position.x < Screen.width)
                    {
                        isRightTouch = true; ;
                    }

                    // ���^�b�`.
                    if (isLeftTouch == true)
                    {
                        // ���������^�b�`�����ۂ̏���.
                        // �^�b�`�J�n.
                        if (touch.phase == TouchPhase.Began)
                        {
                            Debug.log("タッチ開始");
                            // �J�n�ʒu��ۊ�.
                            leftStartTouch = touch.position;
                            // �J�n�ʒu�Ƀ}�[�J�[��\��.
                            touchMarker.SetActive(true);
                            Vector3 touchPosition = touch.position;
                            touchPosition.z = 1f;
                            Vector3 markerPosition = Camera.main.ScreenToWorldPoint(touchPosition);
                            touchMarker.transform.position = markerPosition;
                        }
                        // �^�b�`��.
                        else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                        {
                            Debug.log("タッチ中");
                            // ���݂̈ʒu�𐏎��
                            Vector2 position = touch.position;
                            // �ړ��p�̕�����ۊ�.
                            leftTouchInput = position - leftStartTouch;
                        }
                        // �^�b�`�I��.
                        else if (touch.phase == TouchPhase.Ended)
                        {
                            leftTouchInput = Vector2.zero;
                            // �}�[�J�[���\��.
                            touchMarker.gameObject.SetActive(false);
                        }
                    }

                    // �E�^�b�`.
                    if (isRightTouch == true)
                    {
                        // �E�������^�b�`�����ۂ̏���.
                        cameraController.UpdateRightTouch(touch);
                    }
                }

            }
            else
            {
                isTouch = false;
            }
        }
        else
        {
            // PC�L�[���͎擾.
            horizontalKeyInput = Input.GetAxis("Horizontal");
            verticalKeyInput = Input.GetAxis("Vertical");
        }

        // �v���C���[�̌����𒲐�.
        bool isKeyInput = (horizontalKeyInput != 0 || verticalKeyInput != 0 || leftTouchInput != Vector2.zero);
        if (isKeyInput == true && isAttack == false)
        {
            bool currentIsRun = animator.GetBool("isRun");
            if (currentIsRun == false) animator.SetBool("isRun", true);
            Vector3 dir = rigid.velocity.normalized;
            dir.y = 0;
            this.transform.forward = dir;
        }
        else
            {
                bool currentIsRun = animator.GetBool("isRun");
                if (currentIsRun == true) animator.SetBool("isRun", false);
            }
        }

    void FixedUpdate()
    {
        // �J�����̈ʒu���v���C���[�ɍ��킹��.
        cameraController.FixedUpdateCameraPosition(this.transform);

        if (isAttack == false)
        {
            Vector3 input = new Vector3();
            Vector3 move = new Vector3();
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                input = new Vector3(leftTouchInput.x, 0, leftTouchInput.y);
                move = input.normalized * 2f;
            }
            else
            {
                input = new Vector3(horizontalKeyInput, 0, verticalKeyInput);
                move = input.normalized * 2f;
            }
            Vector3 cameraMove = Camera.main.gameObject.transform.rotation * move;
            cameraMove.y = 0;
            Vector3 currentRigidVelocity = rigid.velocity;
            currentRigidVelocity.y = 0;

            rigid.AddForce(cameraMove - currentRigidVelocity, ForceMode.VelocityChange);
        }
    }

    // ---------------------------------------------------------------------
    /// <summary>
    /// �񕜏���.
    /// </summary>
    /// <param name="healPoint"> �񕜗�. </param>
    // ---------------------------------------------------------------------
    public void OnHeal(int healPoint)
    {
        CurrentStatus.Hp += healPoint;
        Debug.Log("HP��" + healPoint + "��!!");

        if (CurrentStatus.Hp > DefaultStatus.Hp) CurrentStatus.Hp = DefaultStatus.Hp;

        hpBar.value = CurrentStatus.Hp;
    }

    // ---------------------------------------------------------------------
    /// <summary>
    /// �U���{�^���N���b�N�R�[���o�b�N.
    /// </summary>
    // ---------------------------------------------------------------------
    public void OnAttackButtonClicked()
    {
        if (isAttack == false)
        {
            // Animation��isAttack�g���K�[���N��.
            animator.SetTrigger("isAttack");
            // �U���J�n.
            isAttack = true;
        }
    }

    // ---------------------------------------------------------------------
    /// <summary>
    /// �U���A�j���[�V����Hit�C�x���g�R�[��.
    /// </summary>
    // ---------------------------------------------------------------------
    void Anim_AttackHit()
    {
        Debug.Log("Hit");
        // �U������p�I�u�W�F�N�g��\��.
        attackHit.SetActive(true);
    }

    // ---------------------------------------------------------------------
    /// <summary>
    /// �U���A�j���[�V�����I���C�x���g�R�[��.
    /// </summary>
    // ---------------------------------------------------------------------
    void Anim_AttackEnd()
    {
        Debug.Log("End");
        // �U������p�I�u�W�F�N�g���\����.
        attackHit.SetActive(false);
        // �U���I��.
        isAttack = false;
    }

    // ---------------------------------------------------------------------
    /// <summary>
    /// �W�����v�{�^���N���b�N�R�[���o�b�N.
    /// </summary>
    // ---------------------------------------------------------------------
    public void OnJumpButtonClicked()
    {
        if (isGround == true)
        {
            Debug.Log("�W�����v");
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }

    // ---------------------------------------------------------------------
    /// <summary>
    /// FootSphere�g���K�[�X�e�C�R�[��.
    /// </summary>
    /// <param name="col"> �N�������R���C�_�[. </param>
    // ---------------------------------------------------------------------
    void OnFootTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Ground")
        {
            if (isGround == false) isGround = true;
            if (animator.GetBool("isGround") == false) animator.SetBool("isGround", true);
        }
    }

    // ---------------------------------------------------------------------
    /// <summary>
    /// FootSphere�g���K�[�C�O�W�b�g�R�[��.
    /// </summary>
    /// <param name="col"> �N�������R���C�_�[. </param>
    // ---------------------------------------------------------------------
    void OnFootTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Ground")
        {
            isGround = false;
            animator.SetBool("isGround", false);
        }
    }

    // ---------------------------------------------------------------------
    /// <summary>
    /// �U������g���K�[�G���^�[�C�x���g�R�[��.
    /// </summary>
    /// <param name="col"> �N�������R���C�_�[. </param>
    // ---------------------------------------------------------------------
    void OnAttackHitTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            var enemy = col.gameObject.GetComponent<EnemyBase>();
            enemy?.OnAttackHit(CurrentStatus.Power, this.transform.position);
            attackHit.SetActive(false);
        }
    }

// ---------------------------------------------------------------------
    /// <summary>
    /// �G�̍U�����q�b�g�����Ƃ��̏���.
    /// </summary>
    /// <param name="damage"> �H������_���[�W. </param>
    // ---------------------------------------------------------------------
    public void OnEnemyAttackHit( int damage, Vector3 attackPosition )
    {
        CurrentStatus.Hp -= damage;
        hpBar.value = CurrentStatus.Hp;

        var pos = myCollider.ClosestPoint( attackPosition );
        var obj = Instantiate( hitParticlePrefab, pos, Quaternion.identity );
        var par = obj.GetComponent<ParticleSystem>();
        StartCoroutine( WaitDestroy( par ) );
        particleObjectList.Add(obj);

        if ( CurrentStatus.Hp <= 0 )
        {
            OnDie();
        }
        else
        {
            Debug.Log( damage + "�̃_���[�W��H�����!!�c��HP" + CurrentStatus.Hp );
        }
    }
 
�@�@// ---------------------------------------------------------------------
    /// <summary>
    /// �p�[�e�B�N�����I��������j������.
    /// </summary>
    /// <param name="particle"></param>
    // ---------------------------------------------------------------------
    IEnumerator WaitDestroy( ParticleSystem particle )
    {
        yield return new WaitUntil( () => particle.isPlaying == false );
        if (particleObjectList.Contains(particle.gameObject) == true) particleObjectList.Remove(particle.gameObject);
        Destroy( particle.gameObject );
    }

    // ---------------------------------------------------------------------
    /// <summary>
    /// ���S������.
    /// </summary>
    // ---------------------------------------------------------------------
    void OnDie()
    {
        Debug.Log("���S���܂����B");
        StopAllCoroutines();
        if (particleObjectList.Count > 0)
        {
            foreach (var obj in particleObjectList) Destroy(obj);
            particleObjectList.Clear();
        }
        GameOverEvent?.Invoke();
    }

    // ---------------------------------------------------------------------
    /// <summary>
    /// ���g���C����.
    /// </summary>
    // ---------------------------------------------------------------------
    public void Retry()
    {
        // ���݂̃X�e�[�^�X�̏�����.
        CurrentStatus.Hp = DefaultStatus.Hp;
        CurrentStatus.Power = DefaultStatus.Power;
        hpBar.value = CurrentStatus.Hp;

        // �ʒu��]�������ʒu�ɖ߂�.
        this.transform.position = startPosition;
        this.transform.rotation = startRotation;

        //�U�������̓r���ł��ꂽ���p
        isAttack = false;

    }

}