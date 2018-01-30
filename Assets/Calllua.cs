using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using XLua;

public class Calllua : MonoBehaviour {
    LuaEnv luaenv = null;
    string script = @"
      function sum()
      sum=0
      for i=100,1,-2 do
      sum=sum+i
      end 
      end
    ";
	// Use this for initialization
	void Start () {
        luaenv = new LuaEnv();
        luaenv.DoString(script);

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
