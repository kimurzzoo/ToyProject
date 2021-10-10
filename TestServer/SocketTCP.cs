using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Backend
{
    class SocketTCP
    {
        public static Socket SocketStart()
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 8765);
            sock.Bind(ep);
    
            sock.Listen(100);

            return sock;
        }

        public static async Task<Socket> SocketAccept(Socket sock)
        {
            Socket clientSock = await Task.Factory.FromAsync(sock.BeginAccept, sock.EndAccept, null);
            return clientSock;
        }

        public static async Task<int> SocketRecv(Socket clientSock, byte[] buff)
        {
            return await Task.Factory.FromAsync<int>(
                        clientSock.BeginReceive(buff, 0, buff.Length, SocketFlags.None, null, clientSock),
                        clientSock.EndReceive);
        }

        public static async Task SocketSend(Socket clientSock, byte[] buff)
        {
            await Task.Factory.FromAsync(
                            clientSock.BeginSend(buff, 0, buff.Length, SocketFlags.None, null, clientSock),
                            clientSock.EndSend);
        }

        public static void closesocket(Socket clientsock)
        {
            clientsock.Close();
        }
    }
}