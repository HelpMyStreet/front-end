﻿using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HelpMyStreetFE.Models.RequestHelp.Stages.Request
{
    [RequestStageValidation]
    public class RequestHelpRequestStageViewModel : IRequestHelpStageViewModel
    {
        public Guid RequestGuid { get; set; }
        public string PageHeading { get; set; }
        public string IntoText { get; set; }
        public List<TasksViewModel> Tasks { get; set; }
        public List<RequestorViewModel> Requestors { get; set; }
        public QuestionsViewModel Questions { get; set; } = new QuestionsViewModel();
        public string FriendlyName { get; set; } = "Request";
        public string TemplateName { get; set; } = "RequestHelpRequestStageViewModel";
        public List<RequestHelpTimeViewModel> Timeframes { get; set; }
        public bool AgreeToPrivacyAndTerms { get; set; }
    }
}

