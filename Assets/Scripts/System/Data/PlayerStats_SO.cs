using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class PlayerStats_SO : ScriptableObject
{
    public int progress; // 当前游玩到的关卡
    // TODO: 定义其他需要保存的数据
}
