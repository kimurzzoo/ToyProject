using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Client
{
    public class ButtonPress : MonoBehaviour
    {
        static Socket clientSock;

        void Start()
        {
            SocketTCP socketScript = GameObject.Find("SocketTCP").GetComponent<SocketTCP>();
            clientSock = socketScript.GetSocket();
        }

        public static void ArrowButtonPress(DataStruct.PressedButton input)
        {
            byte[] buff = new byte[4096];
            int header = (int)DataStruct.HeaderByte.ButtonPressed;
            int arrow = (int)input;

            Debug.Log("now button press : " + arrow.ToString());

            int offset = 0;

            DataStruct.WriteInt(buff, ref offset, header);
            DataStruct.WriteInt(buff, ref offset, arrow);

            SocketTCP.SocketSend(clientSock, buff);
        }
    }

}
