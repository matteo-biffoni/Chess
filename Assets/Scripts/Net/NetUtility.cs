using System;
using Net.NetMessages;
using Unity.Networking.Transport;
using UnityEngine;
// ReSharper disable UnassignedField.Global

namespace Net
{
    public enum OpCode
    {
        KeepAlive = 1,
        Welcome = 2,
        StartGame = 3,
        MakeMove = 4,
        Rematch = 5,
        CreateConfrontation = 6,
        ResolveConfrontation = 7,
        MakeMoveInConfrontation = 8,
        NormalAttackInConfrontation = 9
    }
    public static class NetUtility
    {
        public static void OnData(DataStreamReader stream, NetworkConnection cnn, Server server = null)
        {
            NetMessage msg = null;
            var opCode = (OpCode)stream.ReadByte();
            switch (opCode)
            {
                case OpCode.KeepAlive:
                    msg = new NetKeepAlive(stream);
                    break;
                case OpCode.Welcome:
                    msg = new NetWelcome(stream);
                    break;
                case OpCode.StartGame:
                    msg = new NetStartGame(stream);
                    break;
                case OpCode.MakeMove:
                    msg = new NetMakeMove(stream);
                    break;
                case OpCode.Rematch:
                    msg = new NetRematch(stream);
                    break;
                case OpCode.CreateConfrontation:
                    msg = new NetCreateConfrontation(stream);
                    break;
                case OpCode.ResolveConfrontation:
                    msg = new NetResolveConfrontation(stream);
                    break;
                case OpCode.MakeMoveInConfrontation:
                    msg = new NetMakeMoveInConfrontation(stream);
                    break;
                case OpCode.NormalAttackInConfrontation:
                    msg = new NetNormalAttackInConfrontation(stream);
                    break;
                default:
                    Debug.LogError("Message received has no OpCode");
                    break;
            }
            if (msg == null)
            {
                Debug.LogError("Error with message");
                return;
            }
            if (server != null)
                msg.ReceivedOnServer(cnn);
            else
                msg.ReceivedOnClient();
        }
        public static Action<NetMessage> CKeepAlive;
        public static Action<NetMessage> CWelcome;
        public static Action<NetMessage> CStartGame;
        public static Action<NetMessage> CMakeMove;
        public static Action<NetMessage> CRematch;
        public static Action<NetMessage> CCreateConfrontation;
        public static Action<NetMessage> CResolveConfrontation;
        public static Action<NetMessage> CMakeMoveInConfrontation;
        public static Action<NetMessage> CNormalAttackInConfrontation;
        public static Action<NetMessage, NetworkConnection> SKeepAlive;
        public static Action<NetMessage, NetworkConnection> SWelcome;
        public static Action<NetMessage, NetworkConnection> SStartGame;
        public static Action<NetMessage, NetworkConnection> SMakeMove;
        public static Action<NetMessage, NetworkConnection> SRematch;
        public static Action<NetMessage, NetworkConnection> SCreateConfrontation;
        public static Action<NetMessage, NetworkConnection> SResolveConfrontation;
        public static Action<NetMessage, NetworkConnection> SMakeMoveInConfrontation;
        public static Action<NetMessage, NetworkConnection> SNormalAttackInConfrontation;
    }
}