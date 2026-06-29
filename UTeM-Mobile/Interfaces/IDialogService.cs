using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTeM_Mobile.Interfaces
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string title, string message, string cancel = "OK");

        Task<bool> ShowConfirmationAsync(
            string title,
            string message,
            string accept = "Yes",
            string cancel = "No");
    }
}
