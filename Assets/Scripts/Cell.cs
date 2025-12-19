using UnityEngine;

public class Cell
{
    public enum Type
    {
        Empty,
        Mine,
        Number,
        NumEmpty,
        Block,
    }

    public Vector3Int position;
    public Type type;
    public int number;
    public bool revealed;
    public bool flagged;
    public bool exploded;
    public bool numempty;
    public bool block;
}
