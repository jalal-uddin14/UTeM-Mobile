using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTeM_Mobile.Interfaces
{
    public interface IAppStartupService
    {
        Task RestoreSessionAsync();
    }
}
