using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSQMConsole.Common
{
    public interface IMhQueue
    {
        int Count { get; }
        Guid Id { get; }
        string Label { get; set; }
        string Name { get; set; }
        IMhMessage Peek();
        IMhMessage Peek(string id);
        void Purge();
        IMhMessage Receive();
        IMhMessage Receive(string id);
        IMhMessage Receive(TimeSpan timeout);
        IEnumerable<IMhMessage> GetMessages();
    }
}
