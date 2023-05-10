using Unity.Networking.Transport;

namespace Net.NetMessages
{
    public sealed class NetMatchConfiguration : NetMessage
    {
        public MatchConfiguration MatchConfiguration;
        public NetMatchConfiguration()
        {
            Code = OpCode.MatchConfiguration;
        }
        public NetMatchConfiguration(DataStreamReader reader)
        {
            Code = OpCode.MatchConfiguration;
            Deserialize(reader);
        }
        public override void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte)Code);
            var attacking = MatchConfiguration.Attacking;
            writer.WriteInt(attacking ? 1 : 0);
            if (attacking)
            {
                var fullBoard = MatchConfiguration.FullBoard;
                writer.WriteInt(fullBoard ? 1 : 0);
                if (!fullBoard)
                {
                    writer.WriteByte((byte)MatchConfiguration.DispositionAttacking);
                }
                writer.WriteInt(MatchConfiguration.Turns ? 1 : 0);
                writer.WriteInt(MatchConfiguration.MiniGame ? 1 : 0);
            }
            else
            {
                writer.WriteByte((byte)MatchConfiguration.DispositionDefending);
            }
        }
        public override void Deserialize(DataStreamReader reader)
        {
            var attacking = reader.ReadInt();
            if (attacking != 0)
            {
                var fullBoard = reader.ReadInt() != 0;
                var dispositionAttacking = DispositionType.None;
                if (!fullBoard)
                {
                    dispositionAttacking = (DispositionType)reader.ReadByte();
                }
                var turns = reader.ReadInt() != 0;
                var miniGame = reader.ReadInt() != 0;
                MatchConfiguration = new MatchConfiguration()
                {
                    Attacking = true,
                    FullBoard = fullBoard,
                    DispositionAttacking = dispositionAttacking,
                    Turns = turns,
                    MiniGame = miniGame
                };
            }
            else
            {
                var dispositionDefending = (DispositionType)reader.ReadByte();
                MatchConfiguration = new MatchConfiguration
                {
                    Attacking = false,
                    DispositionDefending = dispositionDefending
                };
            }
        }
        public override void ReceivedOnClient()
        {
            NetUtility.CMatchConfiguration?.Invoke(this);
        }
        public override void ReceivedOnServer(NetworkConnection cnn)
        {
            NetUtility.SMatchConfiguration.Invoke(this, cnn);
        }
    }
}