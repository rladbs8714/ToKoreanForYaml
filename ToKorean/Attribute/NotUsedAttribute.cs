using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToKorean.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method)]
    internal class NotUsedAttribute : System.Attribute
    {
        public NotUsedAttribute() { }
    }
}
