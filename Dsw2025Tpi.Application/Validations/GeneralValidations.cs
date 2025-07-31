using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Dsw2025Tpi.Application.Validations;

public static class GeneralValidations
{
    public static void ValidateNotNull<T>(T obj, string paramName)
    {
        if (obj == null)
            throw new ArgumentException($"{paramName} cannot be null");
        if(string.IsNullOrWhiteSpace(obj.ToString())
            && obj is not IFormFile
            && obj is not IEnumerable<string>
            && obj is not IEnumerable<int>
            && obj is not IEnumerable<Guid>)
        {
            throw new ArgumentException($"{paramName} cannot be empty or whitespace");
        }
    }

    public static void ValidateText(string text, string paramName)
    {
        ValidateNotNull(text, paramName);
        if (!Regex.IsMatch(text, @"^[a-zA-Z0-9 ]*$"))
        {
            throw new ArgumentException($"{paramName}: invalid input, has to be an string of letters and/or numbers");
        }
    }
    public static void ValidateOptionalText(string text, string paramName)
    {
        if (!Regex.IsMatch(text, @"^[a-zA-Z0-9\s,.\-°º#]*$"))
        {
            throw new ArgumentException($"{paramName}: invalid input, has to be an string of letters, numbers or certain symbols (.\\-°º#)");
        }
    }
    public static void ValidatePositiveWholeNumberAndCero(string number, string paramName)
    {
        ValidateNotNull(number, paramName);
        if (!Regex.IsMatch(number, @"^(0|[1-9]\d*)$"))
        {
            throw new ArgumentException($"{paramName} invalid input, has to be a whole number or cero");
        }
    }

    public static void ValidatePositiveDecimalNumber(string number, string paramName)
    {
        ValidateNotNull(number, paramName);
        if (!Regex.IsMatch(number, @"^[0-9][0-9,\.]*$"))
        {
            throw new ArgumentException($"{paramName} invalid input, has to be a positive decimal number");
        }
    }
    public static void ValidateGuidAndCodes(string text, string paramName)
    {
        ValidateNotNull(text, paramName);
        if (!Regex.IsMatch(text, @"^[a-zA-Z0-9-\-]*$"))
        {
            throw new ArgumentException($"{paramName}: invalid guid");
        }
    }
}
