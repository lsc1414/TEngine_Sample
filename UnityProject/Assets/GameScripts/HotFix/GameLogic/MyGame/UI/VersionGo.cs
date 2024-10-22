using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TEngine;

namespace GameLogic
{
    [Window(UILayer.UI,false,"VersionGO",false)]
    public class VersionGO : UIWindow
    {
        #region 脚本工具生成的代码
        private Text m_textAppver;
        private Text m_textResver;
        protected override void ScriptGenerator()
        {
            m_textAppver = FindChildComponent<Text>("root/m_textAppver");
            m_textResver = FindChildComponent<Text>("root/m_textResver");
        }
        #endregion

        #region 事件

        protected override void OnCreate()
        {
            base.OnCreate();
            m_textAppver.text = Version.GameVersion;
            m_textResver.text = GameModule.Resource.GetPackageVersion();
        }

        #endregion

    }
}