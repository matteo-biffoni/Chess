using ChessPieces;
using Unity.Networking.Transport;
using UnityEngine;

namespace Net.NetMessages
{
    public sealed class NetCreateConfrontation : NetMessage
    {
        public ChessPieceType AttackingType;
        public int AttackingX;
        public int AttackingY;
        public ChessPieceType DefendingType;
        public int DefendingX;
        public int DefendingY;
        public NetCreateConfrontation()
        {
            Code = OpCode.CreateConfrontation;
        }
        public NetCreateConfrontation(DataStreamReader reader)
        {
            Code = OpCode.CreateConfrontation;
            Deserialize(reader);
        }
        public override void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteByte((byte)AttackingType);
            writer.WriteInt(AttackingX);
            writer.WriteInt(AttackingY);
            writer.WriteByte((byte)DefendingType);
            writer.WriteInt(DefendingX);
            writer.WriteInt(DefendingY);
        }
        public override void Deserialize(DataStreamReader reader)
        {
            AttackingType = (ChessPieceType) reader.ReadByte();
            AttackingX = reader.ReadInt();
            AttackingY = reader.ReadInt();
            DefendingType = (ChessPieceType) reader.ReadByte();
            DefendingX = reader.ReadInt();
            DefendingY = reader.ReadInt();
        }
        public override void ReceivedOnClient()
        {
            NetUtility.CCreateConfrontation?.Invoke(this);
        }
        public override void ReceivedOnServer(NetworkConnection cnn)
        {
            NetUtility.SCreateConfrontation?.Invoke(this, cnn);
        }
    }
}
