﻿namespace PacketGenerator;

public class PacketFormat
{
    // {0} 패킷 등록
    public static string managerFormat = 
        @"
using System;
using System.Collections.Generic;
using ServerCore;

public class PacketManager
{{
    #region Singleton

    private static PacketManager _instance = new PacketManager();
    public static PacketManager Instance => _instance;

    #endregion

    PacketManager() => Register();

    private Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _makeFunc = new();
    private Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new();

    public void Register()
    {{
{0}
    }}
    
    public void OnReceivePacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onReceiveCallback = null)
    {{
        ushort count = 0;
        
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
        if (_makeFunc.TryGetValue(id, out func))
        {{
            IPacket packet = func.Invoke(session, buffer);

            if (onReceiveCallback != null)
                onReceiveCallback.Invoke(session, packet);
            else
                HandlePacket(session, packet);
        }}
    }}

    private T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {{
        T packet = new T();
        packet.Read(buffer);

        return packet;
    }}

    public void HandlePacket(PacketSession session, IPacket packet)
    {{
        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(packet.Protocol, out action)) 
            action.Invoke(session, packet);
    }}
}}
";

    // {0} 패킷 이름
    public static string managerRegisterFormat = 
        @"
        _makeFunc.Add((ushort)PacketID.{0}, MakePacket<{0}>);
        _handler.Add((ushort)PacketID.{0}, PacketHandler.{0}Handler);";
    
    // {0} 패킷 이름/번호 목록
    // {1} 패킷 목록
    public static string fileFormat = 
        @"
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

public enum PacketID
{{
    {0}
}}

public interface IPacket
{{
	public ushort Protocol {{ get; }}
	public void Read(ArraySegment<byte> segment);
	public ArraySegment<byte> Write();
}}

{1}";

    // {0} 패킷 이름
    // {1} 패킷 번호
    public static string packetEnumFormat = 
        @"{0} = {1},";
    
    // {0} : 패킷 이름
    // {1} : 멤버 변수들
    // {2} : 멤버 변수 Read
    // {3} : 멤버 변수 Write
    public static string packetFormat = 
        @"
public class {0} : IPacket
{{
    {1}
	public ushort Protocol => (ushort)PacketID.{0};

    public void Read(ArraySegment<byte> segment)
    {{
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        count += sizeof(ushort);
        count += sizeof(ushort);
        {2}
    }}
    
    public ArraySegment<byte> Write()
    {{
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;
        bool success = true;

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.{0});
        count += sizeof(ushort);
       {3}
        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        
        return SendBufferHelper.Close(count);
    }}
}}";
    
    // {0} : 변수 형식
    // {1} : 변수 이름
    public static string memberFormat = "public {0} {1};";

    // {0} : 리스트 이름 [대문자]
    // {1} : 리스트 이름 [소문자]
    // {2} : 멤버 변수들
    // {3} : 멤버 변수 Read
    // {4} : 멤버 변수 Write
    public static string memberListFormat = 
        @"
public class {0}
{{
    {2}

    public void Read(ReadOnlySpan<byte> s, ref ushort count)
    {{
        {3}
    }}

    public bool Write(Span<byte> s, ref ushort count)
    {{
        bool success = true;
        {4}
        return success;
    }}
}}

public List<{0}> {1}List = new List<{0}>();
";
    
    
    // {0} : 변수 이름 
    // {1} : To~ 변수 형식
    // {2} : 변수 형식
    public static string readFormat =
        @"
this.{0} = BitConverter.{1}(s.Slice(count, s.Length - count));
count += sizeof({2});";

    // {0} : 변수 이름
    // {1} : 변수 형식
    public static string readByteFormat = 
        @"
this.{0} = ({1})segment.Array[segment.Offset + count];
count += sizeof({1});";
    
    // {0} : 변수 이름
    public static string readStringFormat = 
        @"
ushort {0}Length = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
count += sizeof(ushort);
this.{0} = Encoding.Unicode.GetString(s.Slice(count, {0}Length));
count += {0}Length;";

    // {0} : 리스트 이름 [대문자]
    // {1} : 리스트 이름 [소문자]
    public static string readListFormat = 
        @"
this.{1}List.Clear();
ushort {1}Length = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
count += sizeof(ushort);

for (int i = 0; i < {1}Length; i++)
{{
    {0} {1} = new {0}();
    {1}.Read(s, ref count);
    {1}List.Add({1});
}}";
    
    // {0} : 변수 이름
    // {1} : 변수 형식
    public static string writeFormat = 
        @"
success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.{0});
count += sizeof({1});";

    // {0} : 변수 이름
    // {1} : 변수 형식
    public static string writeByteFormat = 
        @"
segment.Array[segment.Offset + count] = (byte)this.{0};
count += sizeof({1});";
    
    // {0} : 변수 이름
    public static string writeStringFormat = 
        @"
ushort {0}Length = (ushort)Encoding.Unicode.GetBytes(this.{0}, 0, this.{0}.Length, segment.Array, segment.Offset + count + sizeof(ushort));
success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), {0}Length);
count += sizeof(ushort);
count += {0}Length;";

    // {0} : 리스트 이름 [대문자]
    // {1} : 리스트 이름 [소문자]
    public static string writeListFormat =
        @"
success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort){1}List.Count);
count += sizeof(ushort);

foreach ({0} {1} in this.{1}List) 
    success &= {1}.Write(s, ref count);";
    
}