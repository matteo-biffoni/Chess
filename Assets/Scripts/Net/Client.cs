using System;
using Net.NetMessages;
using Unity.Networking.Transport;
using UnityEngine;

namespace Net
{
    public class Client : MonoBehaviour
    {
        #region Singleton implementation
        public static Client Instance { private set; get; }

        private void Awake()
        {
            Instance = this;
        }
        #endregion
        private NetworkDriver _driver;
        private NetworkConnection _connection;
        private bool _isActive;
        private Action _connectionDropped;
        public void Init(string ip, ushort port)
        {
            _driver = NetworkDriver.Create();
            var endpoint = NetworkEndPoint.Parse(ip, port);
            _connection = _driver.Connect(endpoint);
            Debug.Log("Attempting to connect to Server on " + endpoint.Address);
            _isActive = true;
            RegisterToEvent();
        }
        public void Shutdown()
        {
            if (_isActive)
            {
                UnregisterToEvent();
                _driver.Dispose();
                _isActive = false;
                _connection = default;
            }
        }
        public void OnDestroy()
        {
            Shutdown();
        }
        private void Update()
        {
            if(!_isActive)
                return;
            _driver.ScheduleUpdate().Complete();
            CheckAlive();
            UpdateMessagePump();
        }
        private void CheckAlive()
        {
            if (!_connection.IsCreated && _isActive)
            {
                Debug.Log("Something went wrong, lost connection to server");
                _connectionDropped?.Invoke();
                Shutdown();
            }
        }
        private void UpdateMessagePump()
        {
            NetworkEvent.Type cmd;
            while((cmd = _connection.PopEvent(_driver, out var stream)) != NetworkEvent.Type.Empty)
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (cmd == NetworkEvent.Type.Connect)
                {
                    SendToServer(new NetWelcome
                    {
                        //MatchConfiguration = MatchConfiguration.GetGameUIConfiguration()
                    });
                }
                else if (cmd == NetworkEvent.Type.Data)
                {
                    NetUtility.OnData(stream, default);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client got disconnected from server");
                    _connection = default;
                    _connectionDropped?.Invoke();
                    Shutdown();
                }
            }
        }
        public void SendToServer(NetMessage msg)
        {
            _driver.BeginSend(_connection, out var writer);
            msg.Serialize(ref writer);
            _driver.EndSend(writer);
        }
        private void RegisterToEvent()
        {
            NetUtility.CKeepAlive += OnKeepAlive;
        }
        private void UnregisterToEvent()
        {
            NetUtility.CKeepAlive -= OnKeepAlive;
        }
        private void OnKeepAlive(NetMessage nm)
        {
            SendToServer(nm);
        }
    }
}
