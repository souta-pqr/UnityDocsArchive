using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    void Start()
    {

    }

    // --------------------------------------------------------
    /// <summary>
    /// ��ʃ^�b�v�R�[���o�b�N.
    /// </summary>
    // --------------------------------------------------------
    public void OnScreenTap()
    {
        SceneManager.LoadScene("MainScene");
    }
}