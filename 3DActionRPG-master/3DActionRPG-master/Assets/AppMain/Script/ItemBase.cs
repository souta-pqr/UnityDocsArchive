using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ColliderCallReceiver))]
// ----------------------------------------------------------------------
/// <summary>
/// �A�C�e�����N���X.
/// </summary>
// ----------------------------------------------------------------------
public class ItemBase : MonoBehaviour
{
    [Header("Base Param")]
    // �擾���̃G�t�F�N�g�v���n�u.
    [SerializeField] GameObject effectParticle = null;
    // �A�C�e���̃����_���[.
    [SerializeField] Renderer itemRenderer = null;

    // �R���C�_�[�R�[��.
    ColliderCallReceiver colliderCall = null;
    // �G�t�F�N�g���s�σt���O.
    bool isEffective = true;


    protected virtual void Start()
    {
        colliderCall = GetComponent<ColliderCallReceiver>();
        colliderCall.TriggerEnterEvent.AddListener(OnTriggerEnter);
    }

    // ----------------------------------------------------------------------
    /// <summary>
    /// �A�C�e���̃g���K�[�R���C�_�[�G���^�[���R�[��.
    /// </summary>
    /// <param name="col"> �N�������R���C�_�[. </param>
    // ----------------------------------------------------------------------
    void OnTriggerEnter(Collider col)
    {
        if (isEffective == false) return;

        if (col.gameObject.tag == "Player")
        {
            Debug.Log("�A�C�e�����擾");

            // �I�[�o�[���C�h�\�ȏ��������s.
            ItemAction(col);
            isEffective = false;

            // �G�t�F�N�g�\��.
            if (effectParticle != null)
            {
                var pos = (itemRenderer == null) ? this.transform.position : itemRenderer.gameObject.transform.position;
                var obj = Instantiate(effectParticle, pos, Quaternion.identity);
                var particle = obj.GetComponent<ParticleSystem>();
                StartCoroutine(AutoDestroy(particle));
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    // ----------------------------------------------------------------------
    /// <summary>
    /// �����j��.
    /// </summary>
    /// <param name="particle"> �p�[�e�B�N��. </param>
    // ----------------------------------------------------------------------
    IEnumerator AutoDestroy(ParticleSystem particle)
    {
        // ��Ƀ����_���[������.
        if (itemRenderer != null) itemRenderer.enabled = false;

        yield return new WaitUntil(() => particle.isPlaying == false);

        // �j��.
        Destroy(gameObject);
    }

    // ----------------------------------------------------------------------
    /// <summary>
    /// �A�C�e���擾���̏����i�I�[�o�[���C�h�\�j.
    /// </summary>
    /// <param name="col"> �擾�����R���C�_�[. </param>
    // ----------------------------------------------------------------------
    protected virtual void ItemAction(Collider col)
    {

    }
}