using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] Transform rotationRoot = null;
    // ��������p�g�����X�t�H�[��.
    [SerializeField] Transform heightRoot = null;
    // �v���C���[�J����.
    [SerializeField] Camera mainCamera = null;

    // �J�������ʂ����S�̃v���C���[���獂��.
    [SerializeField] float lookHeight = 1.0f;
    // �J������]�X�s�[�h.
    [SerializeField] float rotationSpeed = 0.01f;
    // �J���������ω��X�s�[�h.
    [SerializeField] float heightSpeed = 0.001f;
    // �J�����ړ�����MinMax.
    [SerializeField] Vector2 heightLimit_MinMax = new Vector2(-1f, 3f);

    // �^�b�`�X�^�[�g�ʒu.
    Vector2 cameraStartTouch = Vector2.zero;
    // ���݂̃^�b�`�ʒu.
    Vector2 cameraTouchInput = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCameraLook(Transform player)
    {
        // �J�������L�����̏�����ɌŒ�.
        var cameraMarker = player.position;
        cameraMarker.y += lookHeight;
        var _camLook = (cameraMarker - mainCamera.transform.position).normalized;
        mainCamera.transform.forward = _camLook;
    }

    public void FixedUpdateCameraPosition(Transform player)
    {
        this.transform.position = player.position;
    }

    public void UpdateRightTouch(Touch touch)
    {
        // �^�b�`�J�n.
        if (touch.phase == TouchPhase.Began)
        {
            Debug.Log("�E�^�b�`�J�n");
            // �J�n�ʒu��ۊ�.
            cameraStartTouch = touch.position;
        }
        // �^�b�`��.
        else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            Debug.Log("�E�^�b�`��");
            // ���݂̈ʒu�𐏎��ۊ�.
            Vector2 position = touch.position;
            // �J�n�ʒu����̈ړ��x�N�g�����Z�o.
            cameraTouchInput = position - cameraStartTouch;
            // �J������].
            var yRot = new Vector3(0, cameraTouchInput.x * rotationSpeed, 0);
            var rResult = rotationRoot.rotation.eulerAngles + yRot;
            var qua = Quaternion.Euler(rResult);
            rotationRoot.rotation = qua;

            // �J��������.
            var yHeight = new Vector3(0, -cameraTouchInput.y * heightSpeed, 0);
            var hResult = heightRoot.transform.localPosition + yHeight;
            if (hResult.y > heightLimit_MinMax.y) hResult.y = heightLimit_MinMax.y;
            else if (hResult.y <= heightLimit_MinMax.x) hResult.y = heightLimit_MinMax.x;
            heightRoot.localPosition = hResult;
        }
        // �^�b�`�I��.
        else if (touch.phase == TouchPhase.Ended)
        {
            Debug.Log("�E�^�b�`�I��");
            cameraTouchInput = Vector2.zero;
        }
    }
}
