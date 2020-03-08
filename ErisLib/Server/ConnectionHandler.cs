using System;
using ErisLib.Server.Packets.Client;
using ErisLib.Server.Packets.Server;
using ErisLib.Utilities;

namespace ErisLib.Server
{
    public class ConnectionHandler
    {
        //cache
        private Proxy _proxy;

        public void Attach(Proxy proxy)
        {
            _proxy = proxy;
            proxy.HookPacket<HelloPacket>(OnHello);
            proxy.HookPacket<CreateSuccessPacket>(OnCreateSuccess);
        }

        private void OnCreateSuccess(Client client, CreateSuccessPacket packet)
        {
            client.SendToClient(PluginUtilities.CreateNotification(client.ObjectId, 0x00FFFF, "Welcome to Eris!"));
        }

        private void OnHello(Client client, HelloPacket packet)
        {
            client.State = _proxy.GetState(client, packet.Key);
            if (client.State.ConRealKey.Length != 0)
            {
                packet.Key = client.State.ConRealKey;
                client.State.ConRealKey = new byte[0];
                Console.WriteLine($"[HELLO PACKET] Packet Key: {packet.Key}\r\n" +
                                  $"               State  Key: {client.State.ConRealKey}");
            }
            client.Connect(packet);
            packet.Send = false;
        }
    }
}