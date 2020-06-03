using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.RequestHelp.NewMVCForm.Interface;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp.NewMVCForm.Models
{
    public class RequestHelpRequestStageViewModel : IRequestHelpStepsViewModel
    {
 
        public List<TasksViewModel> Tasks { get; set; }
        public List<RequestorViewModel> Requestors { get; set; }          

        public string TemplateName { get; set; } = "RequestHelpRequestStageViewModel";
        public List<RequestHelpTimeViewModel> Timeframes { get; set; }

    }

    public class RequestorViewModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string IconDark { get; set; }
        public string IconLight { get; set; }
        public string ColourCode { get; set; }     
        public bool IsSelected { get; set; }
    }
    public class TasksViewModel
    {
        public int ID { get; set; }
        public SupportActivities SupportActivity { get; set; }
        public List<RequestHelpQuestion> Questions { get; set; }

        public bool IsSelected { get; set; }
    }

    public class RequestHelpQuestion
    {
        public int ID { get; set; }
        public string Label { get; set; }
        public InputType InputType { get; set; }
        public dynamic Model { get; set; }
    }

    public enum InputType
    {
        Textarea = 1,
        Textbox = 2,
        Number = 3,
    }

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
            RequestHelpRequestStageViewModel model = JsonConvert.DeserializeObject<RequestHelpRequestStageViewModel>(bindingContext.ValueProvider.GetValue("step").FirstValue);

            int selectedTaskId = Convert.ToInt32(bindingContext.ValueProvider.GetValue("currentStep_SelectedTask_Id").FirstValue);
            int selectedRequestorId = Convert.ToInt32(bindingContext.ValueProvider.GetValue("currentStep_SelectedRequestor_Id").FirstValue); 
            int selectedTimeId = Convert.ToInt32(bindingContext.ValueProvider.GetValue("currentStep_SelectedTimeFrame_Id").FirstValue);

            var requestor = model.Requestors.Where(x => x.ID == selectedRequestorId).First();
            requestor.IsSelected = true;

            var task = model.Tasks.Where(x => x.ID == selectedTaskId).First();
            task.IsSelected = true;

            var time = model.Timeframes.Where(x => x.ID == selectedTimeId).First();
            time.IsSelected = true;

            if (time.AllowCustom){
                time.Days = Convert.ToInt32(bindingContext.ValueProvider.GetValue("currentStep_SelectedTimeFrame_CustomDays").FirstValue);
            }
             
            int questionCount = Convert.ToInt32(bindingContext.ValueProvider.GetValue("currentStep_SelectedTask_QuestionCount").FirstValue);                               

            for(int i = 0; i < questionCount; i++)
            {
                int questionID = Convert.ToInt32(bindingContext.ValueProvider.GetValue($"currentStep_SelectedTask_Questions_[{i}]_Id").FirstValue);
                var question = task.Questions.Where(x => x.ID == questionID).First();
                question.Model = bindingContext.ValueProvider.GetValue($"currentStep_SelectedTask_Questions_[{i}]_Model").FirstValue;                                    
            }

            return model;

        }
    }
    
}

