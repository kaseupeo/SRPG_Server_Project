@echo off
START ../../Server/PacketGenerator/bin/Debug/net8.0/PacketGenerator.exe ../../Server/PacketGenerator/PDL.xml

@REM XCOPY /Y GeneratePackets.cs "../../DummyClient/Packet"
XCOPY /Y GeneratePackets.cs "../../Client/Assets/Scripts/Runtime/Network/Packet"
XCOPY /Y GeneratePackets.cs "../../Server/Server/Packet"

@REM XCOPY /Y ClientPacketManager.cs "../../DummyClient/Packet"
XCOPY /Y ClientPacketManager.cs "../../Client/Assets/Scripts/Runtime/Network/Packet"
XCOPY /Y ServerPacketManager.cs "../../Server/Server/Packet"