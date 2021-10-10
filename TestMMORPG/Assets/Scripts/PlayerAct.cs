using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class PlayerAct : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(Input.anyKeyDown)
            {
                if(Input.GetKeyDown(KeyCode.UpArrow))
                {
                    Debug.Log("up");
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    Debug.Log("down");
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    Debug.Log("left");
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    Debug.Log("right");
                }


                foreach (var dic in DataStruct.buttonDic)
                {
                    if(Input.GetKeyDown(dic.Key))
                    {
                        ButtonPress.ArrowButtonPress(dic.Value);
                    }
                }
            }
        }
    }
}
