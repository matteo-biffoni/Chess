using Unity.Networking.Transport;

namespace Net.NetMessages
{
    public sealed class NetNormalAttackInConfrontation : NetMessage
    {
        public int DestinationX;
        public int DestinationY;
        public NetNormalAttackInConfrontation()
        {
            Code = OpCode.NormalAttackInConfrontation;
        }
        public NetNormalAttackInConfrontation(DataStreamReader reader)
        {
            Code = OpCode.NormalAttackInConfrontation;
            Deserialize(reader);
        }
        public override void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteInt(DestinationX);
            writer.WriteInt(DestinationY);
        }
        public override void Deserialize(DataStreamReader reader)
        {
            DestinationX = reader.ReadInt();
            DestinationY = reader.ReadInt();
        }
        public override void ReceivedOnClient()
        {
            NetUtility.CNormalAttackInConfrontation?.Invoke(this);
        }
        public override void ReceivedOnServer(NetworkConnection cnn)
        {
            NetUtility.SNormalAttackInConfrontation.Invoke(this, cnn);
        }
    }
}