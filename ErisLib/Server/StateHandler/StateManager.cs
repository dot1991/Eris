using System;
using System.Collections.Generic;
using System.Linq;
using ErisLib.Server.Packets.Client;
using ErisLib.Server.Packets.Models;
using ErisLib.Server.Packets.Server;

namespace ErisLib.Server.StateHandler
{
    public class StateManager
    {
        private Proxy _proxy;

        public void Attach(Proxy proxy)
        {
            _proxy = proxy;
            proxy.HookPacket<CreateSuccessPacket>(OnCreateSuccess);
            proxy.HookPacket<MapInfoPacket>(OnMapInfo);
            proxy.HookPacket<UpdatePacket>(OnUpdate);
            proxy.HookPacket<NewTickPacket>(OnNewTick);
            proxy.HookPacket<PlayerShootPacket>(OnPlayerShoot);
            proxy.HookPacket<MovePacket>(OnMove);
        }

        private void OnMove(Client client, MovePacket packet)
        {
            client.PreviousTime = packet.Time;
            client.LastUpdate = Environment.TickCount;
            client.PlayerData.Pos = packet.NewPosition;
        }

        private void OnPlayerShoot(Client client, PlayerShootPacket packet)
        {
            client.PlayerData.Pos = new Location()
            {
                X = packet.Position.X - 0.3f * (float)Math.Cos(packet.Angle),
                Y = packet.Position.Y - 0.3f * (float)Math.Sin(packet.Angle)
            };
        }

        private void OnNewTick(Client client, NewTickPacket packet)
        {
            client.PlayerData.Parse(packet);
        }

        private void OnMapInfo(Client client, MapInfoPacket packet)
        {
            client.State["MapInfo"] = packet;
        }

        private void OnCreateSuccess(Client client, CreateSuccessPacket packet)
        {
            client.PlayerData = new PlayerData(packet.ObjectId, client.State.Value<MapInfoPacket>("MapInfo"));
        }

        private void OnUpdate(Client client, UpdatePacket packet)
        {
            client.PlayerData.Parse(packet);
            if (client.State.ACCID != null) return;

            State resolvedState = null;

            foreach (var cstate in _proxy.States.Values.Where(cstate => cstate.ACCID == client.PlayerData.AccountId))
                resolvedState = cstate;

            if (resolvedState == null)
                client.State.ACCID = client.PlayerData.AccountId;
            else
            {   
                // Info: I use a "cache" here in order to not make the application throw a "Collection Modified" exception and halt execution
                Dictionary<string, dynamic> cache = client.State.States.ToDictionary(pair => pair.Key, pair => pair.Value);

                foreach(var pair in cache) 
                    resolvedState.States[pair.Key] = pair.Value;

                client.State = resolvedState;
            }
        }
    }
}