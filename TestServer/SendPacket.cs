using System;
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
            int offset;
            while(true)
            {
                offset = 0;
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

                try
                {
                    Console.WriteLine("sending to " + myID);
                    await SocketTCP.SocketSend(clientSock, buff);
                }
                catch(SocketException ex)
                {
                    Console.WriteLine("closing socket of " + myID);
                    await Server.AfterSocketClosed(clientSock);
                    break;
                }
                await Task.Delay(20);
            }
            Console.WriteLine("end send packet to " + myID);
        }
    }
}