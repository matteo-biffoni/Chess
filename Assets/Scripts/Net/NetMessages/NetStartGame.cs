using Unity.Networking.Transport;

namespace Net.NetMessages
{
    public sealed class NetStartGame : NetMessage
    {
        public NetStartGame()
        {
            Code = OpCode.StartGame;
        }
        public NetStartGame(DataStreamReader reader)
        {
            Code = OpCode.StartGame;
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
            NetUtility.CStartGame?.Invoke(this);
        }
        public override void ReceivedOnServer(NetworkConnection cnn)
        {
            NetUtility.SStartGame.Invoke(this, cnn);
        }
    }
}
