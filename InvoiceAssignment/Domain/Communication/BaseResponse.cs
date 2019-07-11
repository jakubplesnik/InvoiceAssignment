using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceAssignment.Domain.Communication
{
    public class BaseResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public BaseResponse(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public BaseResponse(int code)
        {
            Code = code;

            switch (code)
            {
                case 401:
                    Message = "Authentication required. Please include a valid API key in the 'X-Api-Key' header of the request.";
                    break;
                case 403:
                    Message = "Forbidden access.";
                    break;
                default:
                    Code = 500;
                    Message = "Message for specified status code not found.";
                    break;
            }
        }
    }
}
