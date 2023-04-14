using ChessPieces;
using Unity.Networking.Transport;

namespace Net.NetMessages
{
    public sealed class NetResolveConfrontation : NetMessage
    {
        public Outcome Outcome;
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
        }
        public override void Deserialize(DataStreamReader reader)
        {
            Outcome = (Outcome) reader.ReadByte();
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
