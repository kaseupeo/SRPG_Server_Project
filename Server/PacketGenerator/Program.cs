using System.Xml;

namespace PacketGenerator;

class Program
{
    private static string _generatePackets;
    private static ushort _packetId;
    private static string _packetEnums;

    private static string _clientRegister;
    private static string _serverRegister;
    
    static void Main(string[] args)
    {
        string pdlPath = "../../../PDL.xml";
            
        XmlReaderSettings settings = new XmlReaderSettings() { IgnoreComments = true, IgnoreWhitespace = true };

        if (args.Length >= 1)
            pdlPath = args[0];
        
        using (XmlReader reader = XmlReader.Create(pdlPath, settings))
        {
            reader.MoveToContent();

            while (reader.Read())
            {
                if (reader.Depth == 1 && reader.NodeType == XmlNodeType.Element)
                    ParsePacket(reader);

                // Console.WriteLine(reader.Name + " " + reader["name"]);
            }

            string fileText = string.Format(PacketFormat.fileFormat, _packetEnums, _generatePackets);
            File.WriteAllText("GeneratePackets.cs", fileText);
            string clientManagerText = string.Format(PacketFormat.managerFormat, _clientRegister);
            File.WriteAllText("ClientPacketManager.cs", clientManagerText);
            string serverManagerText = string.Format(PacketFormat.managerFormat, _serverRegister);
            File.WriteAllText("ServerPacketManager.cs", serverManagerText);
        }
    }

    public static void ParsePacket(XmlReader reader)
    {
        if (reader.NodeType == XmlNodeType.EndElement)
            return;

        if (reader.Name.ToLower() != "packet")
        {
            Console.WriteLine("Packet without node");
            return;
        }

        string packetName = reader["name"];
        if (string.IsNullOrEmpty(packetName))
        {
            Console.WriteLine("Packet without name");
            return;
        }

        Tuple<string, string, string> tuple = ParseMembers(reader);
        _generatePackets += string.Format(PacketFormat.packetFormat, packetName, tuple.Item1, tuple.Item2, tuple.Item3);
        _packetEnums += string.Format(PacketFormat.packetEnumFormat, packetName, ++_packetId) + Environment.NewLine + "\t";
        
        if (packetName.StartsWith("S_") || packetName.StartsWith("s_"))
            _clientRegister += string.Format(PacketFormat.managerRegisterFormat, packetName) + Environment.NewLine;
        else
            _serverRegister += string.Format(PacketFormat.managerRegisterFormat, packetName) + Environment.NewLine;
    }

    // {1} : 멤버 변수들
    // {2} : 멤버 변수 Read
    // {3} : 멤버 변수 Write
    public static Tuple<string, string, string> ParseMembers(XmlReader reader)
    {
        string packetName = reader["name"];
        string memberCode = "";
        string readCode = "";
        string writeCode = "";

        int depth = reader.Depth + 1;
        while (reader.Read())
        {
            if (reader.Depth != depth)
                break;

            string memberName = reader["name"];
            if (string.IsNullOrEmpty(memberName))
            {
                Console.WriteLine("Member without name");
                return null;
            }

            if (string.IsNullOrEmpty(memberCode) == false)
                memberCode += Environment.NewLine;
            if (string.IsNullOrEmpty(readCode) == false)
                readCode += Environment.NewLine;
            if (string.IsNullOrEmpty(writeCode) == false)
                writeCode += Environment.NewLine;

            string memberType = reader.Name.ToLower();
            switch (memberType)
            {
                case "byte":
                case "sbyte":
                    memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                    readCode += string.Format(PacketFormat.readByteFormat, memberName, memberType);
                    writeCode += string.Format(PacketFormat.writeByteFormat, memberName, memberType);
                    break;
                case "bool":
                case "short":
                case "ushort":
                case "int":
                case "long":
                case "float":
                case "double":
                    memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                    readCode += string.Format(PacketFormat.readFormat, memberName, ToMemberType(memberType), memberType);
                    writeCode += string.Format(PacketFormat.writeFormat, memberName, memberType);
                    break;
                case "string":
                    memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                    readCode += string.Format(PacketFormat.readStringFormat, memberName);
                    writeCode += string.Format(PacketFormat.writeStringFormat, memberName);
                    break;
                case "list":
                    Tuple<string, string, string> tuple = ParseList(reader);
                    memberCode += tuple.Item1;
                    readCode += tuple.Item2;
                    writeCode += tuple.Item3;
                    break;
                default:
                    break;
            }
        }

        memberCode = memberCode.Replace("\n", "\n\t");
        readCode = readCode.Replace("\n", "\n\t\t");
        writeCode = writeCode.Replace("\n", "\n\t\t");
        return new Tuple<string, string, string>(memberCode, readCode, writeCode);
    }

    public static Tuple<string, string, string> ParseList(XmlReader reader)
    {
        string listName = reader["name"];
        if (string.IsNullOrEmpty(listName))
        {
            Console.WriteLine("List without name");
            return null;
        }

        Tuple<string,string,string> tuple = ParseMembers(reader);

        string memberCode = string.Format(PacketFormat.memberListFormat, FirstCharToUpper(listName),
            FirstCharToLower(listName), tuple.Item1, tuple.Item2, tuple.Item3);

        string readCode = string.Format(PacketFormat.readListFormat, FirstCharToUpper(listName),
            FirstCharToLower(listName));
        
        string writeCode = string.Format(PacketFormat.writeListFormat, FirstCharToUpper(listName),
            FirstCharToLower(listName));

        return new Tuple<string, string, string>(memberCode, readCode, writeCode);
    }
    
    public static string ToMemberType(string memberType)
    {
        switch (memberType)
        {
            case "bool":
                return "ToBoolean";
            case "short":
                return "ToInt16";
            case "ushort":
                return "ToUInt16";
            case "int":
                return "ToInt32";
            case "long":
                return "ToInt64";
            case "float":
                return "ToSingle";
            case "double":
                return "ToDouble";
            default:
                return "";
        }
    }

    public static string FirstCharToUpper(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "";

        return input[0].ToString().ToUpper() + input.Substring(1);
    }
    
    public static string FirstCharToLower(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "";

        return input[0].ToString().ToLower() + input.Substring(1);
    }
}