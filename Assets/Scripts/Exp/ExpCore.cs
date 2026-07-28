using UnityEngine;
using UnityEngine.UI;

public class ExpCore : MonoBehaviour, IPoolableObject
{
    [SerializeField] private Image _coreImage;

    private int _expValue;

    public bool IsActive { get; private set; }

    public int InstanceId { get; private set; }

    private void OnEnable()
    {
        IsActive = true;
    }

    private void OnDisable()
    {
        IsActive = false;
    }

    public void SetupExpValue(int expValue)
    {
        _expValue = expValue;
    }

    public void SetInstanceId(int instanceId)
    {
        InstanceId = instanceId;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ObjectManager.Instance.GetPlayer().AddExp(_expValue);
            ObjectManager.Instance.DespawnObject(InstanceId);
        }
    }

    public void Initialize(int instanceId)
    {
        InstanceId = instanceId;
    }
}
