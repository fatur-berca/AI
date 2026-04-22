using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DFIS.Utils.Exceptions
{
    public abstract partial class ExceptionCodes
    {
        /// <summary>
        /// Enum ExceptionCodes for user defined error codes
        /// </summary>
        public enum BaseExceptions
        {
            [Description("An unknown error occured")]
            unhandled_exception,

            [Description("Delegation Responsibility Violation")]
            delegation_exception,
        }

        public enum BLLExceptions
        {
            [Description("An unknown error occured")]
            UnhandledException,
            [Description("Code/Key is already exist")]
            KeyExist,
            [Description("Some field is mandatory")]
            Mandatory,
            [Description("Relation key doesn't exist!")]
            NoAvailableKey
        }

        /// <summary>
        /// Security Exceptions for wcf responses
        /// </summary>
        public enum SecurityExceptions
        {
            AccessDenied,
            AuthorizationDenied,
            AuthenticationFailure
        }
    }
}
