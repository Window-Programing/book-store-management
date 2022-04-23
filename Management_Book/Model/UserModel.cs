using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Management_Book.Model
{
    internal class UserModel
    {
        public class Account
        {
            public int Id { get; set; }

            public string UserName { get; set; }

            public string Password { get; set; }

            public string Entropy { get; set; }

        }
    }
}
