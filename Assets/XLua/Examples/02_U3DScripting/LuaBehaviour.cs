/*
 * Tencent is pleased to support the open source community by making xLua available.
 * Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
 * Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
 * http://opensource.org/licenses/MIT
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using System;
//序列化会去除空值，对于复杂类型，会声明一个初始化对象，不能序列化多维数组和dicrionary
[System.Serializable]//序列化对象，这里服务于injection
public class Injection//一定要有，不然会报错
{
    public string name;
    public GameObject value;
}

[LuaCallCSharp]
public class LuaBehaviour : MonoBehaviour {
    public TextAsset luaScript;
    public Injection[] injections;
    internal static LuaEnv luaEnv = new LuaEnv(); //all lua behaviour shared one luaenv only!
    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;//1 second 
    private Action luaStart;
    private Action luaUpdate;
    private Action luaOnDestroy;
    private LuaTable scriptEnv;
    void Awake()
    {
        scriptEnv = luaEnv.NewTable();//这是一个luatable(键值对的数据结构，包含数组和表)
        LuaTable meta = luaEnv.NewTable();
        meta.Set("__index", luaEnv.Global);//metatable加入Key __index,Value luaEnv.Global
        scriptEnv.SetMetaTable(meta);//设置meta为table的metatable
        meta.Dispose();
        scriptEnv.Set("self", this);
        foreach (var injection in injections)//这个值在游戏面板添加，用于向lua载入C#游戏对象
        {
            scriptEnv.Set(injection.name, injection.value);
        }
        //执行lua代码，参数分别为，lua代码，发生error时的debug信息，对应的luatable
        //这里面text是TextAssets的属性，可以输出完整的string类型lua代码
        luaEnv.DoString(luaScript.text, "LuaBehaviour", scriptEnv);
        //使用代理绑定lua的awake start update ondestory函数
        Action luaAwake = scriptEnv.Get<Action>("awake");
        scriptEnv.Get("start", out luaStart);
        scriptEnv.Get("update", out luaUpdate);
        scriptEnv.Get("ondestroy", out luaOnDestroy);
        //通过代理机制绑定lua的几个函数到Unity对应部分执行
        if (luaAwake != null)
        {
            luaAwake();
        }
    }

	// Use this for initialization
	void Start ()
    {
        if (luaStart != null)
        {
            luaStart();
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (luaUpdate != null)
        {
            luaUpdate();
        }
        //清理没清理掉的垃圾并且重置上次清理的时间
        //间隔大于自动回收就说明自动回收没有在工作了呀
        if (Time.time - LuaBehaviour.lastGCTime > GCInterval)
        {
            luaEnv.Tick();
            LuaBehaviour.lastGCTime = Time.time;
        }
	}

    void OnDestroy()
    {
        //初始化
        if (luaOnDestroy != null)
        {
            luaOnDestroy();
        }
        luaOnDestroy = null;
        luaUpdate = null;
        luaStart = null;
        scriptEnv.Dispose();
        injections = null;
    }
}
