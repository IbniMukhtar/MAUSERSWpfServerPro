using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProjectWPF.MVVM
{
   public class User
    {
        public int ID { get; set; }
        public string? FullName { get; set; }
        public DateTime CDate { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? MobileNo { get; set; }
    }
}
