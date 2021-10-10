using System;
using MySql.Data.MySqlClient;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;

namespace Backend
{
    class Server
    {
        static int MAX_USER = 2000;
        public static Dictionary<Socket, string> UserMatching;
        static string strConn = "Server=localhost;Database=testdb;Uid=root;Pwd=A1s2d3f4g%;";
        public static Dictionary<string, DataPacket.PositionStruct> UserPositions;
        static void Main(string[] args)
        {
            MainServer().Wait();
        }

        static async Task MainServer()
        {
            UserPositions = new Dictionary<string, DataPacket.PositionStruct>();
            UserMatching = new Dictionary<Socket, string>();
            Socket mainSocket = SocketTCP.SocketStart();

            while(true)
            {
                Socket clientSock = await SocketTCP.SocketAccept(mainSocket);

                Task commutask = new Task( () => communication(clientSock));
                Console.WriteLine("3");
                commutask.Start();
            }
        }

        static async void communication(Socket clientSock)
        {
            byte[] buff = new byte[4096];
            Array.Clear(buff, 0, buff.Length);
            Console.WriteLine("4");
            int recvbyte = await SocketTCP.SocketRecv(clientSock, buff);
            Console.WriteLine("5");

            if(recvbyte > 0)
            {
                Console.WriteLine("6");
                int offset = 0;
                DataPacket.HeaderByte headerbyte = (DataPacket.HeaderByte) DataPacket.ReadInt(buff, ref offset);
                Console.WriteLine("7");
                switch(headerbyte)
                {
                    case DataPacket.HeaderByte.CheckAlive:
                    {
                        break;
                    }
                    case DataPacket.HeaderByte.ButtonPressed:
                    {
                        string playerID = UserMatching[clientSock];
                        Character.CharacterMoving(buff, playerID);
                        break;
                    }
                    case DataPacket.HeaderByte.SendPosition:
                    {
                        break;
                    }
                    case DataPacket.HeaderByte.LoginCheckFlag:
                    {
                        Console.WriteLine("8");
                        await LoginManager.LoginCheck(clientSock, strConn, buff, UserPositions, UserMatching);
                        Console.WriteLine("9");
                        break;
                    }
                    case DataPacket.HeaderByte.SignUpDuplicationCheck:
                    {
                        await SignUpManager.IDDuplicationCheck(clientSock, strConn, buff);
                        break;
                    }
                    case DataPacket.HeaderByte.CreateAccount:
                    {
                        await SignUpManager.CreatingAccount(clientSock, strConn, buff);
                        break;
                    }
                    case DataPacket.HeaderByte.GameStart:
                    {
                        string myID = "";
                        if(UserMatching.TryGetValue(clientSock, out myID))
                        {
                            Task sendingposition = new Task( () => SendPacket.SendingPosition(clientSock, myID, UserPositions) );
                            sendingposition.Start();
                        }
                        else
                        {
                            Console.WriteLine("user doesn't exist");
                        }
                        break;
                    }
                }
            }
            else
            {

            }
            Console.WriteLine("10");
            Task commutask = new Task( () => communication(clientSock));
            Console.WriteLine("11");
            commutask.Start();
        }
    }
}