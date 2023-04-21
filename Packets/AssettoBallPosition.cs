using System.Numerics;
using AssettoServer.Network.Packets;
using AssettoServer.Network.Packets.Outgoing;

namespace AssettoBallPlugin.Packets;

public class AssettoBallPosition : IOutgoingNetworkPacket
{
    public Vector3 Position { get; set; }

    public void ToWriter(ref PacketWriter writer)
    {
        writer.Write((byte)ACServerProtocol.Extended);
        writer.Write((byte)CSPMessageTypeTcp.ClientMessage);
        writer.Write<byte>(255);
        writer.Write<ushort>(60000);
        writer.Write(0xD5137AF4);
        writer.Write(Position.X);
        writer.Write(Position.Y);
        writer.Write(Position.Z);
    }
}
