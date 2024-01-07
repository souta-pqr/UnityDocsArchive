using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    // ----------------------------------------------------------
    /// <summary>
    /// �X�e�[�^�X.
    /// </summary>
    // ----------------------------------------------------------
    [System.Serializable]
    public class Status
    {
        // HP.
        public int Hp = 10;
        // �U����.
        public int Power = 1;
    }

    // ��{�X�e�[�^�X.
    [SerializeField] Status DefaultStatus = new Status();
    // ���݂̃X�e�[�^�X.
    public Status CurrentStatus = new Status();

    [SerializeField] public bool IsBoss = false;

    // �A�j���[�^�[.
    Animator animator = null;

    // ���Ӄ��[�_�[�R���C�_�[�R�[��.
    [SerializeField] ColliderCallReceiver aroundColliderCall = null;

    //! ���g�̃R���C�_�[.
    [SerializeField] Collider myCollider = null;
    //! �U���q�b�g���G�t�F�N�g�v���n�u.
    [SerializeField] GameObject hitParticlePrefab = null;

    // �U���Ԋu.
    [SerializeField] float attackInterval = 3f;

    // �U����ԃt���O.
    public bool IsBattle = false;
    // �U�����Ԍv���p.
    float attackTimer = 0f;

    //! �U������p�R���C�_�[�R�[��.
    [SerializeField] protected ColliderCallReceiver attackHitColliderCall = null;
    // ���݂̍U���^�[�Q�b�g.
    protected Transform currentAttackTarget = null;

    // �J�n���ʒu.
    Vector3 startPosition = new Vector3();
    // �J�n���p�x.
    Quaternion startRotation = new Quaternion();

    //! HP�o�[�̃X���C�_�[.
    [SerializeField] Slider hpBar = null;

    // �G�̈ړ��C�x���g��`�N���X.
    public class EnemyMoveEvent : UnityEvent<EnemyBase> { }
    // �ړI�n�ݒ�C�x���g.
    public EnemyMoveEvent ArrivalEvent = new EnemyMoveEvent();

    // �i�r���b�V��.
    NavMeshAgent navMeshAgent = null;

    // ���ݐݒ肳��Ă���ړI�n.
    Transform currentTarget = null;

    // ���S���C�x���g.
    public EnemyMoveEvent DestroyEvent = new EnemyMoveEvent();

    protected virtual void Start()
    {
        // Animator���擾���ۊ�.
        animator = GetComponent<Animator>();

        // �ŏ��Ɍ��݂̃X�e�[�^�X����{�X�e�[�^�X�Ƃ��Đݒ�.
        CurrentStatus.Hp = DefaultStatus.Hp;
        CurrentStatus.Power = DefaultStatus.Power;

        // ���ӃR���C�_�[�C�x���g�o�^.
        aroundColliderCall.TriggerEnterEvent.AddListener(OnAroundTriggerEnter);
        aroundColliderCall.TriggerStayEvent.AddListener(OnAroundTriggerStay);
        aroundColliderCall.TriggerExitEvent.AddListener(OnAroundTriggerExit);

        // �U���R���C�_�[�C�x���g�o�^.
        attackHitColliderCall.TriggerEnterEvent.AddListener(OnAttackTriggerEnter);

        attackHitColliderCall.gameObject.SetActive(false);

        // �J�n���̈ʒu��]��ۊ�.
        startPosition = this.transform.position;
        startRotation = this.transform.rotation;

        // �X���C�_�[��������.
        hpBar.maxValue = DefaultStatus.Hp;
        hpBar.value = CurrentStatus.Hp;
    }

    protected virtual void Update()
    {
        // �U���ł����Ԃ̎�.
        if (IsBattle == true)
        {
            attackTimer += Time.deltaTime;

            animator.SetBool("isRun", false);

            if (attackTimer >= 3f)
            {
                animator.SetTrigger("isAttack");
                attackTimer = 0;
            }
        }
        else
        {
            attackTimer = 0;

            if (currentTarget == null)
            {
                animator.SetBool("isRun", false);

                ArrivalEvent?.Invoke(this);
                Debug.Log(gameObject.name + "�ړ��J�n.");
            }
            else
            {
                animator.SetBool("isRun", true);

                var sqrDistance = (currentTarget.position - this.transform.position).sqrMagnitude;
                if (sqrDistance < 3f)
                {
                    ArrivalEvent?.Invoke(this);
                }
            }
        }

    }

    // ----------------------------------------------------------
    /// <summary>
    /// �i�r���b�V���̎��̖ړI�n��ݒ�.
    /// </summary>
    /// <param name="target"> �ړI�n�g�����X�t�H�[��. </param>
    // ----------------------------------------------------------
    public void SetNextTarget(Transform target)
    {
        if (target == null) return;
        if (navMeshAgent == null) navMeshAgent = GetComponent<NavMeshAgent>();

        navMeshAgent.SetDestination(target.position);
        Debug.Log(gameObject.name + "�^�[�Q�b�g�ֈړ�." + target.gameObject.name);
        currentTarget = target;
    }

    // ----------------------------------------------------------
    /// <summary>
    /// �U���q�b�g���R�[��.
    /// </summary>
    /// <param name="damage"> �H������_���[�W. </param>
    // ----------------------------------------------------------
    public void OnAttackHit(int damage, Vector3 attackPosition)
    {
        CurrentStatus.Hp -= damage;
        hpBar.value = CurrentStatus.Hp;
        Debug.Log("Hit Damage " + damage + "/CurrentHp = " + CurrentStatus.Hp);

        var pos = myCollider.ClosestPoint(attackPosition);
        var obj = Instantiate(hitParticlePrefab, pos, Quaternion.identity);
        var par = obj.GetComponent<ParticleSystem>();
        StartCoroutine(WaitDestroy(par));

        if (CurrentStatus.Hp <= 0)
        {
            OnDie();
        }
        else
        {
            animator.SetTrigger("isHit");
        }
    }

    // ---------------------------------------------------------------------
    /// <summary>
    /// �p�[�e�B�N�����I��������j������.
    /// </summary>
    /// <param name="particle"></param>
    // ---------------------------------------------------------------------
    IEnumerator WaitDestroy(ParticleSystem particle)
    {
        yield return new WaitUntil(() => particle.isPlaying == false);
        Destroy(particle.gameObject);
    }

    // ----------------------------------------------------------
    /// <summary>
    /// ���S���R�[��.
    /// </summary>
    // ----------------------------------------------------------
    void OnDie()
    {
        Debug.Log("���S");
        animator.SetBool("isDie", true);
    }

    // ----------------------------------------------------------
    /// <summary>
    /// ���S�A�j���[�V�����I�����R�[��.
    /// </summary>
    // ----------------------------------------------------------
    void Anim_DieEnd()
    {
        // Destroy( gameObject );   // �m�F�̂��߃R�����g�A�E�g���Ă��܂����폜����OK�ł��B
        DestroyEvent?.Invoke(this);
    }
    // ------------------------------------------------------------
    /// <summary>
    /// ���Ӄ��[�_�[�R���C�_�[�G���^�[�C�x���g�R�[��.
    /// </summary>
    /// <param name="other"> �ڋ߃R���C�_�[. </param>
    // ------------------------------------------------------------
    void OnAroundTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            IsBattle = true;

            navMeshAgent.SetDestination(this.transform.position);
            currentTarget = null;
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// ���Ӄ��[�_�[�R���C�_�[�X�e�C�C�x���g�R�[��.
    /// </summary>
    /// <param name="other"> �ڋ߃R���C�_�[. </param>
    // ------------------------------------------------------------
    protected virtual void OnAroundTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            
            var _dir = (other.gameObject.transform.position - this.transform.position).normalized;
            _dir.y = this.transform.position.y;
            this.transform.forward = _dir;

            currentAttackTarget = other.gameObject.transform;
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// ���Ӄ��[�_�[�R���C�_�[�I���C�x���g�R�[��.
    /// </summary>
    /// <param name="other"> �ڋ߃R���C�_�[. </param>
    // ------------------------------------------------------------
    void OnAroundTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            IsBattle = false;

            currentAttackTarget = null;
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// �U���R���C�_�[�G���^�[�C�x���g�R�[��.
    /// </summary>
    /// <param name="other"> �ڋ߃R���C�_�[. </param>
    // ------------------------------------------------------------
    void OnAttackTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            var player = other.GetComponent<PlayerController>();
            player?.OnEnemyAttackHit(CurrentStatus.Power, this.transform.position);
            attackHitColliderCall.gameObject.SetActive(false);
        }
    }

    // ----------------------------------------------------------
    /// <summary>
    /// �U��Hit�A�j���[�V�����R�[��.
    /// </summary>
    // ----------------------------------------------------------
    protected virtual void Anim_AttackHit()
    {
        attackHitColliderCall.gameObject.SetActive(true);
    }

    // ----------------------------------------------------------
    /// <summary>
    /// �U���A�j���[�V�����I�����R�[��.
    /// </summary>
    // ----------------------------------------------------------
    protected virtual void Anim_AttackEnd()
    {
        attackHitColliderCall.gameObject.SetActive(false);
    }

    // ----------------------------------------------------------
    /// <summary>
    /// �v���C���[���g���C���̏���.
    /// </summary>
    // ----------------------------------------------------------
    public void OnRetry()
    {
        // ���݂̃X�e�[�^�X����{�X�e�[�^�X�Ƃ��Đݒ�.
        CurrentStatus.Hp = DefaultStatus.Hp;
        CurrentStatus.Power = DefaultStatus.Power;
        hpBar.value = CurrentStatus.Hp;

        // �J�n���̈ʒu��]��ۊ�.
        this.transform.position = startPosition;
        this.transform.rotation = startRotation;

        this.gameObject.SetActive(true);

    }



}