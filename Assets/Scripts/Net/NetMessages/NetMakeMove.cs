using Unity.Networking.Transport;

namespace Net.NetMessages
{
    public sealed class NetMakeMove : NetMessage
    {
        public int OriginalX;
        public int OriginalY;
        public int DestinationX;
        public int DestinationY;
        public int TeamID;
        public NetMakeMove()
        {
            Code = OpCode.MakeMove;
        }
        public NetMakeMove(DataStreamReader reader)
        {
            Code = OpCode.MakeMove;
            Deserialize(reader);
        }
        public override void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteInt(OriginalX);
            writer.WriteInt(OriginalY);
            writer.WriteInt(DestinationX);
            writer.WriteInt(DestinationY);
            writer.WriteInt(TeamID);
        }
        public override void Deserialize(DataStreamReader reader)
        {
            OriginalX = reader.ReadInt();
            OriginalY = reader.ReadInt();
            DestinationX = reader.ReadInt();
            DestinationY = reader.ReadInt();
            TeamID = reader.ReadInt();
        }
        public override void ReceivedOnClient()
        {
            NetUtility.CMakeMove?.Invoke(this);
        }
        public override void ReceivedOnServer(NetworkConnection cnn)
        {
            NetUtility.SMakeMove.Invoke(this, cnn);
        }
    }
}

