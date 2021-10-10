using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Client
{
    public class LoginManage : MonoBehaviour
    {
        public void LoginClicked()
        {
            SocketTCP socketScript = GameObject.Find("SocketTCP").GetComponent<SocketTCP>();
            GameObject IDInputField = GameObject.Find("IDInputField");
            GameObject PwInputField = GameObject.Find("PwInputField");

            string IDText = IDInputField.transform.GetChild(2).GetComponent<Text>().text;
            string PwText = PwInputField.transform.GetChild(2).GetComponent<Text>().text;

            Debug.Log("ID : " + IDText + " , PW : " + PwText);

            byte[] buff = new byte[4096];

            int offset = 0;     

            DataStruct.HeaderByte header = DataStruct.HeaderByte.LoginCheckFlag;

            DataStruct.WriteInt(buff, ref offset, (int) header);

            DataStruct.WriteInt(buff, ref offset, Encoding.Unicode.GetByteCount(IDText));

            DataStruct.WriteString(buff, ref offset, IDText);

            DataStruct.WriteInt(buff, ref offset, Encoding.Unicode.GetByteCount(PwText));

            DataStruct.WriteString(buff, ref offset, PwText);

            SocketTCP.SocketSend(socketScript.GetSocket(), buff);
        }

        public static async Task LoginResult(byte[] buff)
        {
            int offset = 4;
            int result = DataStruct.ReadInt(buff, ref offset);

            if (result == 1)
            {
                Debug.Log("Login Complete");
                //AsyncTestClass testClass = new AsyncTestClass();
                //await testClass.LoadNewScene("GameScene");
                SceneManager.LoadScene("GameScene");
            }
            else if (result == 0)
            {
                Debug.Log("ID doesn't exist");
            }
            else if (result == -1)
            {
                Debug.Log("PW doesn't matching");
            }
            else
            {
                Debug.Log("Error");
            }
        }

        public void SignUpButtonClicked()
        {
            SceneManager.LoadScene("SignUpScene");
        }
    }
}

