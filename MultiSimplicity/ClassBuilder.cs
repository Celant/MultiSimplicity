using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaToMultiplicity
{
    class ClassBuilder
    {
        List<TerrariaPacket> PacketList;
        List<TerrariaPacket> SkippedPackets;

        public void SetPackets(List<TerrariaPacket> packets)
        {
            PacketList = packets.Where(x => x.Failed == false).ToList();
            SkippedPackets = packets.Where(x => x.Failed == true).ToList();
        }

        public void WriteClasses(string outputFolder)
        {
            if (PacketList == null)
            {
                throw new InvalidOperationException("Packets must be set before building classes");
            }

            Utils.ColorWrite(ConsoleColor.Yellow, "Generating classes for {0} packets", PacketList.Count.ToString());
            Utils.ColorWrite(ConsoleColor.Red, "Skipping generation for {0} packets", SkippedPackets.Count.ToString());

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            foreach (TerrariaPacket packet in PacketList)
            {
                BuildClass(outputFolder + Path.DirectorySeparatorChar + packet.PacketName + ".cs", packet);
            }
        }

        internal void BuildClass(string filename, TerrariaPacket packet)
        {
            using (StreamWriter file = new StreamWriter(filename, false))
            {
                file.WriteLine(ClassBuilderTemplates.PACKET_HEADER);

                if (packet.PacketTypes.Exists((obj) => obj.Type == "Color")) {
                    file.WriteLine(ClassBuilderTemplates.PACKET_HEADER_COLOR);
                }


                file.WriteLine(ClassBuilderTemplates.PACKET_NAMESPACE);

                file.WriteLine(string.Format(ClassBuilderTemplates.CLASS_START, packet.PacketName, packet.PacketID));
                file.WriteLine();

                foreach (PacketType type in packet.PacketTypes)
                {
                    string type_obj = type.Type.ToLower() == "single" ? "float" : type.Type.ToLower();
                    type_obj = type.Type.ToLower() == "int16" ? "short" : type_obj;
					type_obj = type.Type.ToLower() == "uint16" ? "ushort" : type_obj;
					type_obj = type.Type.ToLower() == "int32" ? "int" : type_obj;
					type_obj = type.Type.ToLower() == "uint64" ? "ulong" : type_obj;
					type_obj = type.Type.ToLower() == "boolean" ? "bool" : type_obj;
                    type_obj = type.Type.ToLower() == "color" ? "Color" : type_obj;
		    if (!string.IsNullOrEmpty(type.Notes) && type.Notes != "-")
                    {
                        file.WriteLine(string.Format(ClassBuilderTemplates.PROPERTY_COMMENT, type_obj, type.Name, type.Notes));
                    }
                    else
                    {
                        file.WriteLine(string.Format(ClassBuilderTemplates.PROPERTY, type_obj, type.Name));
                    }
                    file.WriteLine();
                }

                file.WriteLine(string.Format(ClassBuilderTemplates.CONSTRUCTOR, packet.PacketName));
                file.WriteLine();

                file.WriteLine(string.Format(ClassBuilderTemplates.CONSTRUCTOR_READER_START, packet.PacketName));

                foreach (PacketType type in packet.PacketTypes)
                {
                    file.WriteLine(string.Format(ClassBuilderTemplates.CONSTRUCTOR_READER_PROPERTY, type.Name, type.Type));
                }

                file.WriteLine(ClassBuilderTemplates.CONSTRUCTOR_READER_END);
                file.WriteLine();

                file.WriteLine(ClassBuilderTemplates.TO_STRING_START);
                file.WriteLine(ClassBuilderTemplates.GetStringMethodTemplate(packet));
                file.WriteLine(ClassBuilderTemplates.TO_STRING_END);
                file.WriteLine();

                file.WriteLine("        #region implemented abstract members of TerrariaPacket");
                file.WriteLine();
                file.WriteLine(string.Format(ClassBuilderTemplates.LENGTH_GET_OVERRIDE, ClassBuilderTemplates.GetLengthMethodTemplate(packet)));
                file.WriteLine();

                file.WriteLine(ClassBuilderTemplates.TO_STREAM_START);
                foreach (PacketType type in packet.PacketTypes)
                {
                    file.WriteLine(string.Format(ClassBuilderTemplates.TO_STREAM_WRITE, type.Name));
                }
                file.WriteLine(ClassBuilderTemplates.TO_STREAM_END);

                file.WriteLine();
                file.WriteLine("        #endregion");

                file.WriteLine();
                file.WriteLine(ClassBuilderTemplates.CLASS_END);
            }
        }
    }
}
