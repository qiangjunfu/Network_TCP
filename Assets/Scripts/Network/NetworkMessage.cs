using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;



public enum MessageType
{
    PositionUpdate,
    CharacterAction,
    ObjectSpawn
}

public class NetworkMessage
{
    public MessageType MessageType;   // 消息类型
    public byte[] Data;               // 数据内容

    public NetworkMessage(MessageType type, byte[] data)
    {
        MessageType = type;
        Data = data;
    }


    public static void HandleMessage(NetworkMessage message)
    {
        switch (message.MessageType)
        {
            case MessageType.PositionUpdate:
                PositionUpdateMessage posMsg = PositionUpdateMessage.FromByteArray(message.Data);
                // 更新角色位置
                break;

            case MessageType.CharacterAction:
                CharacterActionMessage actionMsg = CharacterActionMessage.FromByteArray(message.Data);
                // 执行角色操作
                break;

            case MessageType.ObjectSpawn:
                ObjectSpawnMessage spawnMsg = ObjectSpawnMessage.FromByteArray(message.Data);
                // 生成物体
                break;

            default:
                Debug.Log("Unknown message type");
                break;
        }
    }


}

public class PositionUpdateMessage
{
    public int ClientId;       // 客户端 ID，唯一标识
    public float X;            // 角色的 X 坐标
    public float Y;            // 角色的 Y 坐标
    public float Z;            // 角色的 Z 坐标

    // 序列化为字节流
    public byte[] ToByteArray()
    {
        var data = new List<byte>();
        data.AddRange(BitConverter.GetBytes(ClientId));
        data.AddRange(BitConverter.GetBytes(X));
        data.AddRange(BitConverter.GetBytes(Y));
        data.AddRange(BitConverter.GetBytes(Z));
        return data.ToArray();
    }

    // 从字节流反序列化
    public static PositionUpdateMessage FromByteArray(byte[] data)
    {
        var message = new PositionUpdateMessage();
        message.ClientId = BitConverter.ToInt32(data, 0);
        message.X = BitConverter.ToSingle(data, 4);
        message.Y = BitConverter.ToSingle(data, 8);
        message.Z = BitConverter.ToSingle(data, 12);
        return message;
    }


    public string PrintInfo()
    {
        string str = $"ClientId: {ClientId} , X: {X} , Y: {Y} , Z: {Z} ";
        Debug.Log(str);
        return str;
    }
}

public class CharacterActionMessage
{
    public int ClientId;       // 客户端 ID，唯一标识
    public string ActionType;  // 动作类型，例如 "Jump", "Attack" 等
    public float Timestamp;    // 时间戳，可以用于同步操作

    public byte[] ToByteArray()
    {
        var data = new List<byte>();
        data.AddRange(BitConverter.GetBytes(ClientId));
        data.AddRange(Encoding.UTF8.GetBytes(ActionType));
        data.AddRange(BitConverter.GetBytes(Timestamp));
        return data.ToArray();
    }

    public static CharacterActionMessage FromByteArray(byte[] data)
    {
        var message = new CharacterActionMessage();
        message.ClientId = BitConverter.ToInt32(data, 0);
        message.ActionType = Encoding.UTF8.GetString(data, 4, data.Length - 12);
        message.Timestamp = BitConverter.ToSingle(data, data.Length - 4);
        return message;
    }

    public string PrintInfo()
    {
        string str = $"ClientId: {ClientId} , ActionType: {ActionType} , Timestamp: {Timestamp} ";
        Debug.Log(str);
        return str;
    }
}


public class ObjectSpawnMessage
{
    public int ClientId;       // 客户端 ID，唯一标识
    public int ObjectId;       // 物体的 ID
    public string ObjectType;  // 物体的类型，可能是 "Tree", "Rock" 等
    public float X;            // 物体的初始 X 坐标
    public float Y;            // 物体的初始 Y 坐标
    public float Z;            // 物体的初始 Z 坐标

    public byte[] ToByteArray()
    {
        var data = new List<byte>();
        data.AddRange(BitConverter.GetBytes(ClientId));
        data.AddRange(BitConverter.GetBytes(ObjectId));
        data.AddRange(Encoding.UTF8.GetBytes(ObjectType));
        data.AddRange(BitConverter.GetBytes(X));
        data.AddRange(BitConverter.GetBytes(Y));
        data.AddRange(BitConverter.GetBytes(Z));
        return data.ToArray();
    }

    public static ObjectSpawnMessage FromByteArray(byte[] data)
    {
        var message = new ObjectSpawnMessage();
        message.ClientId = BitConverter.ToInt32(data, 0);
        message.ObjectId = BitConverter.ToInt32(data, 4);
        message.ObjectType = Encoding.UTF8.GetString(data, 8, data.Length - 20);
        message.X = BitConverter.ToSingle(data, data.Length - 12);
        message.Y = BitConverter.ToSingle(data, data.Length - 8);
        message.Z = BitConverter.ToSingle(data, data.Length - 4);
        return message;
    }

    public string PrintInfo()
    {
        string str = $"ClientId: {ClientId} , ObjectId: {ObjectId} , ObjectType: {ObjectType} , X: {X} , Y: {Y} , Z: {Z} ";
        Debug.Log(str);
        return str;
    }
}

