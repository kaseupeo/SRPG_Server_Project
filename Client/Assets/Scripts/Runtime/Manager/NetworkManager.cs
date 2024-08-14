using System;
using System.Collections.Generic;
using System.Net;
using ServerCore;
using UnityEngine;

public class NetworkManager
{
    private ServerSession _session = new ServerSession();

    public void Init()
    {
        // DNS (Domain Name System)
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddress = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddress, 7777);

        Connector connector = new Connector();
        connector.Connect(endPoint, () => _session, 1);
    }

    public void Update()
    {
        List<IPacket> packetList = PacketQueue.Instance.PopAll();

        foreach (IPacket packet in packetList) 
            PacketManager.Instance.HandlePacket(_session, packet);
    }

    public void Send(ArraySegment<byte> sendBuffer)
    {
        _session.Send(sendBuffer);
    }
}
