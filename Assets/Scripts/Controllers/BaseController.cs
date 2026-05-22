using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] protected Rigidbody2D _rigidbody;

    private void Awake()
    {
        if (_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        _rigidbody.gravityScale = 0f;
        _rigidbody.freezeRotation = true;

        Init();
    }

    protected virtual void Init() { }
}
