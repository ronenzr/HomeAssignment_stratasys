using System;
using System.Collections.Generic;
using System.Text;

namespace HomeAssignment.Common.Interfaces
{
    public interface IStatus
    {
        string GetStatus();
        string SetStatus(string status);
    }
}
