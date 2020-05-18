﻿using HelpMyStreetFE.Enums.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models
{
    public class NotificationModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string  Subtitle { get; set; }
        public string Message { get; set; }
        public string Button { get; set; }
        public NotificationType Type { get; set; }
    }
}
