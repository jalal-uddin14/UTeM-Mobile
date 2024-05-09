using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTeM_Mobile.Data.Models;

namespace UTeM_Mobile.Models
{
    public class PatrolLocation
    {
        public string UserId { get; set; }
        public int PatrolId { get; set; }
        public Location Location { get; set; }
        public ApplicationUser User { get; set; }
    }
}
