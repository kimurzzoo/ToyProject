using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Client
{
    public class SignUpManage : MonoBehaviour
    {
        public void BackButtonClicked()
        {
            SceneManager.LoadScene("LoginScene");
        }

        public void IDDuplicationCheck()
        {
            SocketTCP socketScript = GameObject.Find("SocketTCP").GetComponent<SocketTCP>();
            GameObject IDInputField = GameObject.Find("IDInputField");

            string IDText = IDInputField.transform.GetChild(2).GetComponent<Text>().text;

            byte[] buff = new byte[4096];

            int offset = 0;

            DataStruct.HeaderByte header = DataStruct.HeaderByte.SignUpDuplicationCheck;

            DataStruct.WriteInt(buff, ref offset, (int)header);

            DataStruct.WriteInt(buff, ref offset, Encoding.Unicode.GetByteCount(IDText));

            DataStruct.WriteString(buff, ref offset, IDText);

            SocketTCP.SocketSend(socketScript.GetSocket(), buff);
        }

        public static void IDDuplicationResult(byte[] buff)
        {
            int offset = 4;
            int result = DataStruct.ReadInt(buff, ref offset);

            if (result == 1)
            {
                Debug.Log("check Complete");
            }
            else if (result == 0)
            {
                Debug.Log("check doesn't exist");
            }
            else if (result == -1)
            {
                Debug.Log("duplicated");
            }
            else
            {
                Debug.Log("Error");
            }
        }

        public void CreateAccount()
        {
            SocketTCP socketScript = GameObject.Find("SocketTCP").GetComponent<SocketTCP>();
            GameObject IDInputField = GameObject.Find("IDInputField");
            GameObject PwInputField = GameObject.Find("PwInputField");
            GameObject PwConfirmInputField = GameObject.Find("PwConfirmInputField");

            string IDText = IDInputField.transform.GetChild(2).GetComponent<Text>().text;
            string PwText = PwInputField.transform.GetChild(2).GetComponent<Text>().text;
            string PwConfirmText = PwConfirmInputField.transform.GetChild(2).GetComponent<Text>().text;

            if(!PwText.Equals(PwConfirmText))
            {
                Debug.Log("Confirmation isn't completed");
                return;
            }

            byte[] buff = new byte[4096];

            int offset = 0;

            DataStruct.HeaderByte header = DataStruct.HeaderByte.CreateAccount;

            DataStruct.WriteInt(buff, ref offset, (int)header);

            DataStruct.WriteInt(buff, ref offset, Encoding.Unicode.GetByteCount(IDText));

            DataStruct.WriteString(buff, ref offset, IDText);

            DataStruct.WriteInt(buff, ref offset, Encoding.Unicode.GetByteCount(PwText));

            DataStruct.WriteString(buff, ref offset, PwText);

            DataStruct.WriteInt(buff, ref offset, Encoding.Unicode.GetByteCount(PwConfirmText));

            DataStruct.WriteString(buff, ref offset, PwConfirmText);

            SocketTCP.SocketSend(socketScript.GetSocket(), buff);
        }

        public static void CreateAccountResult(byte[] buff)
        {
            int offset = 4;
            int result = DataStruct.ReadInt(buff, ref offset);

            if (result == 1)
            {
                Debug.Log("creating account Complete");
            }
            else if (result == 0)
            {
                Debug.Log("creating account doesn't exist");
            }
            else if (result == -1)
            {
                Debug.Log("duplicated");
            }
            else if(result == -2)
            {
                Debug.Log("creating isn't completed");
            }
            else
            {
                Debug.Log("Error");
            }
        }
    }
}

