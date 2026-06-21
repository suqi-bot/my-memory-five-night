using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 消耗道具类 0-电池、1-绷带、2-回血药剂、3-香、4-相纸、5-罗盘、6-盐、7-子弹
/// </summary>
public class ConsumableItem
{
    public int id;
    public string name;
    public int type;
    public int maxNum;
    public string tips;
}
/// <summary>
/// 0-桃木剑、1-手枪、2-银剑、3-相机
/// </summary>
public class WeaponItem
{
    public int id;
    public string name;
    public int type;
    public string tips;
}


public class Item
{
    public List<ConsumableItem> cItem = new List<ConsumableItem>();
    public List<WeaponItem> wItem = new List<WeaponItem>();
}



/// <summary>
/// 玩家道具类 type: 0-消耗类型道具、1-武器道具
/// </summary>
public class PlayerBagItem
{
    public int id;
    public int num;
    public int type;
}

