using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTeM_Mobile.Models
{
    public class PatrolLocation
    {
        public string UserId { get; set; }
        public int PatrolId { get; set; }
        public Location Location { get; set; }
    }
}
