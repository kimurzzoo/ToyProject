using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Backend
{
    class SignUpManager
    {
        public static async Task IDDuplicationCheck(Socket clientSock, string connstr, byte[] buff)
        {
            int offset = 4;
            int IDLength = DataPacket.ReadInt(buff, ref offset);
            string IDstr = DataPacket.ReadString(buff, ref offset, IDLength);

            int duplicationresult = await IDDuplication(connstr, IDstr);

            byte[] tempbuff = new byte[4096];
            int newoffset = 0;
            DataPacket.WriteInt(tempbuff, ref newoffset, (int) DataPacket.HeaderByte.SignUpDuplicationCheck);
            DataPacket.WriteInt(tempbuff, ref newoffset, duplicationresult);
            await SocketTCP.SocketSend(clientSock, tempbuff);
        }

        public static async Task CreatingAccount(Socket clientSock, string connstr, byte[] buff)
        {
            int offset = 4;
            int IDLength = DataPacket.ReadInt(buff, ref offset);
            
            string IDstr = DataPacket.ReadString(buff, ref offset, IDLength);

            int PWLength = DataPacket.ReadInt(buff, ref offset);

            string PWstr = DataPacket.ReadString(buff, ref offset, PWLength);

            int PWCLength = DataPacket.ReadInt(buff, ref offset);

            string PWCstr = DataPacket.ReadString(buff, ref offset, PWCLength);

            int creatingaccountresult = await CreatingAccountCheck(connstr, IDstr, PWstr, PWCstr);

            if(creatingaccountresult == 1)
            {
                await CreatingPosition(connstr, IDstr);
            }

            byte[] tempbuff = new byte[4096];
            int newoffset = 0;
            DataPacket.WriteInt(tempbuff, ref newoffset, (int) DataPacket.HeaderByte.CreateAccount);
            DataPacket.WriteInt(tempbuff, ref newoffset, creatingaccountresult);

            await SocketTCP.SocketSend(clientSock, tempbuff);
        }

        public static async Task<int> IDDuplication(string thisconnstr, string IDText)
        {
            int ret = 0;
            MySqlConnection thisconn = null;

            try
            {
                using(thisconn = new MySqlConnection(thisconnstr))
                {
                    thisconn.Open();
                    string cmd = "select count(*) from user where ID = @IDText";
                    MySqlCommand newsqlcmd = new MySqlCommand(cmd, thisconn);
                    newsqlcmd.Parameters.AddWithValue("@IDText", IDText);

                    int count = int.Parse((await newsqlcmd.ExecuteScalarAsync()).ToString());

                    if(count >= 1)
                    {
                        ret = -1;
                    }
                    else
                    {
                        ret = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            
            return ret;
        }

        public static async Task<int> CreatingAccountCheck(string thisconnstr, string IDText, string PWText, string PWCText)
        {
            int ret = 0;
            MySqlConnection thisconn = null;

            if(!PWText.Equals(PWCText))
            {
                return ret;
            }

            int duplicate = await IDDuplication(thisconnstr, IDText);

            if(duplicate == 1)
            {
                try
                {
                    using(thisconn = new MySqlConnection(thisconnstr))
                    {
                        thisconn.Open();
                        string cmd = "insert into user values (@IDText, @PWText);";
                        MySqlCommand newsqlcmd = new MySqlCommand(cmd, thisconn);
                        newsqlcmd.Parameters.AddWithValue("@IDText", IDText);
                        newsqlcmd.Parameters.AddWithValue("@PWText", PWText);

                        int count = await newsqlcmd.ExecuteNonQueryAsync();

                        if(count == 1)
                        {
                            ret = 1;
                        }
                        else
                        {
                            ret = -2;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            else
            {
                ret = -1;
            }

            return ret;
        }

        static async Task<int> CreatingPosition(string thisconnstr, string IDText)
        {
            int ret = 0;
            MySqlConnection thisconn = null;

            try
            {
                using(thisconn = new MySqlConnection(thisconnstr))
                {
                    thisconn.Open();
                    string cmd = "insert into position values (@IDText, 50, 50);";
                    MySqlCommand newsqlcmd = new MySqlCommand(cmd, thisconn);
                    newsqlcmd.Parameters.AddWithValue("@IDText", IDText);

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
            catch (Exception ex)
            {
                throw;
            }

            return ret;
        }
    }
}