using System.Numerics;
using AssettoServer.Shared.Network.Packets;
using AssettoServer.Shared.Network.Packets.Outgoing;

namespace AssettoBallPlugin.Packets;

public class AssettoBallPosition : IOutgoingNetworkPacket
{
    public Vector3 Position { get; set; }
    public Vector3 Velocity { get; set; }

    public void ToWriter(ref PacketWriter writer)
    {
        writer.Write((byte)ACServerProtocol.Extended);
        writer.Write((byte)CSPMessageTypeTcp.ClientMessage);
        writer.Write<byte>(255);
        writer.Write<ushort>(60000);
        writer.Write(0xB4DC2D47);
        writer.Write(Position.X);
        writer.Write(Position.Y);
        writer.Write(Position.Z);
        writer.Write(Velocity.X);
        writer.Write(Velocity.Y);
        writer.Write(Velocity.Z);
    }
}
