/*
 * Tencent is pleased to support the open source community by making xLua available.
 * Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
 * Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
 * http://opensource.org/licenses/MIT
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using UnityEngine;
using System.Collections;
using XLua;

public class MrZhang
{
    //其实买车票的悲情人物是小张
    public static void BuyTicket()
    {
        Debug.Log("每次都让我去买票，渣渣！");
    }

    public static void BuyMovieTicket()
    {
        Debug.Log("我去，自己泡妞，还要让我带电影票！");
    }
}

//小明类
class MrMing
{
    //声明一个委托，其实就是个“命令”
    public delegate void BugTicketEventHandler();
    public delegate void TryEventHandler();

    public void Main()
    {
        //这里就是具体阐述这个命令是干什么的，本例是MrZhang.BuyTicket“小张买车票”
        BugTicketEventHandler myDelegate = new BugTicketEventHandler(MrZhang.BuyTicket);

        myDelegate += MrZhang.BuyMovieTicket;
        //这时候委托被附上了具体的方法
        myDelegate();
    }
} 
public class ByFile : MonoBehaviour {
    LuaEnv luaenv = null;
    // Use this for initialization
    void Start()
    {
        luaenv = new LuaEnv();
        luaenv.DoString("require 'michelle'");
        MrMing mydelegate = new MrMing();
        mydelegate.Main();
    }

    // Update is called once per frame
    void Update()
    {
        if (luaenv != null)
        {
            luaenv.Tick();
        }

    }

    void OnDestroy()
    {
        luaenv.Dispose();
    }
}
