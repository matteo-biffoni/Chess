using System;
using Net.NetMessages;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace Net
{
    public class Server : MonoBehaviour
    {
        #region Singleton implementation
        public static Server Instance { private set; get; }
        private void Awake()
        {
            Instance = this;
        }
        #endregion

        private NetworkDriver _driver;
        private NativeList<NetworkConnection> _connections;
        private bool _isActive;
        private const float KeepAliveTickRate = 20.0f;
        private float _lastKeepAlive;

        private Action _connectionDropped;
        public void Init(ushort port)
        {
            _driver = NetworkDriver.Create();
            var endpoint = NetworkEndPoint.AnyIpv4;
            endpoint.Port = port;
            if (_driver.Bind(endpoint) != 0)
            {
                Debug.Log("Unable to bind on port " + endpoint.Port);
                return;
            }
            _driver.Listen();
            Debug.Log("Currently listening on port " + endpoint.Port);
            _connections = new NativeList<NetworkConnection>(2, Allocator.Persistent);
            _isActive = true;
        }
        public void Shutdown()
        {
            if (_isActive)
            {
                _driver.Dispose();
                _connections.Dispose();
                _isActive = false;
            }
        }
        public void OnDestroy()
        {
            Shutdown();
        }
        public void Update()
        {
            if (!_isActive)
                return;
            KeepAlive();
            _driver.ScheduleUpdate().Complete();
            CleanupConnections();
            AcceptNewConnections();
            UpdateMessagePump();
        }
        private void CleanupConnections()
        {
            for (var i = 0; i < _connections.Length; i++)
            {
                if (!_connections[i].IsCreated)
                {
                    _connections.RemoveAtSwapBack(i);
                    --i;
                }
            }
        }
        private void KeepAlive()
        {
            if (Time.time - _lastKeepAlive > KeepAliveTickRate)
            {
                _lastKeepAlive = Time.time;
                Broadcast(new NetKeepAlive());
            }
        }
        private void AcceptNewConnections()
        {
            NetworkConnection c;
            while ((c = _driver.Accept()) != default)
            {
                _connections.Add(c);
            }
        }
        private void UpdateMessagePump()
        {
            for(var i = 0; i < _connections.Length; i++)
            {
                NetworkEvent.Type cmd;
                while ((cmd = _driver.PopEventForConnection(_connections[i], out var stream)) != NetworkEvent.Type.Empty)
                {
                    // ReSharper disable once ConvertIfStatementToSwitchStatement
                    if (cmd == NetworkEvent.Type.Data)
                    {
                        NetUtility.OnData(stream, _connections[i], this);
                    }
                    else if (cmd == NetworkEvent.Type.Disconnect)
                    {
                        Debug.Log("Client disconnected from server");
                        _connections[i] = default;
                        _connectionDropped?.Invoke();
                        Shutdown();
                    }
                }
            }
        }
        public void SendToClient(NetworkConnection connection, NetMessage msg)
        {
            _driver.BeginSend(connection, out var writer);
            msg.Serialize(ref writer);
            _driver.EndSend(writer);
        }
        public void Broadcast(NetMessage msg)
        {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var connection in _connections)
            {
                if (connection.IsCreated)
                {
                    Debug.Log($"Sending {msg.Code} to: {connection.InternalId}");
                    SendToClient(connection, msg);
                }
            }
        }

    }
}
