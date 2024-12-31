using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FluentValidation;
using FluentValidation.Results;

namespace Haus.Core.Common;

public class HausValidationException : ValidationException
{
    public HausValidationException(string message) : base(message)
    {
    }

    public HausValidationException(string message, IEnumerable<ValidationFailure> errors) : base(message, errors)
    {
    }

    public HausValidationException(string message, IEnumerable<ValidationFailure> errors, bool appendDefaultMessage) :
        base(message, errors, appendDefaultMessage)
    {
    }

    public HausValidationException(IEnumerable<ValidationFailure> errors) : base(errors)
    {
    }

    [Obsolete("Obsolete")]
    public HausValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}