using UnityEngine;
using System.Collections.Generic;
using XLua;

[CSharpCallLua]
public delegate int TestOutDelegate(HotfixCalc calc, int a, out double b, ref string c);

[Hotfix]
public class HotfixCalc
{
    public int Add(int a, int b)
    {
        return a - b;
    }

    public Vector3 Add(Vector3 a, Vector3 b)
    {
        return a - b;
    }

    public int TestOut(int a, out double b, ref string c)
    {
        b = a + 2;
        c = "wrong version";
        return a + 3;
    }

    public int TestOut(int a, out double b, ref string c, GameObject go)
    {
        return TestOut(a, out b, ref c);
    }

    public T Test1<T>()
    {
        return default(T);
    }

    public T1 Test2<T1, T2, T3>(T1 a, out T2 b, ref T3 c)
    {
        b = default(T2);
        return a;
    }

    public static int Test3<T>(T a)
    {
        return 0;
    }

    public static void Test4<T>(T a)
    { 
    }

    public void Test5<T>(int a, params T[] arg)
    {

    }
}

public class NoHotfixCalc
{
    public int Add(int a, int b)
    {
        return a + b;
    }
}

[Hotfix]
public class GenericClass<T>
{
    T a;

    public GenericClass(T a)
    {
        this.a = a;
    }

    public void Func1()
    {
        Debug.Log("a=" + a);
    }

    public T Func2()
    {
        return default(T);
    }
}

[Hotfix]
public class InnerTypeTest
{
    public void Foo()
    {
        _InnerStruct ret = Bar();
        Debug.Log("{x=" + ret.x + ",y= " + ret.y + "}");
    }

    struct _InnerStruct
    {
        public int x;
        public int y;
    }

    _InnerStruct Bar()
    {
        return new _InnerStruct { x = 1, y = 2 };
    }
}

[Hotfix]
public struct StructTest
{
    GameObject go;
    public StructTest(GameObject go)
    {
        this.go = go;
    }

    public GameObject GetGo(int a, object b)
    {
        return go;
    }
}

[Hotfix(HotfixFlag.Stateful)] //详见作者hotfix文档
public struct GenericStruct<T>
{
    T a;

    public GenericStruct(T a)
    {
        this.a = a;
    }

    public T GetA(int p)
    {
        return a;
    }
}

public class HotfixTest2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
        LuaEnv luaenv = new LuaEnv();
        HotfixCalc calc = new HotfixCalc();
        NoHotfixCalc ordinaryCalc = new NoHotfixCalc();

 //对比热更与不热更标签用时（毫秒）,执行同样的一组循环
        int CALL_TIME = 100 * 1000 * 1000;
        var start = System.DateTime.Now;
        for (int i = 0; i < CALL_TIME; i++)
        {
            calc.Add(2, 1);  //return a-b=1
        }
        var d1 = (System.DateTime.Now - start).TotalMilliseconds; 
        Debug.Log("Hotfix using:" + d1);

        start = System.DateTime.Now;
        for (int i = 0; i < CALL_TIME; i++)
        {
            ordinaryCalc.Add(2, 1);
        }
        var d2 = (System.DateTime.Now - start).TotalMilliseconds;
        Debug.Log("No Hotfix using:" + d2);
        Debug.Log("drop:" + ((d1 - d2) / d1));
        Debug.Log("Before Fix: 2 + 1 = " + calc.Add(2, 1)); //热更变成减法了
        Debug.Log("Before Fix: Vector3(2, 3, 4) + Vector3(1, 2, 3) = " + calc.Add(new Vector3(2, 3, 4), new Vector3(1, 2, 3)));//向量减法
        luaenv.DoString(@"
            xlua.hotfix(CS.HotfixCalc, 'Add', function(self, a, b)
                return a + b
            end)
        ");
        Debug.Log("After Fix: 2 + 1 = " + calc.Add(2, 1)); //热更再变成加法
        Debug.Log("After Fix: Vector3(2, 3, 4) + Vector3(1, 2, 3) = " + calc.Add(new Vector3(2, 3, 4), new Vector3(1, 2, 3)));

        double num;
        string str = "hehe";
        int ret = calc.TestOut(100, out num, ref str);
        Debug.Log("ret = " + ret + ", num = " + num + ", str = " + str); //返回值是100+3，num+=2,str被赋值wrong version
        //打印test out, self=这个类，打印传入的值
        luaenv.DoString(@"
            xlua.hotfix(CS.HotfixCalc, 'TestOut', function(self, a, c, go)
                    print('TestOut', self, a, c, go)
                    if go then error('test error') end
                    return a + 10 , a + 20, 'right version'
                end)
        ");
        str = "hehe";
        ret = calc.TestOut(100, out num, ref str);
        Debug.Log("ret = " + ret + ", num = " + num + ", str = " + str); //110 120 rightversion,第一个可以理解成C#只接受第一个返回值

        luaenv.DoString(@"
            xlua.hotfix(CS.HotfixCalc, {
                 Test1 = function(self)
                    print('Test1', self) --self不是实参，相当于this，但是可以指向表
                    return 1
                 end;
                 Test2 = function(self, a, b)  --out向外传参可以不写
                     print('Test1', self, a, b)
                     return a + 10, 1024, b
                 end;
                 Test3 = function(a)
                    print(a)
                    return 10
                 end;
                 Test4 = function(a)
                    print(a)
                 end;
                 Test5 = function(self, a, ...)
                    print('Test4', self, a, ...)
                 end
            })
        ");
 //泛型热更演示
        int r1 = calc.Test1<int>();  //无论什么类型都是返回1
        double r2 = calc.Test1<double>();
        Debug.Log("r1:" + r1 + ",r2:" + r2);

        string ss = "heihei"; //ref必须初始化
        int r3 = calc.Test2(r1, out r2, ref ss);
        Debug.Log("r1:" + r1 + ",r2:" + r2 + ",r3:" + r3 + ",ss:" + ss); //这里面return c#只能接受第一个参数，因为有out和ref所以，r2 r3能取到值
       
        r3 = HotfixCalc.Test3("test3");
        r3 = HotfixCalc.Test3(2);
        r3 = HotfixCalc.Test3(this);  //lua变成C#之后再执行，这里面就和C#脚本的this是一样的
        Debug.Log("r3:" + r3);
        HotfixCalc.Test4(this);
        HotfixCalc.Test4(2);
        calc.Test5(10, "a", "b", "c");
        calc.Test5(10, 1, 3, 5);

        Debug.Log("----------------------before------------------------");
       //Stateless比较适合无状态的类，有状态的话，你得通过反射去操作私有成员，也没法新增状态（field）。Stateless有个好处，可以运行的任意时刻执行替换。
       // Stateful的代价是会在类增加一个LuaTable类型的字段（中间层面增加，不会改源代码）。但这种方式是适用性更广，比如你不想要lua状态，可以在构造函数拦截那返回空。而且操作状态性能比反射操作C#私有变量要好，也可以随意新增任意的状态信息。缺点是，执行成员函数之前就new好的对象，接收到的状态会是空，所以需要重启，在一开始就执行替换。
        TestStateful(); //Stateful方式下你可以在Lua的构造函数返回一个table，然后后续成员函数调用会把这个table给传递过去。
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
        luaenv.DoString(@"
            xlua.hotfix(CS.StatefullTest, {
                ['.ctor'] = function(csobj)  --构造函数固定写法
                    return {evt = {}, start = 0}
                end;
                set_AProp = function(self, v)
                    print('set_AProp', v)
                    self.AProp = v
                end;
                get_AProp = function(self)
                    return self.AProp
                end;
                get_Item = function(self, k)
                    print('get_Item', k)
                    return 1024
                end;
                set_Item = function(self, k, v)
                    print('set_Item', k, v)
                end;
                add_AEvent = function(self, cb)
                    print('add_AEvent', cb)
                    table.insert(self.evt, cb)
                end;
                remove_AEvent = function(self, cb)
                   print('remove_AEvent', cb)
                   for i, v in ipairs(self.evt) do
                       if v == cb then
                           table.remove(self.evt, i)
                           break
                       end
                   end
                end;
                Start = function(self)
                    print('Start')
                    for _, cb in ipairs(self.evt) do
                        cb(self.start, 2)
                    end
                    self.start = self.start + 1
                end;
                StaticFunc = function(a, b, c)
                   print(a, b, c)
                end;
                GenericTest = function(self, a)
                   print(self, a)
                end;
                Finalize = function(self)   --析构函数固定写法
                   print('Finalize', self)
                end
           })
        ");
        Debug.Log("----------------------after------------------------");
        TestStateful(); //在下面调用执行的
        luaenv.FullGc();
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();

        var genericObj = new GenericClass<double>(1.1);
        genericObj.Func1();
        Debug.Log(genericObj.Func2());
        //泛型类要指定类型后热更
        luaenv.DoString(@"
            xlua.hotfix(CS['GenericClass`1[System.Double]'], {  --数字代表几个参数，固定的泛型命名写法 Type.GetType Method (String)
                ['.ctor'] = function(obj, a)
                    print('GenericClass<double>', obj, a)
                end;
                Func1 = function(obj)
                    print('GenericClass<double>.Func1', obj)
                end;
                Func2 = function(obj)
                    print('GenericClass<double>.Func2', obj)
                    return 1314
                end
            })
        ");
        genericObj = new GenericClass<double>(1.1);
        genericObj.Func1();
        Debug.Log(genericObj.Func2());

        InnerTypeTest itt = new InnerTypeTest();
        itt.Foo();
        luaenv.DoString(@"
            xlua.hotfix(CS.InnerTypeTest, 'Bar', function(obj)
                    print('lua Bar', obj)
                    return {x = 10, y = 20}
                end)
        ");
        itt.Foo();

        StructTest st = new StructTest(gameObject);
        Debug.Log("go=" + st.GetGo(123, "john"));
        luaenv.DoString(@"
            xlua.hotfix(CS.StructTest, 'GetGo', function(self, a, b)
                    print('GetGo', self, a, b)
                    return nil
                end)
        ");
        Debug.Log("go=" + st.GetGo(123, "john"));

        GenericStruct<int> gs = new GenericStruct<int>(1);
        Debug.Log("gs.GetA()=" + gs.GetA(123));
        luaenv.DoString(@"
            xlua.hotfix(CS['GenericStruct`1[System.Int32]'], 'GetA', function(self, a)
                    print('GetA',self, a)
                    return 789
                end)
        ");
        Debug.Log("gs.GetA()=" + gs.GetA(123));

        try
        {
            calc.TestOut(100, out num, ref str, gameObject);
        }
        catch(LuaException e)
        {
            Debug.Log("throw in lua an catch in c# ok, e.Message:" + e.Message);
        }
    }

    void TestStateful()
    {
        StatefullTest sft = new StatefullTest();
        sft.AProp = 10;
        Debug.Log("sft.AProp:" + sft.AProp);
        sft["1"] = 1; //给索引为1的值设为1，这里进行了set_item操作
        Debug.Log("sft['1']:" + sft["1"]); 
        System.Action<int, double> cb = (a, b) =>  //add_event方法
        {
            Debug.Log("a:" + a + ",b:" + b);
        };
        sft.AEvent += cb;
        sft.Start();
        sft.Start();
        sft.AEvent -= cb;
        sft.Start();
        StatefullTest.StaticFunc(1, 2);
        StatefullTest.StaticFunc("e", 3, 4);
        sft.GenericTest(1);
        sft.GenericTest("hehe");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
