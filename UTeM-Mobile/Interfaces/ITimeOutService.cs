using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTeM_Mobile.Core.Models;

namespace UTeM_Mobile.Interfaces
{
    public interface ITimeOutService
    {
        Task RunLocationBroadcastAsync(AuthToken token);
        Task CheckTimerToken();
    }
}
