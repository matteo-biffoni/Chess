using Unity.Networking.Transport;

namespace Net.NetMessages
{
    public sealed class NetMakeMoveInConfrontation : NetMessage
    {
        public int DestinationX;
        public int DestinationY;
        public NetMakeMoveInConfrontation()
        {
            Code = OpCode.MakeMoveInConfrontation;
        }
        public NetMakeMoveInConfrontation(DataStreamReader reader)
        {
            Code = OpCode.MakeMoveInConfrontation;
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
            NetUtility.CMakeMoveInConfrontation?.Invoke(this);
        }
        public override void ReceivedOnServer(NetworkConnection cnn)
        {
            NetUtility.SMakeMoveInConfrontation.Invoke(this, cnn);
        }
    }
}