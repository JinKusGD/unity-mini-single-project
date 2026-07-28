using UnityEngine;

public class FieldTrigger : MonoBehaviour
{
    [SerializeField] private string _fieldName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) { return; }

        MapManager.Instance.ChangedField(_fieldName);
    }
}
