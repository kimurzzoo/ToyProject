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
            int recvbyte;
            int offset;
            while(true)
            {
                recvbyte = -1;
                Array.Clear(buff, 0, buff.Length);
                Console.WriteLine("4");
                try
                {
                    recvbyte = await SocketTCP.SocketRecv(clientSock, buff);
                }
                catch(SocketException ex)
                {
                    break;
                }
                Console.WriteLine("5");

                if(recvbyte > 0)
                {
                    Console.WriteLine("6");
                    offset = 0;
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
            }
            
            
        }

        public static async Task<int> AfterSocketClosed(Socket clientSock)
        {
            int ret = 0;
            string userID = UserMatching[clientSock];
            Console.WriteLine(userID + " is closing the game now");
            UserMatching.Remove(clientSock);
            DataPacket.PositionStruct userPosition = UserPositions[userID];
            UserPositions.Remove(userID);
            MySqlConnection thisconn = null;

            try
            {
                using(thisconn = new MySqlConnection(strConn))
                {
                    thisconn.Open();
                    string cmd = "update position set X = @xvalue, Y = @yvalue where ID = @userID";
                    MySqlCommand newsqlcmd = new MySqlCommand(cmd, thisconn);
                    newsqlcmd.Parameters.AddWithValue("@xvalue", userPosition.X);
                    newsqlcmd.Parameters.AddWithValue("@yvalue", userPosition.Y);
                    newsqlcmd.Parameters.AddWithValue("@userID", userID);

                    int count = await newsqlcmd.ExecuteNonQueryAsync();

                    if(count == 1)
                    {
                        ret = 1;
                    }
                    else
                    {
                        ret = -1;
                    }
                }
            }
            catch(Exception ex)
            {
                throw;
            }

            return ret;
        } 
    }
}