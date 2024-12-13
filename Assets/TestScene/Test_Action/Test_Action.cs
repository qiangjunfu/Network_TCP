using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Test_Action : MonoBehaviour
{
    //  如果是代码内部逻辑处理，使用 Action 和 Func 更简单高效。      例如：数学计算、日志打印、代码通知。
    //  如果需要在 Unity 的 Inspector 面板中拖拽绑定事件，使用 UnityEvent。   适合需要让美术或设计人员直接在面板中操作的场景。
    public Action<string> printMessage;          //无返回值
    public Func<int, int, int> multiply;         //有返回值 
    public IntStringEvent onMessageReceived;    //Unity 提供的事件系统，主要用于与 Unity Inspector 面板交互。



    void Start()
    {
        printMessage = message => Debug.Log(message);
        printMessage("Hello, Unity!");


        multiply = (a, b) => a * b;
        int result = multiply(4, 5);  // result 是 20
        print(result);



        //// 添加事件监听器
        //onMessageReceived.AddListener(TriggerEvent);
        //onMessageReceived.AddListener((id, message) =>
        //{
        //    Debug.Log($"Received Message: ID={id}, Message={message}");
        //});
        // 触发事件 
        onMessageReceived?.Invoke(1, "Hello, Unity Event!");
    }


    // Update is called once per frame
    void Update()
    {

    }



    public void TriggerEvent(int id, string message)
    {
        Debug.Log($"TriggerEvent : {id}, {message}");
    }

}



[System.Serializable] // 必须添加这个属性，确保类能在 Inspector 面板中序列化
public class IntStringEvent : UnityEvent<int, string> { }