using System.Collections.Generic;
using System.Linq;
using DFIS.Universal.Domain.Inputs;
using System;

namespace hms_tom_dev.Models.Common
{
    public class PageResult<T> where T : class
    {
        public List<T> Results { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        public PageResult()
        {

        }

        public PageResult(List<T> list, BaseInput criteria)
        {
            TotalRecords = list.Count;
            TotalPages = (list.Count / criteria.PageSize) + (list.Count % criteria.PageSize != 0 ? 1 : 0);
            Results = list.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize).ToList();
        }

        public PageResult(List<T> list)
        {
            TotalRecords = list.Count;
            TotalPages = 1;
            Results = list.ToList();
        }
    }

    public class NonQueryResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public ReducedException Exception { get; set; }
        public object Data { get; set; }

        public NonQueryResult()
        {
            ErrorMessage = null;
            IsSuccess = true;
        }
        public NonQueryResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
            IsSuccess = errorMessage == null;
        }
        public NonQueryResult(bool isSuccess, string errorMessage = null)
        {
            ErrorMessage = errorMessage;
            IsSuccess = isSuccess;
        }
        public NonQueryResult(bool isSuccess, Exception exception, string message = null)
        {
            ErrorMessage = message ?? exception.Message;
            IsSuccess = isSuccess;
            Exception = new ReducedException(exception);
        }
    }

    public class ReducedException
    {
        public string Message;
        public string StackTrace;
        public ReducedException InnerException;

        public ReducedException(Exception exception)
        {
            Message = exception.Message;
            StackTrace = exception.StackTrace;
            if (exception.InnerException != null)
                InnerException = new ReducedException(exception.InnerException);
        }
    }
}