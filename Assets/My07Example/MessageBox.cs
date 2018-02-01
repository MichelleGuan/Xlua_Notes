
using UnityEngine;
using UnityEngine.UI;
using XLua;
using System.Collections.Generic;
using System;
using UnityEngine.Events;


public class MessageBox : MonoBehaviour
{
    //异步调用的前两个值是可变的Obj参，后面是回调
    public static void ShowAlertBox(string message, string title, Action onFinished = null)
    {
        //从预制生成面板，加标题内容
        var alertPanel = GameObject.Find("Canvas").transform.Find("AlertBox");
            alertPanel = (Instantiate(Resources.Load("AlertBox")) as GameObject).transform;
            alertPanel.gameObject.name = "AlertBox";
            alertPanel.SetParent(GameObject.Find("Canvas").transform);
            alertPanel.localPosition = new Vector3(40f, 40f, 0f);
        alertPanel.gameObject.SetActive(true);
        alertPanel.Find("Title").GetComponent<Text>().text = title;
        alertPanel.Find("Message").GetComponent<Text>().text = message;
        if (onFinished != null)
        {
            var button = alertPanel.Find("AlertBtn").GetComponent<Button>();
            UnityAction onclick = null;   
            //回调委托的定义，被点击时执行
            onclick = () =>
            {
                onFinished();
                alertPanel.gameObject.SetActive(false);
                button.onClick.RemoveListener(onclick);
            };
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(onclick);  //当被点击的时候，调用onclick委托
        }
    }

    public static void ShowConfirmBox(string message, string title, Action<bool> onFinished = null)
    {
        var confirmPanel = GameObject.Find("Canvas").transform.Find("ConfirmBox");
        if (confirmPanel == null)
        {
            confirmPanel = (Instantiate(Resources.Load("ConfirmBox")) as GameObject).transform;
            confirmPanel.gameObject.name = "ConfirmBox";
            confirmPanel.SetParent(GameObject.Find("Canvas").transform);
            confirmPanel.localPosition = new Vector3(40f, 40f, 0f);
            confirmPanel.Find("ConfirmTitle").GetComponent<Text>().text = title;
            confirmPanel.Find("ConfirmMessage").GetComponent<Text>().text = message;
        }
            confirmPanel.gameObject.SetActive(true);
        if (onFinished != null)
        {
            var confirmBtn = confirmPanel.Find("ConfirmBtn").GetComponent<Button>();
            var cancelBtn = confirmPanel.Find("CancelBtn").GetComponent<Button>();
            UnityAction onconfirm = null;  //同上，只是多了一个按钮
            UnityAction oncancel = null;

            Action cleanup = () =>
            {
                confirmBtn.onClick.RemoveListener(onconfirm);
                cancelBtn.onClick.RemoveListener(oncancel);
                confirmPanel.gameObject.SetActive(false);
            };

            onconfirm = () =>
            {
                onFinished(true);
                cleanup();
            };

            oncancel = () =>
            {
                onFinished(false);
                cleanup();
            };
            confirmBtn.onClick.RemoveAllListeners();
            cancelBtn.onClick.RemoveAllListeners();
            confirmBtn.onClick.AddListener(onconfirm);
            cancelBtn.onClick.AddListener(oncancel);
        }
    }

    public static int dropdownValue; //拿到下拉菜单选择的值
    public static void ShowRechargeBox(Action<bool> onFinished = null)
    {
        var rechargePanel = GameObject.Find("Canvas").transform.Find("RechargeBox");
        if (rechargePanel == null) {
            rechargePanel = (Instantiate(Resources.Load("RechargeBox")) as GameObject).transform;
            rechargePanel.gameObject.name = "RechargeBox";
            rechargePanel.SetParent(GameObject.Find("Canvas").transform); 
            rechargePanel.localPosition = new Vector3(40f, 40f, 0f); }
            rechargePanel.gameObject.SetActive(true);
       
        if (onFinished != null)
        {
            var rechargeBtn = rechargePanel.Find("RechargeBtn").GetComponent<Button>();
            UnityAction onrecharge = null;
            Action cleanup = () =>
            {
                rechargeBtn.onClick.RemoveListener(onrecharge);
                rechargePanel.gameObject.SetActive(false);
            };

            onrecharge = ()=>
            {
                onFinished(true);
                cleanup();  
            };

            rechargeBtn.onClick.RemoveAllListeners();
            rechargeBtn.onClick.AddListener(onrecharge);
            //addlistener后面回调类型就是Unityaction，可以用lambda写，用委托也一样
            rechargeBtn.onClick.AddListener(() => { dropdownValue = rechargePanel.Find("Dropdown").GetComponent<Dropdown>().value; });
        }
    }
}
//xLua用白名单来指明生成哪些代码，而白名单通过attribute来配置，比如你想从lua调用c#的某个类，希望生成适配代码，你可以为这个类型打一个LuaCallCSharp标签：
//如果希望把一个lua函数适配到一个C#delegate（一类是C#侧各种回调：UI事件，delegate参数，比如List<T>:ForEach；另外一类场景是通过LuaTable的Get函数指明一个lua函数绑定到一个delegate）。或者把一个lua table适配到一个C# interface，该delegate或者interface需要加上该配置。
//不写白名单在生成代码后会报错，修改后记得重新生成代码，才可以使用
public static class MessageBoxConfig
{
    [CSharpCallLua]
    public static List<Type> CSharpCallLua = new List<Type>()
    {
        typeof(Action),
        typeof(Action<bool>),
        typeof(UnityAction),
    };
}