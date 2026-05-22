using UnityEngine;

public struct PoolResult
{
    public bool IsSuccess { get;  private set; }
    public GameObject ResultObject { get; private set; }

    public PoolResult(bool isSuccess, GameObject resultObject)
    {
        IsSuccess = isSuccess;
        ResultObject = resultObject;
    }
}