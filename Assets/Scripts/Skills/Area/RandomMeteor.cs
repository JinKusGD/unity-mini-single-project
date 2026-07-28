using UnityEngine;

public class RandomMeteor : MonoBehaviour, IPoolableObject
{
    public GameObject explosionPrefab;
    public float fallSpeed = 15f;
    public int ownerId;
    private Vector3 targetPos;
    private float damage;
    private float areaSize;
    public int InstanceId { get; private set; }

    public bool IsActive { get; private set; }

    private void OnEnable()
    {
        IsActive = true;
    }

    private void OnDisable()
    {
        IsActive = false;
    }

    public void SetInstanceId(int instanceId)
    {
        InstanceId = instanceId;
    }
    public void Setup(Vector3 target, int id, float dmg, float size)
    {
        this.ownerId = id;
        this.targetPos = target;
        this.damage = dmg;
        this.areaSize = size;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, fallSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (explosionPrefab != null)
        {
            GameObject exp = Instantiate(explosionPrefab, targetPos, Quaternion.identity);
            exp.transform.localScale = Vector3.one * areaSize;
            Destroy(exp, 0.5f);
        }

        float finalRadius = 1.5f * areaSize;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(targetPos, finalRadius);

        foreach (Collider2D col in hitEnemies)
        {
            if (ownerId == 1)
            {
                if (col.CompareTag("Enemy"))
                {
                    BaseStatus enemy = col.GetComponent<BaseStatus>();

                    enemy.TakeDamage("Aa", damage);
                }
            }
            else
            {
                if (col.CompareTag("Player"))
                {
                  //  BaseStatus player = ObjectManager.Instance.GetPlayer();

                 //   player.TakeDamage("Aa", damage);
                }
            }
        }

        Destroy(gameObject);
    }

    public void Initialize(int instanceId)
    {
        throw new System.NotImplementedException();
    }
}