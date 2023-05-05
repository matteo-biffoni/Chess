using ChessPieces;
using Unity.Networking.Transport;

namespace Net.NetMessages
{
    public sealed class NetResolveConfrontation : NetMessage
    {
        public Outcome Outcome;
        public uint NewDefendingHp;
        public NetResolveConfrontation()
        {
            Code = OpCode.ResolveConfrontation;
        }
        public NetResolveConfrontation(DataStreamReader reader)
        {
            Code = OpCode.ResolveConfrontation;
            Deserialize(reader);
        }
        public override void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteByte((byte)Outcome);
            writer.WriteUInt(NewDefendingHp);
        }
        public override void Deserialize(DataStreamReader reader)
        {
            Outcome = (Outcome) reader.ReadByte();
            NewDefendingHp = reader.ReadUInt();
        }
        public override void ReceivedOnClient()
        {
            NetUtility.CResolveConfrontation?.Invoke(this);
        }
        public override void ReceivedOnServer(NetworkConnection cnn)
        {
            NetUtility.SResolveConfrontation?.Invoke(this, cnn);
        }
    }
}
