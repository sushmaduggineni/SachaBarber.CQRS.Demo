﻿using CQRSalad.EventSourcing;

namespace Samples.Domain.Interface.User
{
    public class CreateUserCommand
    {
        [AggregateId]
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}