#if UNITY_EDITOR_WIN // 暂时只支持Windows,其他平台暂时不支持
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TEngine.Editor.SVNTools
{
    public static class SVNTools
    {
        #region SVN

        private static List<string> drives = new List<string>() { "c:", "d:", "e:", "f:" };
        private static string svnPath = @"\Program Files\TortoiseSVN\bin\";
        private static string svnProc = @"TortoiseProc.exe";
        private static string svnProcPath = "";

        [MenuItem("Assets/SVN 更新 &#u")]
        public static void UpdateFromSVN()
        {
            if (string.IsNullOrEmpty(svnProcPath))
                svnProcPath = GetSvnProcPath();
            var dir = new DirectoryInfo(Application.dataPath);
            var path = dir.Parent.FullName.Replace('/', '\\');
            string para;
            if (Selection.assetGUIDs.Length == 0)
            {
                para = "/command:update /path:\"" + path /*+ "/Assets"*/ + "\"";
                System.Diagnostics.Process.Start(svnProcPath, para);
            }
            else if (Selection.assetGUIDs.Length == 1)
            {
                para = "/command:update /path:\"" + path + "/" +
                       AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]) +
                       "\" /closeonend:0";
                System.Diagnostics.Process.Start(svnProcPath, para);
            }
            else
            {
                UnityEditor.EditorUtility.DisplayDialog("SVN:警告", "请选择一个文件或目录", "知道了");
            }
        }

        [MenuItem("Assets/SVN 更新 根目录")]
        public static void UpdateFromSVNRootPath()
        {
            if (string.IsNullOrEmpty(svnProcPath))
                svnProcPath = GetSvnProcPath();
            var dir = new DirectoryInfo(Application.dataPath);
            var path = dir.Parent.FullName.Replace('/', '\\');

            string para;
            para = "/command:update /path:\"" + path + "\" /closeonend:0";
            System.Diagnostics.Process.Start(svnProcPath, para);
        }

        [MenuItem("Assets/SVN 提交 &#s")]
        public static void CommitToSVN()
        {
            if (string.IsNullOrEmpty(svnProcPath))
                svnProcPath = GetSvnProcPath();

            var dir = new DirectoryInfo(Application.dataPath);
            var path = dir.Parent.FullName.Replace('/', '\\');

            string para;
            if (Selection.assetGUIDs.Length == 0)
            {
                para = "/command:commit /path:\"" + path /*+ "/Assets"*/ + "\"";
                System.Diagnostics.Process.Start(svnProcPath, para);
            }
            else if (Selection.assetGUIDs.Length == 1)
            {
                para = "/command:commit /path:\"" + path + "/" +
                       AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]) +
                       "\"";
                System.Diagnostics.Process.Start(svnProcPath, para);
            }
            else
            {
                UnityEditor.EditorUtility.DisplayDialog("SVN:警告", "请选择一个文件或目录", "知道了");
            }
        }

        private static string GetSvnProcPath()
        {
            foreach (var item in drives)
            {
                var path = string.Concat(item, svnPath, svnProc);
                if (File.Exists(path))
                    return path;
            }

            return EditorUtility.OpenFilePanel("选择TortoiseProc.exe文件所在的路径", "c:\\", "exe");
        }

        #endregion
    }
}
#endif