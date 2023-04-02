using Unity.Networking.Transport;

namespace Net.NetMessages
{
    public sealed class NetWelcome : NetMessage
    {
        public int AssignedTeam { set; get; }
        public NetWelcome()
        {
            Code = OpCode.Welcome;
        }
        public NetWelcome(DataStreamReader reader)
        {
            Code = OpCode.Welcome;
            Deserialize(reader);
        }
        public override void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteInt(AssignedTeam);
        }
        public override void Deserialize(DataStreamReader reader)
        {
            AssignedTeam = reader.ReadInt();
        }
        public override void ReceivedOnClient()
        {
            NetUtility.CWelcome?.Invoke(this);
        }
        public override void ReceivedOnServer(NetworkConnection cnn)
        {
            NetUtility.SWelcome.Invoke(this, cnn);
        }
    }
}