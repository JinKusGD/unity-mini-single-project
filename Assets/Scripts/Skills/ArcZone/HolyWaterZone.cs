using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HolyWaterZone : MonoBehaviour, IPoolableObject
{
    private float damage;
    private float duration;
    private float tickRate = 0.5f;

    private Dictionary<EnemyController, float> enemyTimers = new Dictionary<EnemyController, float>();
    private List<EnemyController> enemiesToRemove = new List<EnemyController>();

    public int InstanceId { get; private set; }

    public bool IsActive { get; private set; }

    public void OnEnable()
    {
        IsActive = true;
    }

    private void OnDisable()
    {
        IsActive = false;
    }

    public void Setup(float damage, float duration, float size)
    {
        this.damage = damage;
        this.duration = duration;

        transform.localScale = Vector3.one * size;
        DesponceZone().Forget();
    }

    private async UniTask DesponceZone()
    {
        int a  = (int)(1000 * duration);
        await UniTask.Delay(a);
        ObjectManager.Instance.DespawnObject(InstanceId);
    }

    void Update()
    {
        enemiesToRemove.Clear();

        List<EnemyController> keys = new List<EnemyController>(enemyTimers.Keys);

        foreach (EnemyController enemy in keys)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy)
            {
                enemiesToRemove.Add(enemy);
                continue;
            }

            enemyTimers[enemy] -= Time.deltaTime;

            if (enemyTimers[enemy] <= 0f)
            {
                Debug.Log("Hit");
                enemyTimers[enemy] = tickRate;
            }
        }

        foreach (EnemyController enemy in enemiesToRemove)
        {
            enemyTimers.Remove(enemy);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null && !enemyTimers.ContainsKey(enemy))
            {
                Debug.Log("hit");
                enemyTimers.Add(enemy, tickRate);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null && enemyTimers.ContainsKey(enemy))
            {
                enemyTimers.Remove(enemy); 
            }
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