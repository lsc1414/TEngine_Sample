using TEngine;
using UnityEngine;

namespace GameLogic.DebugModule
{
    public class MyDebuggerWindows : IDebuggerWindow
    {
        private const float TitleWidth = 240f;
        private Vector2 m_ScrollPosition = Vector2.zero;

        public void Initialize(params object[] args)
        {
        }

        public void Shutdown()
        {
        }

        public void OnEnter()
        {
        }

        public void OnLeave()
        {
        }

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }

        public void OnDraw()
        {
            m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);
            {
                OnDrawScrollableWindow();
            }
            GUILayout.EndScrollView();
        }

        protected void OnDrawScrollableWindow()
        {
            GUILayout.Label("<b>Environment Information</b>");
            GUILayout.BeginVertical("box");
            if (GUILayout.Button("加钱", GUILayout.Height(30f)))
            {
                //TakeSample();
                Log.Debug("加钱");
            }
            GUILayout.EndVertical();
        }

        protected static void DrawItem(string title, string content)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(title, GUILayout.Width(TitleWidth));
                if (GUILayout.Button(content, "label"))
                {
                    //CopyToClipboard(content);
                }
            }
            GUILayout.EndHorizontal();
        }

        // private static TextEditor s_TextEditor = null;
        // private static void CopyToClipboard(string content)
        // {
        //     s_TextEditor.text = content;
        //     s_TextEditor.OnFocus();
        //     s_TextEditor.Copy();
        //     s_TextEditor.text = string.Empty;
        // }
    }
}