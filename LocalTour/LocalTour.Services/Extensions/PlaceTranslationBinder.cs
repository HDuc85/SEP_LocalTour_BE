using LocalTour.Domain.Entities;
using LocalTour.Services.ViewModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Extensions
{
    public class PlaceTranslationBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult != ValueProviderResult.None)
            {
                var json = valueProviderResult.FirstValue;
                if (json != null)
                {
                    var placeTranslations = JsonConvert.DeserializeObject<List<PlaceTranslationRequest>>(json);
                    bindingContext.Result = ModelBindingResult.Success(placeTranslations);
                }
            }

            return Task.CompletedTask;
        }
    }
}
