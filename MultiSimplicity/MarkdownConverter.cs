using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TerrariaToMultiplicity
{
    struct TerrariaPacket
    {
        public byte PacketID;
        public string PacketName;
        public List<PacketType> PacketTypes;
        public bool Failed;
    }

    struct PacketType
    {
        public byte Size;
        public string Name;
        public string Type;
        public string Notes;
    }

    enum DataSizes
    {
        Byte = 1,
    }

    class MarkdownConverter
    {
        private List<string> InputArray;

        public MarkdownConverter(string[] inputArray)
        {
            this.InputArray = new List<string>(inputArray);
        }

        public List<TerrariaPacket> ParseMarkdown()
        {
            Console.WriteLine("Read {0} lines", InputArray.Count);

            StripTerminator();
            var packetList = LocatePackets();
            var packets = DeserializePackets(packetList);

            return packets;
        }

        private void StripTerminator()
        {
            var terminator = InputArray.FirstOrDefault(x => x.Contains("***"));
            int terminatorPos = InputArray.IndexOf(terminator);

            Console.WriteLine("Detected terminator at line {0}", terminatorPos);

            InputArray.RemoveRange(terminatorPos, InputArray.Count - terminatorPos);

            Console.WriteLine("New line count: {0}", InputArray.Count);
        }

        private List<List<string>> LocatePackets()
        {
            List<List<string>> packetList = new List<List<string>>();
            List<string> packet = new List<string>();

            foreach (string s in InputArray)
            {
                if (string.IsNullOrEmpty(s))
                {
                    packetList.Add(packet);
                    packet = new List<string>();
                    continue;
                } 
                packet.Add(s);
            }

            Console.WriteLine("Detected {0} packets", packetList.Count);

            return packetList;
        }

        private List<TerrariaPacket> DeserializePackets(List<List<string>> packetListRaw)
        {
            List<TerrariaPacket> packetList = new List<TerrariaPacket>();

            MarkdownOverrides overrides = new MarkdownOverrides();
            TypeChecker typeChecker = new TypeChecker();

            foreach (List<string> packetRaw in packetListRaw)
            {
                TerrariaPacket packet = new TerrariaPacket();
                packet.PacketTypes = new List<PacketType>();
                for (int i = 0; i < packetRaw.Count; i++)
                {
                    if (Regex.IsMatch(packetRaw[i], @"##\s(.+)\s\[(\d+)\]"))
                    {
                        Match match = Regex.Match(packetRaw[i], @"##\s(.+)\s\[(\d+)\]");
                        packet.PacketName = match.Groups[1].ToString().Replace(" ", string.Empty);
                        packet.PacketID = byte.Parse(match.Groups[2].ToString());

                        if (overrides.ForceFail.ContainsKey(packet.PacketID))
                        {
                            packet.Failed = true;
                        }
                    }
                    else if (Regex.IsMatch(packetRaw[i], @"\|(\d+|\?)\|([\w\s]+)\|([\w\s]+)\|(-|.+)?\|?"))
                    {
                        Match match = Regex.Match(packetRaw[i], @"\|(\d+|\?)\|([\w\s]+)\|([\w\s]+)\|(-|.+)?\|?");
                        PacketType type = new PacketType();
                        if (match.Groups[1].ToString() == "?")
                        {
                            type.Size = 0;
                        } 
                        else
                        {
                            type.Size = byte.Parse(match.Groups[1].ToString());
                        }
                        
                        type.Name = match.Groups[2].ToString().Replace(" ", string.Empty);
                        type.Type = match.Groups[3].ToString().Replace(" ", string.Empty);
                        type.Notes = match.Groups[4].ToString();

                        if (!string.IsNullOrWhiteSpace(type.Notes) && type.Notes != "-" && !overrides.ForceContinue.ContainsKey(packet.PacketID))
                        {
                            packet.Failed = true;
                            Utils.ColorWrite(ConsoleColor.Red, "{0} [{1}] - Packet contains extra notes, cannot automatically generate", packet.PacketID.ToString(), packet.PacketName);
                            break;
                        }

                        if (!typeChecker.TypeSizes.ContainsKey(type.Type))
                        {
                            packet.Failed = true;
                            Utils.ColorWrite(ConsoleColor.Red, "{0} [{1}] - Unknown type '{2}'", packet.PacketID.ToString(), packet.PacketName, type.Type);
                            break;
                        }

                        byte size = typeChecker.TypeSizes[type.Type];
                        if (type.Size != size)
                        {
                            packet.Failed = true;
                            Utils.ColorWrite(ConsoleColor.Red, "{0} [{1}] - Type '{2}' does not match size '{3}'", packet.PacketID.ToString(), packet.PacketName, type.Type, type.Size.ToString());
                            break;
                        }

                        packet.PacketTypes.Add(type);
                    }
                }

                if (!packet.Failed)
                {
                    Utils.ColorWrite(ConsoleColor.Green, "Succeeded: {0} [{1}]", packet.PacketID.ToString(), packet.PacketName);
                }

                packetList.Add(packet);
            }

            return packetList;
        }
    }
}
