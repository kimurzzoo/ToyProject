using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend
{
    class SendPacket
    {
        public static async void SendingPosition(Socket clientSock, string myID, Dictionary<string, DataPacket.PositionStruct> UserPositions)
        {
            byte[] buff = new byte[4096];
            int offset = 0;
            DataPacket.WriteInt(buff, ref offset, (int) DataPacket.HeaderByte.SendPosition);
            DataPacket.WriteByteArray(buff, ref offset, DataPacket.RawSerialize(UserPositions[myID]));
            DataPacket.WriteInt(buff, ref offset, UserPositions.Count - 1);
            foreach(KeyValuePair<string, DataPacket.PositionStruct> kvp in UserPositions)
            {
                if(!kvp.Key.Equals(myID))
                {
                    DataPacket.WriteInt(buff, ref offset, Encoding.Unicode.GetByteCount(kvp.Key));
                    DataPacket.WriteString(buff, ref offset, kvp.Key);
                    DataPacket.WriteByteArray(buff, ref offset, DataPacket.RawSerialize(kvp.Value));
                }
            }

            await SocketTCP.SocketSend(clientSock, buff);

            Task sendingposition = new Task( () => SendPacket.SendingPosition(clientSock, myID, UserPositions) );
            sendingposition.Start();
        }
    }
}