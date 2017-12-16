using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnJiaWebServer_V1.Models
{
    public class Enrollment
    {
    
        public int EnrollmentID { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }


        public Users User { get; set; }
        public Groups Group { get; set; }
    }
}
