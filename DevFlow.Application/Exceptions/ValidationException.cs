using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DevFlow.Application.Exceptions
{
    public class ValidationException:Exception
    {
        public List<String> Errors { get;  }
        public ValidationException(List<String>errors) : base("One or more validation errors occurred.") {
            Errors = errors;
        }
    }
}
