using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class GameStartManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            SocketTCP socketScript = GameObject.Find("SocketTCP").GetComponent<SocketTCP>();

            byte[] buff = new byte[4096];
            int offset = 0;
            DataStruct.HeaderByte header = DataStruct.HeaderByte.GameStart;
            DataStruct.WriteInt(buff, ref offset, (int)header);
            SocketTCP.SocketSend(socketScript.GetSocket(), buff);
        }
    }
}
