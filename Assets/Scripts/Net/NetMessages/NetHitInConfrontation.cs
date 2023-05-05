using Unity.Networking.Transport;

namespace Net.NetMessages
{
    public sealed class NetHitInConfrontation : NetMessage
    {
        public NetHitInConfrontation()
        {
            Code = OpCode.HitInConfrontation;
        }
        public NetHitInConfrontation(DataStreamReader reader)
        {
            Code = OpCode.HitInConfrontation;
            Deserialize(reader);
        }
        public override void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte)Code);
        }
        public override void Deserialize(DataStreamReader reader)
        {
        }
        public override void ReceivedOnClient()
        {
            NetUtility.CHitInConfrontation?.Invoke(this);
        }
        public override void ReceivedOnServer(NetworkConnection cnn)
        {
            NetUtility.SHitInConfrontation.Invoke(this, cnn);
        }
    }
}