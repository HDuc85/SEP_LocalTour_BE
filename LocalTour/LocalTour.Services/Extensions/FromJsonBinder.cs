using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections;
using System.Text.Json;
namespace LocalTour.Services.Extensions
{
    public class FromJsonBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            var value = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            try
            {
                var modelType = bindingContext.ModelType.GetGenericArguments().First();
                object result;

                // Check if the JSON represents a single object or an array
                if (value.Trim().StartsWith("["))
                {
                    // Deserialize as an array
                    result = JsonSerializer.Deserialize(value, bindingContext.ModelType, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else
                {
                    // Deserialize a single object and wrap it in a list
                    var singleItem = JsonSerializer.Deserialize(value, modelType, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    var listType = typeof(List<>).MakeGenericType(modelType);
                    var list = (IList)Activator.CreateInstance(listType);

                    list.Add(singleItem);
                    result = list;
                }

                bindingContext.Result = ModelBindingResult.Success(result);
            }
            catch (JsonException ex)
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"Invalid JSON: {ex.Message}");
            }

            return Task.CompletedTask;
        }
    }
}