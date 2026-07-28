using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class HolyWaterBottle : MonoBehaviour, IPoolableObject
{
    private string skillid;

    public string ZonePrefabkey;
    public AnimationCurve heightCurve;
    public float flightDuration = 0.8f;
    public float zoneSize;

    private Vector3 startPos;
    private Vector3 targetPos;

    private float timer;

    private float damage;
    private float zoneDuration;

    private bool _isExploding;

    public int InstanceId { get; private set; }

    public bool IsActive { get; private set; }

    private void OnEnable()
    {
        IsActive = true;
        _isExploding = false;
    }

    private void OnDisable()
    {
        IsActive = false;
    }

    public void Setup(string id, Vector3 target, float dmg, float duration, float size, string zonePrefabKey)
    {
        skillid = id;

        startPos = transform.position;
        targetPos = target;

        damage = dmg;
        zoneDuration = duration;
        zoneSize = size;

        ZonePrefabkey = zonePrefabKey;

        timer = 0f;
        _isExploding = false;
    }

    private void Update()
    {
        if (_isExploding) { return; }

        timer += Time.deltaTime;

        float progress = timer / flightDuration;

        if (progress >= 1f)
        {
            _isExploding = true;

            transform.position = targetPos;

            DistroyBottleAsync().Forget();
            return;
        }

        Vector3 currentPos = Vector3.Lerp(startPos, targetPos, progress);

        if (heightCurve != null)
        {
            currentPos.y += heightCurve.Evaluate(progress) * 2f;
        }

        transform.position = currentPos;

        transform.Rotate(Vector3.forward * 360f * Time.deltaTime);
    }

    private async UniTask DistroyBottleAsync()
    {
        ObjectManager.Instance.DespawnObject(InstanceId);
        await ExplodeAsync();
    }

    private async UniTask ExplodeAsync()
    {
        string id = $"{skillid}_Zone";

        GameObject projectile = await ObjectManager.Instance.SpawnSkillObjectAsync(id, ZonePrefabkey, targetPos);

        if (projectile != null)
        {
            HolyWaterZone zone = projectile.GetComponent<HolyWaterZone>();

            zone.Setup(damage, zoneDuration, zoneSize);
        }
    }

    public void SetInstanceId(int instanceId)
    {
        InstanceId = instanceId;
    }

    public void Initialize(int instanceId)
    {
        throw new System.NotImplementedException();
    }
}