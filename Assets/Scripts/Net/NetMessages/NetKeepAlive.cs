using Unity.Networking.Transport;

namespace Net.NetMessages
{
    public sealed class NetKeepAlive : NetMessage
    {
        public NetKeepAlive()
        {
            Code = OpCode.KeepAlive;
        }
        public NetKeepAlive(DataStreamReader reader)
        {
            Code = OpCode.KeepAlive;
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
            NetUtility.CKeepAlive?.Invoke(this);
        }
        public override void ReceivedOnServer(NetworkConnection cnn)
        {
            NetUtility.SKeepAlive?.Invoke(this, cnn);
        }
    }
}
