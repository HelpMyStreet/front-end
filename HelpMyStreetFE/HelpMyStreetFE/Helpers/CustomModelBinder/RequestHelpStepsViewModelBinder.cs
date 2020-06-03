﻿using HelpMyStreetFE.Models.RequestHelp.NewMVCForm.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Helpers.CustomModelBinder
{ 
    public class RequestHelpStepsViewModelBinder : IModelBinder
    {
        private readonly IModelMetadataProvider _provider;
        public RequestHelpStepsViewModelBinder(IModelMetadataProvider provider)
        {
            _provider = provider;
        }
        Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
        {
            var stepTypeValue = bindingContext.ValueProvider.GetValue("StepType");
            var stepType = Type.GetType(stepTypeValue.ToString());
            bindingContext.ModelMetadata = _provider.GetMetadataForType(stepType);
            switch (stepType.Name)
            {
                case nameof(RequestHelpRequestStageViewModel):
                    bindingContext.Result = ModelBindingResult.Success(BindRequestStage(bindingContext));
                    break;
            }

            return Task.CompletedTask;
        }



        private RequestHelpRequestStageViewModel BindRequestStage(ModelBindingContext bindingContext)
        {
            RequestHelpRequestStageViewModel model = JsonConvert.DeserializeObject<RequestHelpRequestStageViewModel>(bindingContext.ValueProvider.GetValue("RequestStep").FirstValue);

            int selectedTaskId, selectedRequestorId, selectedTimeId = -1;

            int.TryParse(bindingContext.ValueProvider.GetValue("currentStep.SelectedTask.Id").FirstValue, out selectedTaskId);
            int.TryParse(bindingContext.ValueProvider.GetValue("currentStep.SelectedRequestor.Id").FirstValue, out selectedRequestorId);
            int.TryParse(bindingContext.ValueProvider.GetValue("currentStep.SelectedTimeFrame.Id").FirstValue, out selectedTimeId);

            var requestor = model.Requestors.Where(x => x.ID == selectedRequestorId).FirstOrDefault();
            if (requestor != null)
            {
                requestor.IsSelected = true;
            }

            var task = model.Tasks.Where(x => x.ID == selectedTaskId).FirstOrDefault();
            if (task != null)
            {
                task.IsSelected = true;
            }

            var time = model.Timeframes.Where(x => x.ID == selectedTimeId).FirstOrDefault();
            if (time != null)
            {
                time.IsSelected = true;
                if (time.AllowCustom)
                {
                    int selectedDays = -1;
                    int.TryParse(bindingContext.ValueProvider.GetValue("currentStep.SelectedTimeFrame.CustomDays").FirstValue, out selectedDays);
                    time.Days = selectedDays;
                }
            }

            int questionCount = -1;
            int.TryParse(bindingContext.ValueProvider.GetValue("currentStep.SelectedTask.QuestionCount").FirstValue, out questionCount);

            for (int i = 0; i < questionCount; i++)
            {
                int questionID = -1;
                int.TryParse(bindingContext.ValueProvider.GetValue($"currentStep.SelectedTask_Questions_[{i}]_Id").FirstValue, out questionID);
                var question = task.Questions.Where(x => x.ID == questionID).FirstOrDefault();
                if (question != null)
                {
                    question.Model = bindingContext.ValueProvider.GetValue($"currentStep.SelectedTask.Questions.[{i}].Model").FirstValue;
                }
            }

            bool IsHealthCritical, AgreeToTerms, AgreeToPrivacy = false;
            model.IsHealthCritical = bool.TryParse(bindingContext.ValueProvider.GetValue("currentStep.IsHealthCritical").FirstValue, out IsHealthCritical) ? IsHealthCritical : (bool?)null;
            bool.TryParse(bindingContext.ValueProvider.GetValue("currentStep.AgreeToPrivacy").FirstValue, out AgreeToPrivacy);
            bool.TryParse(bindingContext.ValueProvider.GetValue("currentStep.AgreeToTerms").FirstValue, out AgreeToTerms);
            model.AgreeToPrivacy = AgreeToPrivacy;
            model.AgreeToTerms = AgreeToTerms;

            return model;

        }
    }

}
