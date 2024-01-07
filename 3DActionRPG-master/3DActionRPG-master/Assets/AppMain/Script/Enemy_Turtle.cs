using System.Collections;
using UnityEngine;

// ------------------------------------------------------------
/// <summary>
/// EnemyBase���p�������GTurtle.
/// </summary>
// ------------------------------------------------------------
public class Enemy_Turtle : EnemyBase
{
    [Header("Sub Param")]
    //! �������U���R���C�_�[�R�[��.
    [SerializeField] ColliderCallReceiver farAttackCall = null;
    //! �������U�����W�b�h�{�f�B.
    [SerializeField] Rigidbody farAttackRigid = null;

    //! �ߋ����t���O.
    bool isNear = true;
    //! �������U���R���[�`��.
    Coroutine farAttackCor = null;

    protected override void Start()
    {
        base.Start();

        farAttackCall.TriggerEnterEvent.AddListener(OnFarAttackEnter);
    }

    protected override void Update()
    {
        base.Update();

    }

    // ------------------------------------------------------------
    /// <summary>
    /// ���Ӄ��[�_�[�R���C�_�[�X�e�C�C�x���g�R�[��.
    /// </summary>
    /// <param name="other"> �ڋ߃R���C�_�[. </param>
    // ------------------------------------------------------------
    protected override void OnAroundTriggerStay(Collider other)
    {
        base.OnAroundTriggerStay(other);

        if (other.gameObject.tag == "Player")
        {
            var sqrMag = (this.transform.position - other.gameObject.transform.position).sqrMagnitude;
            if (sqrMag > 3f) isNear = false;�@// �l�͓K�X�������Ă�������.
            else isNear = true;
        }
    }

    // ----------------------------------------------------------
    /// <summary>
    /// �U��Hit�A�j���[�V�����R�[��.
    /// </summary>
    // ----------------------------------------------------------
    protected override void Anim_AttackHit()
    {
        if (isNear == true) attackHitColliderCall.gameObject.SetActive(true);
        else
        {
            farAttackCall.gameObject.SetActive(true);
            var dir = (currentAttackTarget.position - this.transform.position);
            dir.y = 0;
            farAttackRigid.AddForce(dir * 100f); // 100�͓K�X�����A�K�v�Ȃ�ϐ��ɂ��܂��傤�B

            if (farAttackCor != null)
            {
                StopCoroutine(farAttackCor);
                farAttackCor = null;
                ResetFarAttack();
            }
            farAttackCor = StartCoroutine(AutoErase());
        }
    }

    // ----------------------------------------------------------
    /// <summary>
    /// �U���A�j���[�V�����I�����R�[��.
    /// </summary>
    // ----------------------------------------------------------
    protected override void Anim_AttackEnd()
    {
        base.Anim_AttackEnd();
    }

    // ------------------------------------------------------------
    /// <summary>
    /// �������U���R���C�_�[�G���^�[�R�[��.
    /// </summary>
    /// <param name="col"> �ڐG�R���C�_�[. </param>
    // ------------------------------------------------------------
    void OnFarAttackEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (farAttackCor != null)
            {
                StopCoroutine(farAttackCor);
                farAttackCor = null;
                ResetFarAttack();
            }

            var player = col.GetComponent<PlayerController>();
            player?.OnEnemyAttackHit(CurrentStatus.Power, this.transform.position);
            attackHitColliderCall.gameObject.SetActive(false);
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// ���Ԍo�߂Ŏ����폜.
    /// </summary>
    // ------------------------------------------------------------
    IEnumerator AutoErase()
    {
        yield return new WaitForSeconds(1f);
        ResetFarAttack();
    }

    // ------------------------------------------------------------
    /// <summary>
    /// �������U�������Z�b�g.
    /// </summary>
    // ------------------------------------------------------------
    void ResetFarAttack()
    {
        farAttackCall.gameObject.SetActive(false);
        farAttackRigid.velocity = Vector3.zero;
        farAttackRigid.angularVelocity = Vector3.zero;
        var reset = Vector3.zero;
        reset.y = 0.4f;
        reset.z = 1f;
        farAttackCall.gameObject.transform.localPosition = reset;
    }
}