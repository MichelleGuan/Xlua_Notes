using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
//从这个C#代码，调用async.lua（相当于一个接口），这个lua里面的方法都在另外一个messsagebox的lua和对应的C#文件实现
public class Cafe : MonoBehaviour {
    LuaEnv luaenv = null;
	// Use this for initialization
	void Start () {
        luaenv = new LuaEnv();
        luaenv.DoString("require 'async'");
	}
	
	// Update is called once per frame
	void Update () {
        if (luaenv != null)
        {
            luaenv.Tick();
        }
	}
}
