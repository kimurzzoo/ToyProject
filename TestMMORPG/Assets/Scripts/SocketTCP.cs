using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Client
{
    public class SocketTCP : MonoBehaviour
    {
        private Socket clientSock;
        private static SocketTCP instance;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
                return;
            }
            Destroy(this.gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            clientSock = SocketConnect();
            Task commutask = new Task(() => RecvCommunication());
            commutask.Start();
        }

        async void RecvCommunication()
        {
            while(true)
            {
                byte[] buff = new byte[4096];
                int recvbytes;
                if (clientSock.Connected)
                {
                    Array.Clear(buff, 0, buff.Length);
                    recvbytes = await SocketRecvAsync(clientSock, buff);

                    if (recvbytes > 0)
                    {
                        int offset = 0;
                        DataStruct.HeaderByte headerbyte = (DataStruct.HeaderByte)DataStruct.ReadInt(buff, ref offset);
                        switch (headerbyte)
                        {
                            case DataStruct.HeaderByte.CheckAlive:
                                {
                                    break;
                                }
                            case DataStruct.HeaderByte.ButtonPressed:
                                {
                                    break;
                                }
                            case DataStruct.HeaderByte.SendPosition:
                                {
                                    ReceivingPosition.CharacterPosition(buff);
                                    break;
                                }
                            case DataStruct.HeaderByte.LoginCheckFlag:
                                {
                                    Debug.Log("4");
                                    await LoginManage.LoginResult(buff);
                                    break;
                                }
                            case DataStruct.HeaderByte.SignUpDuplicationCheck:
                                {
                                    SignUpManage.IDDuplicationResult(buff);
                                    break;
                                }
                            case DataStruct.HeaderByte.CreateAccount:
                                {
                                    SignUpManage.CreateAccountResult(buff);
                                    break;
                                }
                        }
                    }
                }
            }
        }

        public static Socket SocketConnect()
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Loopback, 8765);
            sock.Connect(ep);

            return sock;
        }

        public static void SocketSend(Socket clientSock, byte[] buff)
        {
            clientSock.Send(buff);
        }

        public static int SocketRecv(Socket clientSock, byte[] buff)
        {
            int ret = clientSock.Receive(buff);
            return ret;
        }

        public static async Task<int> SocketRecvAsync(Socket clientSock, byte[] buff)
        {
            return await Task.Factory.FromAsync<int>(
                        clientSock.BeginReceive(buff, 0, buff.Length, SocketFlags.None, null, clientSock),
                        clientSock.EndReceive);
        }

        public static void closesocket(Socket clientsock)
        {
            clientsock.Close();
        }

        public Socket GetSocket()
        {
            return clientSock;
        }
    }
}