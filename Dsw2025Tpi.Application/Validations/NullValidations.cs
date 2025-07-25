using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Validations;

public static class NullValidations
{
    public static void ValidateNotNull<T>(T obj, string paramName)
    {
        if (obj == null)
            throw new ArgumentNullException($"{paramName} cannot be null");
    }
}
