﻿namespace CloudFileStorage.Models.DTOs
{
    public class AuthRequest
    {
        public required string username { get; set; }
        public required string password { get; set; }
    }
}
