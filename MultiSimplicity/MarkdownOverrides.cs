using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaToMultiplicity
{
    class MarkdownOverrides
    {
        public Dictionary<byte, string> ForceFail;

        public Dictionary<byte, string> ForceContinue;

        public MarkdownOverrides()
        {
            ForceFail = new Dictionary<byte, string>();
            ForceFail.Add(72, "Packet has abnormal size and type");
            ForceFail.Add(10, "Packet has no type info");
            ForceFail.Add(67, "Plugin packet");
            ForceFail.Add(15, "Null packet");

            ForceContinue = new Dictionary<byte, string>();
            ForceContinue.Add(4, "Player Info");
            ForceContinue.Add(5, "Player Inventory");
        }
    }
}
