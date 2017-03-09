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
            this.OperationType = operationType;
            this.Id = id;
        }
        [DataMember]
        internal BooksModels body;
        [DataMember]
        private string operationType;
        [DataMember]
        internal int Id =-1;

        internal string OperationType
        {
            get
            {
                return operationType;
            }

            set
            {
                operationType = value;
            }
        }
        public override string ToString()
        {
            return "Author: " + body.author + " Title: " + body.title;
        }
    }
}
