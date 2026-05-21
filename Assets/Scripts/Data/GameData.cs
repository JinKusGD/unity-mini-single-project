using System;
using System.Collections.Generic;

[Serializable]
public class JsonWrapper<T>
{
    public List<T> items;
}

[Serializable]
public abstract class GameData
{
    public string Id;
}