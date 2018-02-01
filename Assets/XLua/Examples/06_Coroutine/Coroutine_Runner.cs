using UnityEngine;
using XLua;
using System.Collections.Generic;
using System.Collections;
using System;

[LuaCallCSharp]
public class Coroutine_Runner : MonoBehaviour
{
//如果yeild ruturn后面的参数是可枚举的，比如时间，就再嵌套一层协程,是www这种不可以枚举的就直接返回协程，到lua里面用作者的异步方法执行    
    public void YieldAndCallback(object to_yield, Action callback)  
    {
        StartCoroutine(CoBody(to_yield, callback));
    }

    private IEnumerator CoBody(object to_yield, Action callback)  //判定可枚举，先实现枚举，再进入lua协程
    {
        if (to_yield is IEnumerator)
            yield return StartCoroutine((IEnumerator)to_yield);
        else
            yield return to_yield;
        callback();
    }
}

public static class CoroutineConfig
{
    [LuaCallCSharp]
    public static List<Type> LuaCallCSharp
    {
        get
        {
            return new List<Type>()
            {
                typeof(WaitForSeconds),
                typeof(WWW)
            };
        }
    }
}
