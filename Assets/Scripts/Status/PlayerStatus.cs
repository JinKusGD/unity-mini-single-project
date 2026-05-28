using UnityEngine;

public class PlayerStatus : BaseStatus
{
    public override UnitType UnitType
    {
        get { return UnitType.Player; }
    }


    protected override void Die()
    {
        Debug.Log("PlayerDie");
    }
}
