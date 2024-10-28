using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TEngine
{
    [DisallowMultipleComponent]
    public class PlatformNativeModule : Module
    {
        public PlatformNativeManager Manager = null;

        private void Start()
        {
            RootModule rootModule = ModuleSystem.GetModule<RootModule>();
            if (rootModule == null)
            {
                Log.Fatal("Base component is invalid.");
                return;
            }

            AsyncInit().Forget();
        }

        private async UniTaskVoid AsyncInit()
        {
            Manager = gameObject.AddComponent<PlatformNativeManager>();
            await UniTask.WaitUntil(() => Manager.isInitFinish);
            Log.Debug("PlatformNativeManager init finish");
        }
    }
}