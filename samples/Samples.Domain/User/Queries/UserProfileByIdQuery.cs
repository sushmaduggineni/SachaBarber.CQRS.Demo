﻿using CQRSalad.Domain;

namespace Samples.Domain.Interface.User
{
    public class UserProfileByIdQuery : IQuery<UserProfile>
    {
        public string UserId { get; set; }
    }
}
