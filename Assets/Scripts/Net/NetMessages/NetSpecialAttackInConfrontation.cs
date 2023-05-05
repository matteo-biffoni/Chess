using ChessPieces;
using Unity.Networking.Transport;

namespace Net.NetMessages
{
    public sealed class NetSpecialAttackInConfrontation : NetMessage
    {
        public int CellX;
        public int CellY;
        public int SpecialAttackI;
        public NetSpecialAttackInConfrontation()
        {
            Code = OpCode.SpecialAttackInConfrontation;
        }
        public NetSpecialAttackInConfrontation(DataStreamReader reader)
        {
            Code = OpCode.SpecialAttackInConfrontation;
            Deserialize(reader);
        }
        public override void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteInt(CellX);
            writer.WriteInt(CellY);
            writer.WriteInt(SpecialAttackI);
        }
        public override void Deserialize(DataStreamReader reader)
        {
            CellX = reader.ReadInt();
            CellY = reader.ReadInt();
            SpecialAttackI = reader.ReadInt();
        }
        public override void ReceivedOnClient()
        {
            NetUtility.CSpecialAttackInConfrontation?.Invoke(this);
        }
        public override void ReceivedOnServer(NetworkConnection cnn)
        {
            NetUtility.SSpecialAttackInConfrontation.Invoke(this, cnn);
        }
    }
}