using Unity.Networking.Transport;

namespace Net.NetMessages
{
    public sealed class NetRematch : NetMessage
    {
        public int TeamID;
        public byte WantRematch;
        public NetRematch()
        {
            Code = OpCode.Rematch;
        }
        public NetRematch(DataStreamReader reader)
        {
            Code = OpCode.Rematch;
            Deserialize(reader);
        }
        public override void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteInt(TeamID);
            writer.WriteByte(WantRematch);
        }
        public override void Deserialize(DataStreamReader reader)
        {
            TeamID = reader.ReadInt();
            WantRematch = reader.ReadByte();
        }
        public override void ReceivedOnClient()
        {
            //NetUtility.CRematch?.Invoke(this);
        }
        public override void ReceivedOnServer(NetworkConnection cnn)
        {
            //NetUtility.SRematch.Invoke(this, cnn);
        }
    }
}