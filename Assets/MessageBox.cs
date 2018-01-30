
using UnityEngine;
using UnityEngine.UI;
using XLua;
using System.Collections.Generic;
using System;
using UnityEngine.Events;


public class MessageBox : MonoBehaviour
{

    public static void ShowAlertBox(string message, string title, Action onFinished = null)
    {
        var alertPanel = GameObject.Find("Canvas").transform.Find("AlertBox");
            alertPanel = (Instantiate(Resources.Load("AlertBox")) as GameObject).transform;
            alertPanel.gameObject.name = "AlertBox";
            alertPanel.SetParent(GameObject.Find("Canvas").transform);
            alertPanel.localPosition = new Vector3(40f, 40f, 0f);
        alertPanel.Find("Title").GetComponent<Text>().text = title;
        alertPanel.Find("Message").GetComponent<Text>().text = message;
        alertPanel.gameObject.SetActive(true);
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
        var rechargePanel = GameObject.Find("Canvas").transform.Find("ConfirmBox");
        if (rechargePanel == null)
        {
            rechargePanel = (Instantiate(Resources.Load("ConfirmBox")) as GameObject).transform;
            rechargePanel.gameObject.name = "ConfirmBox";
            rechargePanel.SetParent(GameObject.Find("Canvas").transform);
            rechargePanel.localPosition = new Vector3(40f, 40f, 0f);
        }
        rechargePanel.Find("ConfirmTitle").GetComponent<Text>().text = title;
        rechargePanel.Find("ConfirmMessage").GetComponent<Text>().text = message;
        rechargePanel.gameObject.SetActive(true);
        if (onFinished != null)
        {
            var confirmBtn = rechargePanel.Find("ConfirmBtn").GetComponent<Button>();
            var cancelBtn = rechargePanel.Find("CancelBtn").GetComponent<Button>();
            UnityAction onconfirm = null;  //同上，只是多了一个按钮
            UnityAction oncancel = null;

            Action cleanup = () =>
            {
                confirmBtn.onClick.RemoveListener(onconfirm);
                cancelBtn.onClick.RemoveListener(oncancel);
                rechargePanel.gameObject.SetActive(false);
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
    public static void ShowRechargeBox(int dropdownValue,Action<bool> onFinished = null)
    {
        var rechargePanel = GameObject.Find("Canvas").transform.Find("RechargeBox");
        if (rechargePanel == null)
        {
            rechargePanel = (Instantiate(Resources.Load("RechargeBox")) as GameObject).transform;
            rechargePanel.gameObject.name = "RechargeBox";
            rechargePanel.SetParent(GameObject.Find("Canvas").transform);
            rechargePanel.localPosition = new Vector3(40f, 40f, 0f);
        }
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
           // rechargeBtn.onClick.AddListener(() => {dropdownValue = rechargePanel.Find("Dropdown").GetComponent<Dropdown>().value; });
        }
    }
}