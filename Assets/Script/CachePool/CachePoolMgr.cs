using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//改为UTF-8编码
public class CachePoolMgr : BaseManager<CachePoolMgr>
{

    public Dictionary<string,PoolData> poolDic = new Dictionary<string,PoolData>();

    private GameObject poolObj;

    public void GetObj(string name,UnityAction<GameObject> callBack,Vector3 pos)
    {
        if (poolDic.ContainsKey(name) && poolDic[name].poolList.Count > 0)
        {
            GameObject obj = poolDic[name].GetObj();
            obj.transform.position = pos;
            obj.SetActive(true);
            callBack(obj);
        }
        else  {
            //TODO:需要等待资源管理器方案落地
            
            // ResMgr.Instance.LoadAsync<GameObject>(name, (obj) =>
            // {
            //     obj.name = name; 
            //     callBack.Invoke(obj);
            // },pos);    
        }
    }
    public void PushObj(string name,GameObject obj)
    {
        if(poolObj == null) poolObj = new GameObject("Pool");

        if (poolDic.ContainsKey(name))
        {
            poolDic[name].PushObj(obj);
        }
        else
        {
            poolDic.Add(name,new PoolData(obj,poolObj));
        }
    }
    public void ClearCache()
    {
        poolDic.Clear();
        poolObj = null;
    }
}
