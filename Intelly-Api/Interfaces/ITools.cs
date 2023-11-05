﻿using Intelly_Api.Entities;
using System.Data;

namespace Intelly_Api.Interfaces
{
    public interface ITools
    {
        String CreatePassword(int length);
        bool SendEmail(string recipient, string subject, string body);
        public string GenerateToken(string userId);
        string MakeHtmlNewUser(UserEnt userData, string temporalPassword);
        string Encrypt(string texto);
        string Decrypt(string texto);
    }
}
