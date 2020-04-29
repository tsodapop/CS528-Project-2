﻿/**************************************************************************************************
* THE OMICRON PROJECT
 *-------------------------------------------------------------------------------------------------
 * Copyright 2010-2018		Electronic Visualization Laboratory, University of Illinois at Chicago
 * Authors:										
 *  Arthur Nishimoto		anishimoto42@gmail.com
 *-------------------------------------------------------------------------------------------------
 * Copyright (c) 2010-2018, Electronic Visualization Laboratory, University of Illinois at Chicago
 * All rights reserved.
 * Redistribution and use in source and binary forms, with or without modification, are permitted 
 * provided that the following conditions are met:
 * 
 * Redistributions of source code must retain the above copyright notice, this list of conditions 
 * and the following disclaimer. Redistributions in binary form must reproduce the above copyright 
 * notice, this list of conditions and the following disclaimer in the documentation and/or other 
 * materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR 
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND 
 * FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE  GOODS OR SERVICES; LOSS OF 
 * USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
 * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *************************************************************************************************/
 
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

#if USING_GETREAL3D
public class CAVE2RPCManager : getReal3D.MonoBehaviourWithRpc
{
#else
public class CAVE2RPCManager : MonoBehaviour {
#endif

    // Cluster Sync
    public int cave2RPCCallCount;

    [SerializeField]
    bool debugRPC = false;

    // Remote Networking
    HostTopology topology;
    [SerializeField] int hostId;
    [SerializeField] int connectionId;

    HashSet<int> clientIDs = new HashSet<int>();

    [Header("Message Server")]
    [SerializeField]
    public bool useMsgServer;

    static short MessageID = 1104;
    NetworkServerSimple msgServer;
    NetworkMessageDelegate serverOnClientConnect;
    NetworkMessageDelegate serverOnClientDisconnect;
    NetworkMessageDelegate serverOnData;

    [SerializeField]
    int serverListenPort = 9105;

    [SerializeField]
    int reliableChannelId;

    [SerializeField]
    int unreliableChannelId;

    [SerializeField]
    int stateUpdateChannelId;

    [SerializeField]
    int maxConnections = 100;

    public enum MsgType { Reliable, Unreliable, StateUpdate };

    [Header("Message Client")]
    [SerializeField]
    public bool useMsgClient;

    NetworkClient msgClient;
    NetworkMessageDelegate clientOnConnect;
    NetworkMessageDelegate clientOnDisconnect;
    NetworkMessageDelegate clientOnData;

    [SerializeField]
    string serverIP;

    [SerializeField]
    bool debugMsg;

    bool connected = false;

    [SerializeField]
    bool autoReconnect = true;

    [SerializeField]
    float autoReconnectDelay = 5;

    float autoReconnectTimer;
    int reconnectAttemptCount;

    [SerializeField]
    RemoteTerminal remoteTerminal;

    string defaultTargetObjectName;

    private void LogUI(string msg)
    {
        if (remoteTerminal)
            remoteTerminal.PrintUI(msg);
        else
            Debug.Log(msg);
    }

    private void Start()
    {
        /*
        msgServer = new NetworkServerSimple();
        msgClient = new NetworkClient();

        ConnectionConfig myConfig = new ConnectionConfig();
        reliableChannelId = myConfig.AddChannel(QosType.Reliable);
        unreliableChannelId = myConfig.AddChannel(QosType.Unreliable);

        msgServer.Configure(myConfig, maxConnections);
        msgClient.Configure(myConfig, maxConnections);
        */
        SetupNetworking();

        if (useMsgServer)
        {
            StartNetServer();
        }
        if (useMsgClient)
        {
            StartNetClient();
        }
    }

    private void Update()
    {
        /*
        msgServer.Update();

        if(useMsgClient && autoReconnect && !connected)
        {
            if (autoReconnectTimer < autoReconnectDelay)
            {
                autoReconnectTimer += Time.deltaTime;
            }
            else
            {
                reconnectAttemptCount++;
                LogUI("Msg Client: Reconnecting to server " + serverIP + ":" + serverListenPort + " (Attempt: " + reconnectAttemptCount + ")");
                msgClient.Disconnect();
                msgClient.Connect(serverIP, serverListenPort);

                autoReconnectTimer = 0;
            }
        }
        */

        UpdateNetwork();
    }

    public bool IsReconnecting()
    {
        return (reconnectAttemptCount > 0);
    }

    private void SetupNetworking()
    {
        GlobalConfig gConfig = new GlobalConfig();
        //gConfig.MaxPacketSize = 500;

        NetworkTransport.Init(gConfig);

        ConnectionConfig config = new ConnectionConfig();
        reliableChannelId = config.AddChannel(QosType.Reliable);
        unreliableChannelId = config.AddChannel(QosType.Unreliable);
        stateUpdateChannelId = config.AddChannel(QosType.StateUpdate);

        topology = new HostTopology(config, maxConnections);
        hostId = NetworkTransport.AddHost(topology, serverListenPort);
    }

    private void StartNetServer()
    {
        /*
        serverOnClientConnect += ServerOnClientConnect;
        serverOnClientDisconnect += ServerOnClientDisconnect;
        serverOnData += ServerOnRecvMsg;

        msgServer.RegisterHandler(MsgType.Connect, serverOnClientConnect);
        msgServer.RegisterHandler(MsgType.Disconnect, serverOnClientDisconnect);
        msgServer.RegisterHandler(MessageID, serverOnData);
        
        msgServer.Listen(serverListenPort);
        LogUI("Starting message server on port " + serverListenPort);
        */
    }

    private void StartNetClient()
    {
        LogUI("Msg Client: Connecting to server " + serverIP + ":" + serverListenPort);

        byte error;
        connectionId = NetworkTransport.Connect(hostId, serverIP, serverListenPort, 0, out error);

        /*
        msgClient.Connect(serverIP, serverListenPort);

        clientOnConnect += ClientOnConnect;
        clientOnDisconnect += ClientOnDisconnect;
        clientOnData += ClientOnRecvMsg;

        msgClient.RegisterHandler(MsgType.Connect, clientOnConnect);
        msgClient.RegisterHandler(MsgType.Disconnect, clientOnDisconnect);
        msgClient.RegisterHandler(MessageID, clientOnData);
        */
    }

    void UpdateNetwork()
    {
        NetworkEventType recData;
        do
        {
            int recHostId;
            int recConnectionId;
            int channelId;
            int bufferSize = 1024;
            byte[] recBuffer = new byte[bufferSize];
            int dataSize;
            byte error;
            recData = NetworkTransport.Receive(out recHostId, out recConnectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
            switch (recData)
            {
                case NetworkEventType.Nothing: break;
                case NetworkEventType.ConnectEvent:
                    Debug.Log("ConnectEvent");
                    if (connectionId == recConnectionId)
                    {
                        ClientOnConnect();
                    }
                    else
                    {
                        ServerOnClientConnect(recConnectionId);
                    }
                    break;
                case NetworkEventType.DataEvent:
                    NetworkReader networkReader = new NetworkReader(recBuffer);

                    byte[] readerMsgSizeData = networkReader.ReadBytes(2);
                    short readerMsgSize = (short)((readerMsgSizeData[1] << 8) + readerMsgSizeData[0]);

                    byte[] readerMsgTypeData = networkReader.ReadBytes(2);
                    short readerMsgType = (short)((readerMsgTypeData[1] << 8) + readerMsgTypeData[0]);

                    string targetObjectName = networkReader.ReadString();
                    string methodName = networkReader.ReadString();
                    int paramCount = networkReader.ReadInt32();

                    switch (readerMsgType)
                    {
                        case (101): BroadcastCAVE2RPC(targetObjectName, methodName, ReaderToObject(networkReader)); break;
                        case (102): BroadcastCAVE2RPC4(targetObjectName, methodName, ReaderToObject(networkReader), ReaderToObject(networkReader)); break;
                        case (201): SendCAVE2RPC(targetObjectName, methodName, ReaderToObject(networkReader)); break;
                        case (202):
                            SendCAVE2RPC4(targetObjectName, methodName,
                    ReaderToObject(networkReader),
                    ReaderToObject(networkReader)); break;
                        case (203):
                            SendCAVE2RPC5(targetObjectName, methodName,
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader)); break;
                        case (204):
                            SendCAVE2RPC6(targetObjectName, methodName,
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader)); break;
                        case (205):
                            SendCAVE2RPC7(targetObjectName, methodName,
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader)); break;
                        case (207):
                            SendCAVE2RPC9(targetObjectName, methodName,
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader)); break;
                        case (216):
                            SendCAVE2RPC18(targetObjectName, methodName,
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader),
                            ReaderToObject(networkReader)); break;
                    }
                    break;
                case NetworkEventType.DisconnectEvent:
                    Debug.Log("DisconnectEvent");
                    if (connectionId != recConnectionId)
                    {
                        ServerOnClientDisconnect(recConnectionId);
                    }
                    break;
                case NetworkEventType.BroadcastEvent:
                    Debug.Log("BroadcastEvent");
                    break;
            }

        } while (recData != NetworkEventType.Nothing);
    }

    void ServerOnClientConnect(int clientConnectionId)
    {
        LogUI("Msg Server: Client " + clientConnectionId  + " connected.");
        clientIDs.Add(clientConnectionId);
    }

    void ServerOnClientDisconnect(int clientConnectionId)
    {
        LogUI("Msg Server: Client " + clientConnectionId + " disconnected.");
        clientIDs.Remove(clientConnectionId);
    }

    void ClientOnConnect()
    {
        LogUI("Msg Client: Connected to " + serverIP);
        connected = true;
        reconnectAttemptCount = 0;
    }

    void ClientOnDisconnect(NetworkMessage msg)
    {
        LogUI("Msg Client: Disconnected");
        connected = false;
    }

    void ServerSendMsgToClients(byte[] writerData, MsgType msgType)
    {
        int channelId = reliableChannelId;
        switch(msgType)
        {
            case (MsgType.Reliable): channelId = reliableChannelId; break;
            case (MsgType.Unreliable): channelId = unreliableChannelId; break;
            case (MsgType.StateUpdate): channelId = stateUpdateChannelId; break;
        }

        foreach (int clientId in clientIDs)
        {
            byte error;
            NetworkTransport.Send(hostId, clientId, channelId, writerData, writerData.Length, out error);
        }

        /*
        System.Collections.ObjectModel.ReadOnlyCollection<NetworkConnection> connections = msgServer.connections;

        foreach (NetworkConnection client in connections)
        {
            if (client != null)
            {
                NetworkWriter writer = new NetworkWriter();
                writer.StartMessage(MessageID);
                writer.Write(msgStr);
                writer.FinishMessage();
                if(debugMsg)
                    LogUI("sending: " + msgStr);
                msgServer.SendWriterTo(client.connectionId, writer, useReliable ? reliableChannelId : unreliableChannelId);
            }
        }
        */
    }

    void ClientOnRecvMsg(NetworkMessage msg)
    {
        ProcessMsg(msg);
    }

    void ServerOnRecvMsg(NetworkMessage msg)
    {
        // Reset reader index
        msg.reader.SeekZero();

        string msgString = msg.reader.ReadString();
        if (debugMsg)
            LogUI("Msg Server: ServerOnRecvMsg '" + msgString + "'");
        ProcessMsg(msg);

        // Forward message to clients (except sender client);
        //ServerSendMsgToClients(msgString, true, msg.conn);
    }

    public void ProcessMsg(NetworkMessage msg)
    {
        // Reset reader index
        msg.reader.SeekZero();

        string msgString = msg.reader.ReadString();
        char[] charSeparators = new char[] { '|' };
        string[] msgStrArray = msgString.Split(charSeparators, System.StringSplitOptions.RemoveEmptyEntries);

        GameObject targetObj = null;
        string targetObjectName = "";
        string functionName = msgStrArray[0];

        // Starting index of data fields
        // Assumes index:
        // 0: Function Name
        // 1: Target GameObject Name
        // 2: Data parameters
        // May change if a default GameObject is set i.e.
        // 0: Function Name
        //  : (GameObject name is not provided)
        // 1: Data parameters
        int startingDataIndex = 2;

        if (msgStrArray.Length < 2)
        {
            LogUI(msg.conn.address + " sent unknown msg '" + msgString + "'");
        }
        else
        {
            targetObjectName = msgStrArray[1];
            targetObj = GameObject.Find(targetObjectName);
            if(targetObj == null)
            {
                targetObjectName = defaultTargetObjectName;
                targetObj = GameObject.Find(targetObjectName);
                startingDataIndex = 1;
            }
        }

        if (targetObj != null)
        {
            // CAVE2 Transform Sync
            if (functionName.Equals("SyncPosition"))
            {
                float x = 0;
                float y = 0;
                float z = 0;
                float.TryParse(msgStrArray[startingDataIndex], out x);
                float.TryParse(msgStrArray[startingDataIndex + 1], out y);
                float.TryParse(msgStrArray[startingDataIndex + 2], out z);

                targetObj.SendMessage("SyncPosition", new Vector3(x, y, z));
            }
            else if (functionName.Equals("SyncRotation"))
            {
                float x = 0;
                float y = 0;
                float z = 0;
                float w = 0;
                float.TryParse(msgStrArray[startingDataIndex], out x);
                float.TryParse(msgStrArray[startingDataIndex + 1], out y);
                float.TryParse(msgStrArray[startingDataIndex + 2], out z);
                float.TryParse(msgStrArray[startingDataIndex + 3], out w);

                targetObj.SendMessage("SyncRotation", new Quaternion(x, y, z, w));
            }

            // Generic Transform functions
            else if (functionName.Equals("translate", System.StringComparison.OrdinalIgnoreCase))
            {
                float x = 0;
                float y = 0;
                float z = 0;
                float.TryParse(msgStrArray[startingDataIndex], out x);
                float.TryParse(msgStrArray[startingDataIndex + 1], out y);
                float.TryParse(msgStrArray[startingDataIndex + 2], out z);

                if(msgStrArray.Length == 6)
                {
                    if(msgStrArray[5].Equals("self", System.StringComparison.OrdinalIgnoreCase))
                    {
                        targetObj.transform.Translate(x, y, z, Space.Self);
                    }
                    else if (msgStrArray[5].Equals("world", System.StringComparison.OrdinalIgnoreCase))
                    {
                        targetObj.transform.Translate(x, y, z, Space.World);
                    }
                }
                else
                {
                    targetObj.transform.Translate(x, y, z, Space.Self);
                }
            }
            else if (functionName.Equals("rotate", System.StringComparison.OrdinalIgnoreCase))
            {
                float x = 0;
                float y = 0;
                float z = 0;
                float.TryParse(msgStrArray[startingDataIndex], out x);
                float.TryParse(msgStrArray[startingDataIndex + 1], out y);
                float.TryParse(msgStrArray[startingDataIndex + 2], out z);

                if (msgStrArray.Length == 6)
                {
                    if (msgStrArray[5].Equals("self", System.StringComparison.OrdinalIgnoreCase))
                    {
                        targetObj.transform.Rotate(x, y, z, Space.Self);
                    }
                    else if (msgStrArray[5].Equals("world", System.StringComparison.OrdinalIgnoreCase))
                    {
                        targetObj.transform.Rotate(x, y, z, Space.World);
                    }
                }
                else
                {
                    targetObj.transform.Rotate(x, y, z, Space.Self);
                }
            }
            else if (functionName.Equals("setPosition", System.StringComparison.OrdinalIgnoreCase))
            {
                float x = 0;
                float y = 0;
                float z = 0;
                float.TryParse(msgStrArray[startingDataIndex], out x);
                float.TryParse(msgStrArray[startingDataIndex + 1], out y);
                float.TryParse(msgStrArray[startingDataIndex + 2], out z);

                targetObj.transform.position = new Vector3(x, y, z);
            }
            else if (functionName.Equals("setEulerAngles", System.StringComparison.OrdinalIgnoreCase))
            {
                float x = 0;
                float y = 0;
                float z = 0;
                float.TryParse(msgStrArray[startingDataIndex], out x);
                float.TryParse(msgStrArray[startingDataIndex + 1], out y);
                float.TryParse(msgStrArray[startingDataIndex + 2], out z);

                targetObj.transform.eulerAngles = new Vector3(x, y, z);
            }
            else if (functionName.Equals("setLocalPosition", System.StringComparison.OrdinalIgnoreCase))
            {
                float x = 0;
                float y = 0;
                float z = 0;
                float.TryParse(msgStrArray[2], out x);
                float.TryParse(msgStrArray[startingDataIndex], out x);
                float.TryParse(msgStrArray[startingDataIndex + 1], out y);
                float.TryParse(msgStrArray[startingDataIndex + 2], out z);

                targetObj.transform.localPosition = new Vector3(x, y, z);
            }
            else if (functionName.Equals("setLocalEulerAngles", System.StringComparison.OrdinalIgnoreCase))
            {
                float x = 0;
                float y = 0;
                float z = 0;
                float.TryParse(msgStrArray[startingDataIndex], out x);
                float.TryParse(msgStrArray[startingDataIndex + 1], out y);
                float.TryParse(msgStrArray[startingDataIndex + 2], out z);

                targetObj.transform.localEulerAngles = new Vector3(x, y, z);
            }
            else if (functionName.Equals("setLocalScale", System.StringComparison.OrdinalIgnoreCase))
            {
                float x = 0;
                float y = 0;
                float z = 0;

                if (msgStrArray.Length == 3)
                {
                    float.TryParse(msgStrArray[startingDataIndex], out x);
                    y = x;
                    z = x;
                }
                else
                {
                    float.TryParse(msgStrArray[startingDataIndex], out x);
                    float.TryParse(msgStrArray[startingDataIndex + 1], out y);
                    float.TryParse(msgStrArray[startingDataIndex + 2], out z);
                }

                targetObj.transform.localScale = new Vector3(x, y, z);
            }
            else if (functionName.Equals("setTargetObject", System.StringComparison.OrdinalIgnoreCase))
            {
                defaultTargetObjectName = targetObjectName;
            }

            // Let the object handle the message
            else
            {
                string[] paramArray = new string[msgStrArray.Length - 2];
                for(int i = 0; i < paramArray.Length; i++)
                {
                    paramArray[i] = msgStrArray[i + 2];
                }
                targetObj.SendMessage(functionName, paramArray);
            }
        }
        else
        {
            LogUI("CAVE2RPCManager: Msg target object '" + msgStrArray[1] + "' not found");
        }
    }

    private void ParamToByte(NetworkWriter writer, object param)
    {
        if(param is System.Enum || param is System.Int32)
        {
            writer.Write("int");
            writer.Write((int)param);
        }
        else if (param is Vector3)
        {
            writer.Write("Vector3");
            writer.Write((Vector3)param);
        }
        else if (param is System.Single)
        {
            writer.Write("Single");
            writer.Write((System.Single)param);
        }
        else if (param is System.Boolean)
        {
            writer.Write("Boolean");
            writer.Write((System.Boolean)param);
        }
        else if (param is System.String)
        {
            writer.Write("String");
            writer.Write((System.String)param);
        }
        else if (param is Quaternion)
        {
            writer.Write("Quaternion");
            writer.Write((Quaternion)param);
        }
        else
        {
            Debug.LogWarning("CAVE2RPCManager: Unknown param " + param.GetType());
        }
    }

    private object ReaderToObject(NetworkReader networkReader)
    {
        string type = networkReader.ReadString();
        if(type == "int")
        {
            return networkReader.ReadInt32();
        }
        else if (type == "Vector3")
        {
            return networkReader.ReadVector3();
        }
        else if (type == "Single")
        {
            return networkReader.ReadSingle();
        }
        else if (type == "Boolean")
        {
            return networkReader.ReadBoolean();
        }
        else if (type == "String")
        {
            return networkReader.ReadString();
        }
        else if (type == "Quaternion")
        {
            return networkReader.ReadQuaternion();
        }
        else
        {
            return null;
        }
    }

    public void BroadcastMessage(string targetObjectName, string methodName, object param, MsgType msgType = MsgType.Reliable)
    {
        if (debugRPC)
        {
            LogUI("CAVE2 BroadcastMessage (Param 1) '" + methodName + "' on " + targetObjectName);
        }

        NetworkWriter writer = new NetworkWriter();
        writer.StartMessage(101);
        writer.Write(targetObjectName);
        writer.Write(methodName);
        writer.Write(1);

        ParamToByte(writer, param);

        writer.FinishMessage();

        ServerSendMsgToClients(writer.ToArray(), msgType);

#if USING_GETREAL3D
        if (getReal3D.Cluster.isMaster)
            getReal3D.RpcManager.call("BroadcastCAVE2RPC", targetObjectName, methodName, param);
        else
            BroadcastCAVE2RPC(targetObjectName, methodName, param);
#else
        BroadcastCAVE2RPC(targetObjectName, methodName, param);
#endif
    }

    public void BroadcastMessage(string targetObjectName, string methodName, object param, object param2, MsgType msgType = MsgType.Reliable)
    {
        if (debugRPC)
        {
            LogUI("CAVE2 BroadcastMessage (Param 4) '" + methodName + "' on " + targetObjectName);
        }

        NetworkWriter writer = new NetworkWriter();
        writer.StartMessage(102);
        writer.Write(targetObjectName);
        writer.Write(methodName);
        writer.Write(2);

        ParamToByte(writer, param);
        ParamToByte(writer, param2);

        writer.FinishMessage();

        ServerSendMsgToClients(writer.ToArray(), msgType);

#if USING_GETREAL3D
        if (getReal3D.Cluster.isMaster)
            getReal3D.RpcManager.call("BroadcastCAVE2RPC4", targetObjectName, methodName, param, param2);
        else
            BroadcastCAVE2RPC4(targetObjectName, methodName, param, param2);
#else
        BroadcastCAVE2RPC4(targetObjectName, methodName, param, param2);
#endif
    }

    public void SendMessage(string targetObjectName, string methodName, object param, MsgType msgType = MsgType.Reliable)
    {
        if (debugRPC)
        {
            LogUI("CAVE2 SendMessage (Param 1) '" + methodName + "' on " + targetObjectName);
        }

        NetworkWriter writer = new NetworkWriter();
        writer.StartMessage(201);
        writer.Write(targetObjectName);
        writer.Write(methodName);
        writer.Write(1);

        ParamToByte(writer, param);

        writer.FinishMessage();

        ServerSendMsgToClients(writer.ToArray(), msgType);

#if USING_GETREAL3D
        if (getReal3D.Cluster.isMaster)
            getReal3D.RpcManager.call("SendCAVE2RPC", targetObjectName, methodName, param);
        else
            SendCAVE2RPC(targetObjectName, methodName, param);
#else
        SendCAVE2RPC(targetObjectName, methodName, param);
#endif
    }

    public void SendMessage(string targetObjectName, string methodName, object param, object param2, MsgType msgType = MsgType.Reliable)
    {
        if (debugRPC)
        {
            LogUI("CAVE2 SendMessage (Param 4)'" + methodName + "' on " + targetObjectName);
        }

        NetworkWriter writer = new NetworkWriter();
        writer.StartMessage(202);
        writer.Write(targetObjectName);
        writer.Write(methodName);
        writer.Write(2);

        ParamToByte(writer, param);
        ParamToByte(writer, param2);

        writer.FinishMessage();

        ServerSendMsgToClients(writer.ToArray(), msgType);

#if USING_GETREAL3D
        if (getReal3D.Cluster.isMaster)
            getReal3D.RpcManager.call("SendCAVE2RPC4", targetObjectName, methodName, param, param2);
        else
            SendCAVE2RPC4(targetObjectName, methodName, param, param2);
#else
        SendCAVE2RPC4(targetObjectName, methodName, param, param2);
#endif
    }

    public void SendMessage(string targetObjectName, string methodName, object param, object param2, object param3, MsgType msgType = MsgType.Reliable)
    {
        if (debugRPC)
        {
            LogUI("CAVE2 SendMessage (Param 5)'" + methodName + "' on " + targetObjectName);
        }

        NetworkWriter writer = new NetworkWriter();
        writer.StartMessage(203);
        writer.Write(targetObjectName);
        writer.Write(methodName);
        writer.Write(3);

        ParamToByte(writer, param);
        ParamToByte(writer, param2);
        ParamToByte(writer, param3);

        writer.FinishMessage();

        ServerSendMsgToClients(writer.ToArray(), msgType);

#if USING_GETREAL3D
        if (getReal3D.Cluster.isMaster)
            getReal3D.RpcManager.call("SendCAVE2RPC5", targetObjectName, methodName, param, param2, param3);
        else
            SendCAVE2RPC5(targetObjectName, methodName, param, param2, param3);
#else
        SendCAVE2RPC5(targetObjectName, methodName, param, param2, param3);
#endif
    }

    public void SendMessage(string targetObjectName, string methodName, object param, object param2, object param3, object param4, MsgType msgType = MsgType.Reliable)
    {
        if (debugRPC)
        {
            LogUI("CAVE2 SendMessage (Param 6)'" + methodName + "' on " + targetObjectName);
        }

        NetworkWriter writer = new NetworkWriter();
        writer.StartMessage(204);
        writer.Write(targetObjectName);
        writer.Write(methodName);
        writer.Write(4);

        ParamToByte(writer, param);
        ParamToByte(writer, param2);
        ParamToByte(writer, param3);
        ParamToByte(writer, param4);

        writer.FinishMessage();

        ServerSendMsgToClients(writer.ToArray(), msgType);

#if USING_GETREAL3D
        if (getReal3D.Cluster.isMaster)
            getReal3D.RpcManager.call("SendCAVE2RPC6", targetObjectName, methodName, param, param2, param3, param4);
        else
            SendCAVE2RPC6(targetObjectName, methodName, param, param2, param3, param4);
#else
        SendCAVE2RPC6(targetObjectName, methodName, param, param2, param3, param4);
#endif
    }

    public void SendMessage(string targetObjectName, string methodName, object param, object param2, object param3, object param4, object param5, MsgType msgType = MsgType.Reliable)
    {
        if (debugRPC)
        {
            LogUI("CAVE2 SendMessage (Param 6)'" + methodName + "' on " + targetObjectName);
        }

        NetworkWriter writer = new NetworkWriter();
        writer.StartMessage(205);
        writer.Write(targetObjectName);
        writer.Write(methodName);
        writer.Write(5);

        ParamToByte(writer, param);
        ParamToByte(writer, param2);
        ParamToByte(writer, param3);
        ParamToByte(writer, param4);
        ParamToByte(writer, param5);

        writer.FinishMessage();

        ServerSendMsgToClients(writer.ToArray(), msgType);

#if USING_GETREAL3D
        if (getReal3D.Cluster.isMaster)
            getReal3D.RpcManager.call("SendCAVE2RPC7", targetObjectName, methodName, param, param2, param3, param4, param5);
        else
            SendCAVE2RPC7(targetObjectName, methodName, param, param2, param3, param4, param5);
#else
        SendCAVE2RPC7(targetObjectName, methodName, param, param2, param3, param4, param5);
#endif
    }

    public void SendMessage(string targetObjectName, string methodName, object param, object param2, object param3, object param4, object param5, object param6, object param7, MsgType msgType = MsgType.Reliable)
    {
        if (debugRPC)
        {
            LogUI("CAVE2 SendMessage (Param 9)'" + methodName + "' on " + targetObjectName);
        }

        NetworkWriter writer = new NetworkWriter();
        writer.StartMessage(207);
        writer.Write(targetObjectName);
        writer.Write(methodName);
        writer.Write(7);

        ParamToByte(writer, param);
        ParamToByte(writer, param2);
        ParamToByte(writer, param3);
        ParamToByte(writer, param4);
        ParamToByte(writer, param5);
        ParamToByte(writer, param6);
        ParamToByte(writer, param7);

        writer.FinishMessage();

        ServerSendMsgToClients(writer.ToArray(), msgType);

#if USING_GETREAL3D
        if (getReal3D.Cluster.isMaster)
            getReal3D.RpcManager.call("SendCAVE2RPC9", targetObjectName, methodName, param, param2, param3, param4, param5, param6, param7);
        else
            SendCAVE2RPC9(targetObjectName, methodName, param, param2, param3, param4, param5, param6, param7);
#else
        SendCAVE2RPC9(targetObjectName, methodName, param, param2, param3, param4, param5, param6, param7);
#endif
    }

    public void SendMessage(string targetObjectName, string methodName, object param, object param2, object param3, object param4, object param5, object param6, object param7, object param8, object param9, object param10, object param11, object param12, object param13, object param14, object param15, object param16, MsgType msgType = MsgType.Reliable)
    {
        if (debugRPC)
        {
            LogUI("CAVE2 SendMessage (Param 18)'" + methodName + "' on " + targetObjectName);
        }

        NetworkWriter writer = new NetworkWriter();
        writer.StartMessage(216);
        writer.Write(targetObjectName);
        writer.Write(methodName);
        writer.Write(16);

        ParamToByte(writer, param);
        ParamToByte(writer, param2);
        ParamToByte(writer, param3);
        ParamToByte(writer, param4);
        ParamToByte(writer, param5);
        ParamToByte(writer, param6);
        ParamToByte(writer, param7);
        ParamToByte(writer, param8);
        ParamToByte(writer, param9);
        ParamToByte(writer, param10);
        ParamToByte(writer, param11);
        ParamToByte(writer, param12);
        ParamToByte(writer, param13);
        ParamToByte(writer, param14);
        ParamToByte(writer, param15);
        ParamToByte(writer, param16);

        writer.FinishMessage();

        ServerSendMsgToClients(writer.ToArray(), msgType);

#if USING_GETREAL3D
        if (getReal3D.Cluster.isMaster)
            getReal3D.RpcManager.call("SendCAVE2RPC18", targetObjectName, methodName, param, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, param16);
        else
            SendCAVE2RPC18(targetObjectName, methodName, param, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, param16);
#else
        SendCAVE2RPC18(targetObjectName, methodName, param, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, param16);
#endif
    }

    public void SendMessage(string targetObjectName, string methodName, MsgType msgType = MsgType.Reliable)
    {
        SendMessage(targetObjectName, methodName, 0, msgType);
    }

    public void Destroy(string targetObjectName)
    {
#if USING_GETREAL3D
        if (getReal3D.Cluster.isMaster)
            getReal3D.RpcManager.call("CAVE2DestroyRPC", targetObjectName);
        else
            CAVE2DestroyRPC(targetObjectName);
#else
        CAVE2DestroyRPC(targetObjectName);
#endif
    }

#if USING_GETREAL3D
    [getReal3D.RPC]
#endif
    public void BroadcastCAVE2RPC(string targetObjectName, string methodName, object param)
    {
        cave2RPCCallCount++;

        if (debugRPC)
            Debug.Log("BroadcastCAVE2RPC: call '" + methodName + "' on " + targetObjectName);

        GameObject targetObject = GameObject.Find(targetObjectName);
        if (targetObject != null)
        {
            //Debug.Log ("Broadcast '" +methodName +"' on "+targetObject.name);
            targetObject.BroadcastMessage(methodName, param, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            Debug.LogWarning("CAVE2RPCManager: BroadcastCAVE2RPC failed to find gameObject '" + targetObjectName + "'");
        }
    }

#if USING_GETREAL3D
    [getReal3D.RPC]
#endif
    public void BroadcastCAVE2RPC4(string targetObjectName, string methodName, object param, object param2)
    {
        cave2RPCCallCount++;

        if (debugRPC)
            Debug.Log("BroadcastCAVE2RPC4: call '" + methodName + "' on " + targetObjectName);

        GameObject targetObject = GameObject.Find(targetObjectName);
        if (targetObject != null)
        {
            //Debug.Log ("Broadcast '" +methodName +"' on "+targetObject.name);
            targetObject.BroadcastMessage(methodName, new object[] { param, param2 }, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            Debug.LogWarning("CAVE2RPCManager: BroadcastCAVE2RPC failed to find gameObject '" + targetObjectName + "'");
        }
    }

#if USING_GETREAL3D
    [getReal3D.RPC]
#endif
    public void SendCAVE2RPC(string targetObjectName, string methodName, object param)
    {
        cave2RPCCallCount++;

        if (debugRPC)
            Debug.Log ("SendCAVE2RPC: call '" +methodName +"' on "+targetObjectName);

        GameObject targetObject = GameObject.Find(targetObjectName);
        if (targetObject != null)
        {
            //Debug.Log ("Broadcast '" +methodName +"' on "+targetObject.name);
            targetObject.SendMessage(methodName, param, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            Debug.LogWarning("CAVE2RPCManager: SendCAVE2RPC failed to find gameObject '" + targetObjectName + "'");
        }
    }

#if USING_GETREAL3D
    [getReal3D.RPC]
#endif
    public void SendCAVE2RPC4(string targetObjectName, string methodName, object param, object param2)
    {
        cave2RPCCallCount++;
        if (debugRPC)
            Debug.Log ("SendCAVE2RPC4: call '" +methodName +"' on "+targetObjectName);

        GameObject targetObject = GameObject.Find(targetObjectName);
        if (targetObject != null)
        {
            //Debug.Log ("Broadcast '" +methodName +"' on "+targetObject.name);
            targetObject.SendMessage(methodName, new object[] { param, param2 }, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            Debug.LogWarning("CAVE2RPCManager: SendCAVE2RPC4 failed to find gameObject '" + targetObjectName + "'");
        }
    }

#if USING_GETREAL3D
    [getReal3D.RPC]
#endif
    public void SendCAVE2RPC5(string targetObjectName, string methodName, object param, object param2, object param3)
    {
        cave2RPCCallCount++;
        if (debugRPC)
            Debug.Log ("SendCAVE2RPC5: call '" +methodName +"' on "+targetObjectName);

        GameObject targetObject = GameObject.Find(targetObjectName);
        if (targetObject != null)
        {
            //Debug.Log ("Broadcast '" +methodName +"' on "+targetObject.name);
            targetObject.SendMessage(methodName, new object[] { param, param2, param3 }, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            Debug.LogWarning("CAVE2RPCManager: SendCAVE2RPC5 failed to find gameObject '" + targetObjectName + "'");
        }
    }

#if USING_GETREAL3D
    [getReal3D.RPC]
#endif
    public void SendCAVE2RPC6(string targetObjectName, string methodName, object param, object param2, object param3, object param4)
    {
        cave2RPCCallCount++;
        if (debugRPC)
            Debug.Log ("SendCAVE2RPC6: call '" +methodName +"' on "+targetObjectName);

        GameObject targetObject = GameObject.Find(targetObjectName);
        if (targetObject != null)
        {
            //Debug.Log ("Broadcast '" +methodName +"' on "+targetObject.name);
            targetObject.SendMessage(methodName, new object[] { param, param2, param3, param4 }, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            Debug.LogWarning("CAVE2RPCManager: SendCAVE2RPC6 failed to find gameObject '" + targetObjectName + "'");
        }
    }

#if USING_GETREAL3D
    [getReal3D.RPC]
#endif
    public void SendCAVE2RPC7(string targetObjectName, string methodName, object param, object param2, object param3, object param4, object param5)
    {
        cave2RPCCallCount++;
        if (debugRPC)
            Debug.Log("SendCAVE2RPC7: call '" + methodName + "' on " + targetObjectName);

        GameObject targetObject = GameObject.Find(targetObjectName);
        if (targetObject != null)
        {
            //Debug.Log ("Broadcast '" +methodName +"' on "+targetObject.name);
            targetObject.SendMessage(methodName, new object[] { param, param2, param3, param4, param5 }, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            Debug.LogWarning("CAVE2RPCManager: SendCAVE2RPC7 failed to find gameObject '" + targetObjectName + "'");
        }
    }

#if USING_GETREAL3D
    [getReal3D.RPC]
#endif
    public void SendCAVE2RPC9(string targetObjectName, string methodName, object param, object param2, object param3, object param4, object param5, object param6, object param7)
    {
        cave2RPCCallCount++;
        if (debugRPC)
            Debug.Log("SendCAVE2RPC9: call '" + methodName + "' on " + targetObjectName);

        GameObject targetObject = GameObject.Find(targetObjectName);
        if (targetObject != null)
        {
            //Debug.Log ("Broadcast '" +methodName +"' on "+targetObject.name);
            targetObject.SendMessage(methodName, new object[] { param, param2, param3, param4, param5, param6, param7 }, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            Debug.LogWarning("CAVE2RPCManager: SendCAVE2RPC9 failed to find gameObject '" + targetObjectName + "'");
        }
    }

#if USING_GETREAL3D
    [getReal3D.RPC]
#endif
    public void SendCAVE2RPC18(string targetObjectName, string methodName, object param, object param2, object param3, object param4, object param5, object param6, object param7, object param8, object param9, object param10, object param11, object param12, object param13, object param14, object param15, object param16)
    {
        cave2RPCCallCount++;
        if (debugRPC)
            Debug.Log("SendCAVE2RPC18: call '" + methodName + "' on " + targetObjectName);

        GameObject targetObject = GameObject.Find(targetObjectName);
        if (targetObject != null)
        {
            //Debug.Log ("Broadcast '" +methodName +"' on "+targetObject.name);
            targetObject.SendMessage(methodName, new object[] { param, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, param16 }, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            Debug.LogWarning("CAVE2RPCManager: SendCAVE2RPC18 failed to find gameObject '" + targetObjectName + "'");
        }
    }

#if USING_GETREAL3D
    [getReal3D.RPC]
#endif
    public void CAVE2DestroyRPC(string targetObjectName)
    {
        cave2RPCCallCount++;
        if (debugRPC)
            Debug.Log ("SendCAVE2RPC: call 'Destroy' on "+targetObjectName);

        GameObject targetObject = GameObject.Find(targetObjectName);
        if (targetObject != null)
        {
            Destroy(targetObject);
        }
        else
        {
            Debug.LogWarning("CAVE2RPCManager: CAVE2DestroyRPC failed to find gameObject '" + targetObjectName + "'");
        }
    }
}
