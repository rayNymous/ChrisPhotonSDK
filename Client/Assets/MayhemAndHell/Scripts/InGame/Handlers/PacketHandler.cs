using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;
using ProtoBuf;
using UnityEngine;

public abstract class PacketHandler : PhotonEventHandler
{
    private InGameController _controller;
    private Transform _entities;

    private EntityManager _entityManager;

    public PacketHandler(InGameController controller) : base(controller)
    {
        _controller = controller;
        _entityManager = GameObject.FindObjectOfType<EntityManager>();
    }

    public InGameController Controller
    {
        get { return _controller; }
    }

    public override byte Code
    {
        get { return (byte) ClientEventCode.ServerPacket; }
    }

    public override int? SubCode
    {
        get { return (int?) MessageSubCode; }
    }

    public abstract MessageSubCode MessageSubCode { get; }

    public WorldObject GetObjectById(int id)
    {
        return _entityManager.GetObject(id);
    }

    public static T Deserialize<T>(byte[] bytes)
    {
        T deserialized;

        using (var ms = new MemoryStream(bytes))
        {
            deserialized = Serializer.Deserialize<T>(ms);
        }

        return deserialized;
    }

    public static byte[] Serialize<T>(T obj)
    {
        byte[] b = null;

        using (var ms = new MemoryStream())
        {
            Serializer.Serialize<T>(ms, obj);
            b = new byte[ms.Position];
            var fullB = ms.GetBuffer();
            Array.Copy(fullB, b, b.Length);
        }

        return b;
    }
}
