using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class HudDamagePopupText : MonoBehaviour, IPoolableObject
{
    [SerializeField] private Text _text;
    [SerializeField] private float _duration = 0.5f;

    private RectTransform _rectTransform;
    private float _timer;

    public int InstanceId { get; private set; }

    public bool IsActive { get; private set; }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        IsActive = true;
    }

    private void Update()
    {
        if (!IsActive) { return; }

        _timer += Time.deltaTime;

        Vector2 movePosition = _rectTransform.anchoredPosition;

        movePosition.y += 100f * Time.deltaTime;
        movePosition.x += Mathf.Sin((_timer * 8f)) * 30f * Time.deltaTime;

        _rectTransform.anchoredPosition = movePosition;


        if (_timer >= _duration)
        {
            ObjectManager.Instance.DespawnObject(InstanceId);
        }
    }

    private void OnDisable()
    {
        IsActive = false;
    }

    public void Setup(float damage, Vector3 targetPosition, Color textColor)
    {
        _text.text = damage.ToString("N0");
        _timer = 0f;

        _text.color = textColor;

        Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0f, 0.3f), 0f);

        if (!GameManager.Instance.IsPlay) { return; }

        _rectTransform.position = Camera.main.WorldToScreenPoint(targetPosition + randomOffset);
        gameObject.SetActive(true);
    }

    public void Initialize(int instanceId)
    {
        InstanceId = instanceId;
    }
}