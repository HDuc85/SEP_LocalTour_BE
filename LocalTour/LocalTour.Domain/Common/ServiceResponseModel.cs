using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Services.Model 
{
    public class ServiceResponseModel<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;

        public ServiceResponseModel(T data)
        {
            Data = data;
        }
        public ServiceResponseModel(bool success, string message)
        {
            Success = success;
            Message = message;
        }
        
        public ServiceResponseModel(bool success, T data)
        {
            Success = success;
            Data = data;
        }
        
    }
}
