﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string targetEmail, string subject, string message);
    }
}
