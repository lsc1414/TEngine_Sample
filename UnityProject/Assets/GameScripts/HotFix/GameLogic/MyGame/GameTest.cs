using System;
using System.Collections;
using System.Collections.Generic;
using GameLogic.DebugModule;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class GameTest : MonoBehaviour
    {
        private MyDebuggerWindows windows;

        // Start is called before the first frame update
        void Start()
        {
            var item = ConfigSystem.Instance.Tables.TbItem.Get(10000);
            Log.Debug(item.Id + item.Name + item.Desc);
            //Debug.Log(ConfigSystem.Instance.Tables.TbtextInfo.Get("Hair").En);

            GameModule.UI.ShowUI<VersionGO>();
            windows = new MyDebuggerWindows();
            GameModule.Debugger.RegisterDebuggerWindow("测试界面/测试按钮", windows);
        }

        private void OnDestroy()
        {
            if (GameApp.IsValid) GameModule.Debugger.UnregisterDebuggerWindow("测试界面/测试按钮");
        }

        // Update is called once per frame
        void Update()
        {
            transform.eulerAngles += Vector3.forward * (Time.deltaTime * 100);
        }
    }
}