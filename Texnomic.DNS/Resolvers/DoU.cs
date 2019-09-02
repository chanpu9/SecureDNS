﻿using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Resolvers
{
    public class DoU : IResolver
    {
        private IPEndPoint IPEndPoint;
        private readonly UdpClient Client;

        public DoU(IPAddress IPAddress)
        {
            IPEndPoint = new IPEndPoint(IPAddress, 53);
            Client = new UdpClient
            {
                Client =
                {
                    SendTimeout = 2000, 
                    ReceiveTimeout = 2000
                }
            };
        }

        public void Dispose()
        {
            Client.Dispose();
        }

        public byte[] Resolve(byte[] Query)
        {
            Client.Send(Query, Query.Length, IPEndPoint);

            return Client.Receive(ref IPEndPoint);
        }

        public Message Resolve(Message Query)
        {
            var Buffer = Query.ToArray();

            Client.Send(Buffer, Buffer.Length, IPEndPoint);

            Buffer = Client.Receive(ref IPEndPoint);

            return Message.FromArray(Buffer);
        }

        public async ValueTask<byte[]> ResolveAsync(byte[] Query)
        {
            await Client.SendAsync(Query, Query.Length, IPEndPoint);

            var Result = await Client.ReceiveAsync();

            return Result.Buffer;
        }

        public async ValueTask<Message> ResolveAsync(Message Query)
        {
            var Buffer = Query.ToArray();

            await Client.SendAsync(Buffer, Buffer.Length, IPEndPoint);

            var Result = await Client.ReceiveAsync();

            return Message.FromArray(Result.Buffer);
        }
    }
}
