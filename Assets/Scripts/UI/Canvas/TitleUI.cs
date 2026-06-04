using Cysharp.Threading.Tasks;
using UnityEngine;

public class TitleUI : UIBase
{
    [SerializeField] private TitleButton Start_Button;
    [SerializeField] private TitleButton Collection_Button;
    [SerializeField] private TitleButton Option_Button;
    [SerializeField] private TitleButton Quit_Button;

    private void OnEnable()
    {
        BindButtonEventWithNullCheck(Start_Button, OnStartClick);
        BindButtonEventWithNullCheck(Collection_Button, OnCollectionClick);
        BindButtonEventWithNullCheck(Option_Button, OnOptionClick);
        BindButtonEventWithNullCheck(Quit_Button, OnQuitClick);

        AudioManager.Instance.PlayBGM("AudioClip_002_Title");
    }

    private void OnStartClick()
    {
        GameManager.Instance.StartGame().Forget();
    }

    private void OnCollectionClick()
    {
        Debug.Log("도감 열림");
    }

    private void OnOptionClick()
    {
        Debug.Log("옵션 열림");
    }

    private void OnQuitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}