﻿using ErisLib.Server.Packets.Models;

namespace ErisLib.Server.Packets.Server
{
    public class CreateGuildResultPacket : Packet
    {
        public bool Success;
        public string ErrorText;
        public override PacketType Type => PacketType.CREATEGUILDRESULT;

        public override void Read(PacketReader r)
        {
            Success = r.ReadBoolean();
            ErrorText = r.ReadString();
        }

        public override void Write(PacketWriter w)
        {
            w.Write(Success);
            w.Write(ErrorText);
        }
    }
}
