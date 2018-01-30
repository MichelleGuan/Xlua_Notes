using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

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
