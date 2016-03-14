﻿using CQRSalad.EventSourcing;

namespace Samples.Domain.Interface.User
{
    public sealed class AddFollowerCommand
    {
        [AggregateId]
        public string UserId { get; set; }
        public string FollowerUserId { get; set; }
    }
}