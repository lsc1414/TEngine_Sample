using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class GameTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var item = ConfigSystem.Instance.Tables.TbItem.Get(0);
            Debug.Log(item.Id+item.Name+item.Desc);
                //Debug.Log(ConfigSystem.Instance.Tables.TbtextInfo.Get("Hair").En);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
