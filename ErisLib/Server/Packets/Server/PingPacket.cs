using ErisLib.Server.Packets.Models;

namespace ErisLib.Server.Packets.Server
{
    public class PingPacket : Packet
    {
        public int Serial;

        public override PacketType Type => PacketType.PING;
        
        public override void Read(PacketReader r) => Serial = r.ReadInt32();
        
        public override void Write(PacketWriter w) => w.Write(Serial);
    }
}