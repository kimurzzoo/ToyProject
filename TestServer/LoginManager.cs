using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Backend
{
    class LoginManager
    {
        public static async Task LoginCheck(Socket clientSock, string connstr, byte[] buff, Dictionary<string, DataPacket.PositionStruct> UserPositions, Dictionary<Socket, string> UserMatching)
        {
            int offset = 4;
            int IDLength = DataPacket.ReadInt(buff, ref offset);
            Console.WriteLine("id length : " + IDLength.ToString());
            string IDstr = DataPacket.ReadString(buff, ref offset, IDLength);
            int PWLength = DataPacket.ReadInt(buff, ref offset);
            Console.WriteLine("pw length : " + PWLength.ToString());
            string PWstr = DataPacket.ReadString(buff, ref offset, PWLength);

            Console.WriteLine("id : " + IDstr + ", pw : " + PWstr);

            int loginresult = await LoginMatching(connstr, IDstr, PWstr);
            Console.WriteLine("login result : " + loginresult.ToString());

            if(loginresult == 1)
            {
                DataPacket.PositionStruct nowUserPosition = await LoadingPosition(connstr, IDstr);
                UserPositions.Add(IDstr, nowUserPosition);
                UserMatching.Add(clientSock, IDstr);
            }

            byte[] tempbuff = new byte[4096];
            int newoffset = 0;
            Console.WriteLine("header byte : " + ((int) DataPacket.HeaderByte.LoginCheckFlag).ToString());
            DataPacket.WriteInt(tempbuff, ref newoffset, (int) DataPacket.HeaderByte.LoginCheckFlag);
            DataPacket.WriteInt(tempbuff, ref newoffset, loginresult);

            await SocketTCP.SocketSend(clientSock, tempbuff);
        }

        public static async Task<int> LoginMatching(string thisconnstr, string IDText, string PwText)
        {
            int ret = 0;
            MySqlConnection thisconn = null;

            try
            {
                using(thisconn = new MySqlConnection(thisconnstr))
                {
                    thisconn.Open();
                    string cmd = "select * from user where ID = @IDText";
                    MySqlCommand newsqlcmd = new MySqlCommand(cmd, thisconn);
                    newsqlcmd.Parameters.AddWithValue("@IDText", IDText);

                    using (var newtable = await newsqlcmd.ExecuteReaderAsync())
                    {
                        while(newtable.Read())
                        {
                            if(newtable["PW"].ToString().Equals(PwText))
                            {
                                ret = 1;
                            }
                            else
                            {
                                ret = -1;
                            }
                        }
                        newtable.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            /*finally
            {
                if (thisconn != null)
                {
                    thisconn.Close();
                }
            }*/

            return ret;
        }

        public static async Task<DataPacket.PositionStruct> LoadingPosition(string thisconnstr, string IDText)
        {
            DataPacket.PositionStruct ret = new DataPacket.PositionStruct();
            MySqlConnection thisconn = null;

            try
            {
                using(thisconn = new MySqlConnection(thisconnstr))
                {
                    thisconn.Open();
                    string cmd = "select X,Y from position where ID = @IDText";
                    MySqlCommand newsqlcmd = new MySqlCommand(cmd, thisconn);
                    newsqlcmd.Parameters.AddWithValue("@IDText", IDText);

                    using (var newtable = await newsqlcmd.ExecuteReaderAsync())
                    {
                        while(newtable.Read())
                        {
                            ret.X = int.Parse(newtable["X"].ToString());
                            ret.Y = int.Parse(newtable["Y"].ToString());
                        }
                        newtable.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return ret;
        }
    }
}