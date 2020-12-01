using System;

namespace Maincotech.Domain
{
    public class Person : EntityBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string NickName { get; set; }
        public bool? Gender { get; set; }
        public DateTime? Birthday { get; set; }
    }
}