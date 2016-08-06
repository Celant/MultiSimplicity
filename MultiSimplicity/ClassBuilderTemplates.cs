using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaToMultiplicity
{
    class ClassBuilderTemplates
    {
        public static string PACKET_HEADER = @"using System;
using System.IO;
using System.Drawing;

namespace LibMultiplicity.Packets
{";

        public static string CLASS_START = @"    /// <summary>
    /// The {0} (0x{1:X}) packet.
    /// </summary>
    public class {0} : TerrariaPacket
    {{";

        public static string PROPERTY = @"        public {0} {1} {{ get; set; }}";

        public static string PROPERTY_COMMENT = @"        /// <summary>
        /// Gets or sets the {1} - {2}
        /// </summary>
        public {0} {1} {{ get; set; }}";

        public static string CONSTRUCTOR = @"        /// <summary>
        /// Initializes a new instance of the <see cref=""LibMultiplicity.Packets.{0}""/> class.
        /// </summary>
        public {0}()
            : base((byte)PacketTypes.{0})
        {{

        }}";

        public static string CONSTRUCTOR_READER_START = @"        /// <summary>
        /// Initializes a new instance of the <see cref=""LibMultiplicity.Packets.{0}""/> class.
        /// </summary>
        /// <param name=""br"">br</param>
        public {0}(BinaryReader br)
            : base(br)
        {{";

        public static string CONSTRUCTOR_READER_PROPERTY = @"            this.{0} = br.Read{1}();";

        public static string CONSTRUCTOR_READER_END = @"        }";

        public static string LENGTH_GET_OVERRIDE = @"        public override short GetLength()
        {{
            {0}
        }}";

        public static string TO_STREAM_START = @"        public override void ToStream(Stream stream, bool includeHeader = true)
        {
            /*
             * Length and ID headers get written in the base packet class.
             */
            if (includeHeader) {
                base.ToStream(stream, includeHeader);
            }

            /*
             * Always make sure to not close the stream when serializing.
             * 
             * It is up to the caller to decide if the underlying stream
             * gets closed.  If this is a network stream we do not want
             * the regressions of unconditionally closing the TCP socket
             * once the payload of data has been sent to the client.
             */
            using (BinaryWriter br = new BinaryWriter(stream, new System.Text.UTF8Encoding(), leaveOpen: true)) {";

        public static string TO_STREAM_WRITE = @"                br.Write({0});";

        public static string TO_STREAM_END = @"            }
        }";

        public static string TO_STRING_START = @"        public override string ToString()
        {";

        public static string TO_STRING_WRITE = @"            return string.Format(""[{0}:{1}]"", {2});";

        public static string TO_STRING_END = @"        }";

        public static string CLASS_END = @"    }
}";

        public static string GetLengthMethodTemplate(TerrariaPacket packet)
        {
            List<string> stringProperties = new List<string>();
            int typeLength = 0;
            string lengthString = @"return (short)({0}{1});";
            TypeChecker typeChecker = new TypeChecker();

            foreach (PacketType type in packet.PacketTypes)
            {
                if (type.Type.ToLower() == "string")
                {
                    stringProperties.Add(type.Name);
                }
                else
                {
                    typeLength += typeChecker.TypeSizes[type.Type];
                }
            }

            if (stringProperties.Count() > 0)
            {
                string stringLengths = "";
                foreach (string property in stringProperties)
                {
                    stringLengths += " + " + property + ".Length";
                    typeLength += 1;
                }
                return string.Format(lengthString, typeLength, stringLengths);
            }
            else
            {
                return string.Format(lengthString, typeLength, string.Empty);
            }
        }

        public static string GetStringMethodTemplate(TerrariaPacket packet)
        {
            string templateArgsString = string.Join(", ", packet.PacketTypes.Select(x => x.Name).ToArray());
            string templateString = "";

            for (int i = 0; i < packet.PacketTypes.Count; i++)
            {
                templateString += string.Format(" {0} = {{{1}}}", packet.PacketTypes[i].Name, i);
            }

            return string.Format(TO_STRING_WRITE, packet.PacketName, templateString, templateArgsString);
        }
    }
}
