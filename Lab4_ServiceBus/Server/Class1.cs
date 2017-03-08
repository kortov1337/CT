using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    [DataContract]
    internal class Class1
    {
            [DataMember]
            internal string name;

            [DataMember]
            internal int age;
    }
}
