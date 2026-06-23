using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//改为UTF-8编码
public class PoolData
{
    public GameObject fatherObj;

    public List<GameObject> poolList;

    public PoolData(GameObject obj , GameObject poolObj)
    {
        fatherObj = new GameObject(obj.name);
        fatherObj.transform.SetParent(poolObj.transform);
        poolList = new List<GameObject>();
        PushObj(obj);
        
    }
    public void PushObj(GameObject obj)
    {
        obj.SetActive(false);
        poolList.Add(obj);
        obj.transform.SetParent(fatherObj.transform);
    }
    public GameObject GetObj()
    {
        GameObject obj = null;
        obj = poolList[0];
        poolList.RemoveAt(0);
        obj.transform.parent = null;
       return obj;
    }
}
