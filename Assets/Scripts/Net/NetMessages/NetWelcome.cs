using Unity.Networking.Transport;
using UnityEngine;

namespace Net.NetMessages
{
    public sealed class NetWelcome : NetMessage
    {
        public int AssignedTeam { set; get; }
        //public MatchConfiguration MatchConfiguration { set; get; }
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
            //Debug.Log(MatchConfiguration);
            /*writer.WriteByte(MatchConfiguration.Attacking ? (byte) 0b00000001 : (byte) 0b00000000);
            if (MatchConfiguration.Attacking)
            {
                writer.WriteByte(MatchConfiguration.FullBoard ? (byte) 0b00000001 : (byte) 0b00000000);
                if (!MatchConfiguration.FullBoard)
                    writer.WriteByte((byte)MatchConfiguration.DispositionAttacking);
                writer.WriteByte(MatchConfiguration.MiniGame ? (byte) 0b00000001 : (byte) 0b00000000);
                writer.WriteByte(MatchConfiguration.OneMoveTurn ? (byte) 0b00000001 : (byte) 0b00000000);
            }
            else
            {
                writer.WriteByte((byte)MatchConfiguration.DispositionDefending);
            }*/
        }

        public override void Deserialize(DataStreamReader reader)
        {
            AssignedTeam = reader.ReadInt();
            /*var attacking = reader.ReadByte() != 0b00000000;
            if (attacking)
            {
                var fullBoard = reader.ReadByte() != 0b00000000;
                var dispositionAttacking = DispositionType.None;
                if (fullBoard)
                    dispositionAttacking = (DispositionType)reader.ReadByte();
                var miniGame = reader.ReadByte() != 0b00000000;
                var oneMoveTurn = reader.ReadByte() != 0b00000000;
                MatchConfiguration = new MatchConfiguration
                {
                    FullBoard = fullBoard,
                    DispositionAttacking = dispositionAttacking,
                    MiniGame = miniGame,
                    OneMoveTurn = oneMoveTurn
                };
            }
            else
            {
                var dispositionDefending = (DispositionType)reader.ReadByte();
                MatchConfiguration = new MatchConfiguration
                {
                    DispositionDefending = dispositionDefending
                };
            }*/
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