using System;
using System.Collections.Generic;
using System.Text;

namespace FishnChips.Model
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public long ContactNumber { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
