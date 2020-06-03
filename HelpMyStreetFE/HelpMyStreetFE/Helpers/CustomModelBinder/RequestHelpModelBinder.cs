using HelpMyStreetFE.Models.RequestHelp.NewMVCForm.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Helpers.CustomModelBinder
{
    public class RequestHelpModelBinder : IModelBinder
    {
        private readonly IModelMetadataProvider _provider;
        public RequestHelpModelBinder(IModelMetadataProvider provider)
        {
            _provider = provider;
        }
        Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
        {

            RequestHelpNewViewModel model = JsonConvert.DeserializeObject<RequestHelpNewViewModel>(bindingContext.ValueProvider.GetValue("requestHelp").FirstValue, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });
            var type = Type.GetType(model.ToString());
            bindingContext.ModelMetadata = _provider.GetMetadataForType(type);
            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }
}
