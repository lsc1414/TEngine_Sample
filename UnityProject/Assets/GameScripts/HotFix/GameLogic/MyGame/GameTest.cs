using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class GameTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var item = ConfigSystem.Instance.Tables.TbItem.Get(10000);
            Log.Debug(item.Id+item.Name+item.Desc);
                //Debug.Log(ConfigSystem.Instance.Tables.TbtextInfo.Get("Hair").En);
                
            GameModule.UI.ShowUI<VersionGO>();
        }

        // Update is called once per frame
        void Update()
        {
            transform.eulerAngles+=Vector3.left*(Time.deltaTime*100);
        }
    }
}
