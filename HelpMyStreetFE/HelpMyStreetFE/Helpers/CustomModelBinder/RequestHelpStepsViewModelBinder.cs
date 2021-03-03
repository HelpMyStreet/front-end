using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Models.RequestHelp.Stages.Detail;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;
using HelpMyStreetFE.Models.RequestHelp.Stages.Review;
using HelpMyStreetFE.Services.Requests;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace HelpMyStreetFE.Helpers.CustomModelBinder
{ 
    public class RequestHelpStepsViewModelBinder : IModelBinder
    {
        private readonly IModelMetadataProvider _provider;
        private readonly IRequestHelpBuilder _requestHelpBuilder;
        public RequestHelpStepsViewModelBinder(IModelMetadataProvider provider, IRequestHelpBuilder requestHelpBuilder)
        {
            _provider = provider;
            _requestHelpBuilder = requestHelpBuilder;
        }
        Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
        {
            var stepTypeValue = bindingContext.ValueProvider.GetValue("StepType");
            var stepType = Type.GetType(stepTypeValue.ToString());
            RequestHelpFormVariant requestHelpFormVariant = Enum.Parse<RequestHelpFormVariant>(bindingContext.ValueProvider.GetValue("FormVariant").FirstValue);
            int.TryParse(bindingContext.ValueProvider.GetValue("ReferringGroupId").FirstValue, out int groupId);
            Enum.TryParse<SupportActivities>(bindingContext.ValueProvider.GetValue("SelectedSupportActivity").FirstValue, out SupportActivities selectedSupportActivity);
            bindingContext.ModelMetadata = _provider.GetMetadataForType(stepType);
            switch (stepType.Name)
            {
                case nameof(RequestHelpRequestStageViewModel):
                    bindingContext.Result = ModelBindingResult.Success(BindRequestStage(bindingContext, requestHelpFormVariant, groupId).Result);
                    break;
                case nameof(RequestHelpDetailStageViewModel):
                    bindingContext.Result = ModelBindingResult.Success(BindDetailStage(bindingContext, requestHelpFormVariant, selectedSupportActivity, groupId).Result);
                    break;
                case nameof(RequestHelpReviewStageViewModel):
                    bindingContext.Result = ModelBindingResult.Success(BindReviewStage(bindingContext));
                    break;
            }

            return Task.CompletedTask;
        }

        private RequestHelpReviewStageViewModel BindReviewStage(ModelBindingContext bindingContext)
        {
            RequestHelpReviewStageViewModel model = JsonConvert.DeserializeObject<RequestHelpReviewStageViewModel>(bindingContext.ValueProvider.GetValue("ReviewStep").FirstValue);
            return model;
        }

        private async Task<RequestHelpDetailStageViewModel> BindDetailStage(ModelBindingContext bindingContext, RequestHelpFormVariant requestHelpFormVariant, SupportActivities selectedSupportActivity, int groupId)
        {
            RequestHelpDetailStageViewModel model = JsonConvert.DeserializeObject<RequestHelpDetailStageViewModel>(bindingContext.ValueProvider.GetValue("DetailStep").FirstValue);
            var recpientnamePrefix = "currentStep.Recipient.";
            model.Recipient = new RecipientDetails
            {
                Firstname = bindingContext.ValueProvider.GetValue(recpientnamePrefix + nameof(model.Recipient.Firstname)).FirstValue,
                Lastname = bindingContext.ValueProvider.GetValue(recpientnamePrefix + nameof(model.Recipient.Lastname)).FirstValue ,
                MobileNumber = bindingContext.ValueProvider.GetValue(recpientnamePrefix + nameof(model.Recipient.MobileNumber)).FirstValue,
                AlternatePhoneNumber = bindingContext.ValueProvider.GetValue(recpientnamePrefix + nameof(model.Recipient.AlternatePhoneNumber)).FirstValue,
                AddressLine1 = bindingContext.ValueProvider.GetValue(recpientnamePrefix + nameof(model.Recipient.AddressLine1)).FirstValue ,
                AddressLine2 = bindingContext.ValueProvider.GetValue(recpientnamePrefix + nameof(model.Recipient.AddressLine2)).FirstValue ,
                County = bindingContext.ValueProvider.GetValue(recpientnamePrefix + nameof(model.Recipient.County)).FirstValue ,
                Postcode = bindingContext.ValueProvider.GetValue(recpientnamePrefix + nameof(model.Recipient.Postcode)).FirstValue ,
                Email = bindingContext.ValueProvider.GetValue(recpientnamePrefix + nameof(model.Recipient.Email)).FirstValue,
                Town = bindingContext.ValueProvider.GetValue(recpientnamePrefix + nameof(model.Recipient.Town)).FirstValue,
            };


            var requestorPrefix = "currentStep.Requestor.";
            model.Requestor = new RequestorDetails
            {
                Firstname = bindingContext.ValueProvider.GetValue(requestorPrefix + nameof(model.Recipient.Firstname)).FirstValue ,
                Lastname = bindingContext.ValueProvider.GetValue(requestorPrefix + nameof(model.Recipient.Lastname)).FirstValue ,
                MobileNumber = bindingContext.ValueProvider.GetValue(requestorPrefix + nameof(model.Recipient.MobileNumber)).FirstValue,
                AlternatePhoneNumber = bindingContext.ValueProvider.GetValue(requestorPrefix + nameof(model.Recipient.AlternatePhoneNumber)).FirstValue,         
                Postcode = bindingContext.ValueProvider.GetValue(requestorPrefix + nameof(model.Recipient.Postcode)).FirstValue,
                Email = bindingContext.ValueProvider.GetValue(requestorPrefix + nameof(model.Recipient.Email)).FirstValue,                     
            };

            model.Organisation = bindingContext.ValueProvider.GetValue("currentStep.Organisation").FirstValue;

            model.Questions = new QuestionsViewModel() { Questions = await _requestHelpBuilder.GetQuestionsForTask(requestHelpFormVariant, RequestHelpFormStage.Detail, selectedSupportActivity, groupId) };

            foreach (RequestHelpQuestion question in model.Questions.Questions)
            {
                question.Model = bindingContext.ValueProvider.GetValue($"currentStep.Questions.[{question.ID}].Model").FirstValue;
            }

            return model;
        }

        private async Task<RequestHelpRequestStageViewModel> BindRequestStage(ModelBindingContext bindingContext, RequestHelpFormVariant requestHelpFormVariant, int groupId)
        {
            RequestHelpRequestStageViewModel model = JsonConvert.DeserializeObject<RequestHelpRequestStageViewModel>(bindingContext.ValueProvider.GetValue("RequestStep").FirstValue);

            int selectedTaskId, selectedRequestorId, selectedTimeId = -1;

            int.TryParse(bindingContext.ValueProvider.GetValue("currentStep.SelectedTask.Id").FirstValue, out selectedTaskId);
            int.TryParse(bindingContext.ValueProvider.GetValue("currentStep.SelectedRequestor.Id").FirstValue, out selectedRequestorId);
            int.TryParse(bindingContext.ValueProvider.GetValue("currentStep.SelectedTimeFrame.Id").FirstValue, out selectedTimeId);

            model.Requestors.ForEach(x => x.IsSelected = false);
            var requestor = model.Requestors.Where(x => x.ID == selectedRequestorId).FirstOrDefault();
            if (requestor != null)
            {
                requestor.IsSelected = true;
            }

            model.Tasks.ForEach(x => x.IsSelected = false);
            var task = model.Tasks.Where(x => (int)x.SupportActivity == selectedTaskId).FirstOrDefault();
            if (task != null)
            {
                task.IsSelected = true;
            }

            model.Timeframes.ForEach(x => x.IsSelected = false);
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
                if (time.DueDateType.HasDate())
                {
                    time.Date = DateTime.ParseExact(bindingContext.ValueProvider.GetValue("currentStep.SelectedTimeFrame.Date").ToString(), DatePickerHelpers.DATE_PICKER_DATE_FORMAT, new CultureInfo("en-GB"));
                    if (time.DueDateType.HasStartTime())
                    {
                        time.StartTime = ParseTime(time.Date, bindingContext.ValueProvider.GetValue("currentStep.SelectedTimeFrame.StartTime").ToString());
                    }
                    if (time.DueDateType.HasEndTime())
                    {
                        time.EndTime = ParseTime(time.Date, bindingContext.ValueProvider.GetValue("currentStep.SelectedTimeFrame.EndTime").ToString());
                        if (time.StartTime >= time.EndTime)
                        {
                            time.EndTime = time.EndTime.AddDays(1);
                        }
                    }
                }
            }

            model.Questions = new QuestionsViewModel() { Questions = await _requestHelpBuilder.GetQuestionsForTask(requestHelpFormVariant, RequestHelpFormStage.Request, task.SupportActivity, groupId) };

            foreach (RequestHelpQuestion question in model.Questions.Questions)
            {
                question.Model = bindingContext.ValueProvider.GetValue($"currentStep.Questions.[{question.ID}].Model").FirstValue;
            }

            bool AgreeToPrivacyAndTerms = false;            
            bool.TryParse(bindingContext.ValueProvider.GetValue("currentStep." + nameof(model.AgreeToPrivacyAndTerms)).FirstValue, out AgreeToPrivacyAndTerms);
            model.AgreeToPrivacyAndTerms = AgreeToPrivacyAndTerms;

            return model;

        }

        private DateTime ParseTime(DateTime date, string value)
        {
            var components = value.Split(':');
            if (components.Count() >= 2)
            {
                int.TryParse(components[0], out int hours);
                int.TryParse(components[1], out int minutes);
                return date.AddHours(hours).AddMinutes(minutes);
            }
            throw new FormatException($"Badly formatted time string '{value}'");
        }
    }

}
