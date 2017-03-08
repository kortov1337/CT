using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Client1.Models;

namespace DAL
{
    [DataContract]
    public class Operation
    {
        public Operation(BooksModels newItem,string operationType, int id)
        {
            this.body = newItem;
            this.operationType = operationType;
            this.Id = id;
        }
        [DataMember]
        internal BooksModels body;
        [DataMember]
        internal string operationType;
        [DataMember]
        internal int Id =-1;
    }
}
