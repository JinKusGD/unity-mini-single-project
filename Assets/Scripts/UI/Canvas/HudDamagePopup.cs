using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class HudDamagePopup : UIBase
{
    private readonly Dictionary<GameObject, HudDamagePopupText> _popupTextComponentCache = new Dictionary<GameObject, HudDamagePopupText>();

    private void OnEnable()
    {
        foreach(var a in _popupTextComponentCache.Keys)
        {
           Destroy(a);
        }

        _popupTextComponentCache.Clear();

        WarmUpPopupTextPool().Forget();
    }

    public async UniTask ShowDamagePopupText(float damage, Vector3 targetPosition, Color textColor)
    {
        GameObject damagePopupText = await SpawnDamagePopupText();

        HudDamagePopupText damagePopupTextComponent = _popupTextComponentCache[damagePopupText];
        damagePopupTextComponent.Setup(damage, targetPosition, textColor);
    }

    private async UniTask WarmUpPopupTextPool()
    {
        List<GameObject> spawnedDamagePopupTextList = new List<GameObject>(100);

        for (int i = 0; i < 100; i++)
        {
            GameObject damagePopupText = await SpawnDamagePopupText();

            spawnedDamagePopupTextList.Add(damagePopupText);
        }

        foreach (GameObject damagePopupText in spawnedDamagePopupTextList)
        {
            ObjectManager.Instance.DespawnObject(damagePopupText);
        }
    }

    private async UniTask<GameObject> SpawnDamagePopupText()
    {
        GameObject damagePopupText = await ObjectManager.Instance.SpawnDamagePopupTextAsync(transform);

        if (damagePopupText == null)
        {
            Debug.LogError($"[SpawnDamagePopupText] SpawnDamagePopupText 스폰 실패.");
            return null;
        }

        if (!damagePopupText.TryGetComponent(out HudDamagePopupText damageTextUI))
        {
            Debug.LogError($"[{damagePopupText.name}] 생성된 오브젝트에 HudDamagePopupText 컴포넌트가 없습니다.");
            ObjectManager.Instance.DestroyObject(damagePopupText);
            return null;
        }

        if (!_popupTextComponentCache.ContainsKey(damagePopupText))
        {
            _popupTextComponentCache[damagePopupText] = damageTextUI;
        }

        return damagePopupText;
    }
}