using UnityEngine;
using System.Collections;

[System.Serializable]
public class StageInformation{
    public enum LimitType { pipe, time };
    public LimitType limitType;
    public int limitNumber;
    public StageInfo.Mission[] mission;
}
