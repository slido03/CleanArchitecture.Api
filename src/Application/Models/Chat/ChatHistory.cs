﻿using CleanArchitecture.Application.Interfaces.Chat;
using System;

namespace CleanArchitecture.Application.Models.Chat
{
    public partial class ChatHistory<TUser> : IChatHistory<TUser> where TUser : IChatUser
    {
        public long Id { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual TUser FromUser { get; set; }
        public virtual TUser ToUser { get; set; }
    }
}