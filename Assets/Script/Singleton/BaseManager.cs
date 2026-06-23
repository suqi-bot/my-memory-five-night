using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//改为UTF-8编码
public class BaseManager<T> where T : new ()
{
    private static T instance = new T();
    public static T Instance => instance;
     

}
