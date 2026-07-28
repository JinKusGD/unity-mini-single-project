using UnityEngine;
using UnityEngine.UI;

public class HpSlotHud : MonoBehaviour
{
    RectTransform rect;
    Transform targetTransform;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        rect.position = Camera.main.WorldToScreenPoint(targetTransform.position);
    }

    public void InitSlot(Transform target)
    {
        targetTransform = target;
    }
}
