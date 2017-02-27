using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSQMConsole.Common
{
    public interface IMhMessage
    {
        object Body { get; set; }
        string Id { get; set; }
        string Label { get; set; }
        DateTime ArrivedTime { get; set; }
        DateTime SentTime { get; set; }
    }
}
