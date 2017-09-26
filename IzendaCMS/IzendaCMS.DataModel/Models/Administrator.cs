using System;
using System.Collections.Generic;

namespace IzendaCMS.DataModel.Models
{
    public partial class Administrator : User
    {
        //public int Id { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        public Nullable<System.DateTime> HireDate { get; set; }
        //public string UserName { get; set; }
        //public string Password { get; set; }
        //public string UserType { get; set; }

        public override string ToString()
        {
            return $"Administrator ID: {Id}\nName: {LastName}, {FirstName}\nHire Date: {HireDate}\nUser Name: {UserName}\nPassword: {Password}\n";
        }
    }
}
