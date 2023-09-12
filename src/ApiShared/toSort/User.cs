using System;
using System.Collections.Generic;

namespace BlazingChat.Shared.Models
{

    public class User
    {
        public long UserId { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

    }
}
