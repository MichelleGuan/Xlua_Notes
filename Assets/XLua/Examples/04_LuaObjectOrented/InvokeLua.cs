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

public class InvokeLua : MonoBehaviour
{
    //接口回调准备好的接口ICalc
    [CSharpCallLua]
    public interface ICalc
    {
        int Add(int a, int b);
        int Mult { get; set; }
    }
    //定义委托类CalcNew,返回接口ICalc,参数为一个整数和一个数量可变的字符串
    [CSharpCallLua]
    public delegate ICalc CalcNew(int mult, params string[] args);
    //lua元表操作__index，可掉用Add方法，self=this，...对应可变字符串
    private string script = @"
              --这个是luatable的元表,__index,在表和元表都没有这个键的时候调用
                local calc_mt = {
                    __index = {
                        Add = function(self, a, b)
                            return (a + b) * self.Mult
                        end
                    }
                }
             --lua table
                Calc = {
	                New = function (mult, ...)
                        print(...)
                        return setmetatable({Mult = mult}, calc_mt)
                    end
                }
	        ";
    // Use this for initialization
    void Start()
    {
        LuaEnv luaenv = new LuaEnv();
        Test(luaenv);//调用了带可变参数的delegate，函数结束都不会释放delegate，即使置空并调用GC
        luaenv.Dispose();
        
    }

    void Test(LuaEnv luaenv)
    {
        luaenv.DoString(script);
        //声明代理对象，把它和lua Calc的New方法绑定，mult=mult;...=args
        CalcNew calc_new = luaenv.Global.GetInPath<CalcNew>("Calc.New");
        //通过委托传参给New,打印“hi","john",calc_mt的元表，Mult设置为10
        ICalc calc = calc_new(10, "hi", "john"); //constructor
        //调用元表的add方法，calc作为接口可以使用add和mult属性方法，this.Mult=10（通过元表设置）
        //local calc_mt，定义了Calc元表的__index，查找不到add方法，自动执行了__index
        Debug.Log("sum(*10) =" + calc.Add(1, 2));
        //通过接口get set方法，改变元表Mult
        calc.Mult = 100;
        Debug.Log("sum(*100)=" + calc.Add(1, 2));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
