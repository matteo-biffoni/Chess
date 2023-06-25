using Unity.Networking.Transport;

namespace Net.NetMessages
{
    public sealed class NetLeftMatch : NetMessage
    {
        public int Team;
        
        public NetLeftMatch()
        {
            Code = OpCode.LeftMatch;
        }
        public NetLeftMatch(DataStreamReader reader)
        {
            Code = OpCode.LeftMatch;
            Deserialize(reader);
        }
        public override void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteInt(Team);
        }
        public override void Deserialize(DataStreamReader reader)
        {
            Team = reader.ReadInt();
        }
        public override void ReceivedOnClient()
        {
            NetUtility.CLeftMatch?.Invoke(this);
        }
        public override void ReceivedOnServer(NetworkConnection cnn)
        {
            NetUtility.SLeftMatch.Invoke(this, cnn);
        }
    }
}