public class TitleButton : ButtonBase
{ 
    protected override void OnClickDefaultEvent()
    {
        AudioManager.Instance.PlaySFX("AudioClip_003_ButtonClick");
    }
}
