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
        public Operation(BooksModels newItem,string operationType)
        {
            this.body = newItem;
            this.OperationType = operationType;
        }
        [DataMember]
        public BooksModels body;
        [DataMember]
        private string operationType;
       
        public string OperationType
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
            return "Author: " + body.author + "Title: " + body.title;
        }
    }
}
