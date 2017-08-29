using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaToMultiplicity
{
    class TypeChecker
    {
        public Dictionary<string, byte> TypeSizes = new Dictionary<string, byte>();

        public TypeChecker()
        {
			TypeSizes.Add("Byte", 1);
			TypeSizes.Add("SByte", 1);
			TypeSizes.Add("Int16", 2);
			TypeSizes.Add("UInt16", 2);
			TypeSizes.Add("Int32", 4);
			TypeSizes.Add("UInt32", 4);
			TypeSizes.Add("Int64", 8);
			TypeSizes.Add("UInt64", 8);
            TypeSizes.Add("Boolean", 1);
            TypeSizes.Add("Single", 4);
			TypeSizes.Add("String", 0);
			TypeSizes.Add("NetworkText", 0);
            TypeSizes.Add("Color", 3);
        }
    }
}
