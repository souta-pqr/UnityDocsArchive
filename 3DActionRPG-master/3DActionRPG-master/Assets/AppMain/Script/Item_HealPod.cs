using UnityEngine;

// -------------------------------------------------------------
/// <summary>
/// �񕜃A�C�e���N���X.
/// </summary>
// -------------------------------------------------------------
public class Item_HealPod : ItemBase
{
    [Header("Item Param")]
    // �񕜗�.
    [SerializeField] int healPoint = 10;

    protected override void Start()
    {
        base.Start();
    }

    // -------------------------------------------------------------
    /// <summary>
    /// �A�C�e���擾������.
    /// </summary>
    /// <param name="col"> �N�����Ă����R���C�_�[. </param>
    // -------------------------------------------------------------
    protected override void ItemAction(Collider col)
    {
        base.ItemAction(col);
        var player = col.gameObject.GetComponent<PlayerController>();
        player.OnHeal(healPoint);
    }
}