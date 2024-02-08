using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToKorean
{
    internal interface ILastActionEvent
    {
        event EventHandler<EventArgs> LastActionEvent;
    }
}
