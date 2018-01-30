using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class HelloWorld2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
        LuaEnv luaenv = new LuaEnv();
        luaenv.DoString("print('Hello world')");
        luaenv.Dispose();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
