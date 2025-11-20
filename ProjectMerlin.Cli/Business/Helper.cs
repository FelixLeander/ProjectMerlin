using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMerlin.Cli.Business;

internal static class Helper
{
    public static T? PopulateInstanceFromConsole<T>() where T : class, new() // type constraints not really reuired.
    {
        var type = typeof(T);
        var instance = Activator.CreateInstance<T>();

        var propertyInfos = type.GetProperties()
            .Where(w => w.GetCustomAttribute<RequiredAttribute>() != null)
            .ToArray();

        if (propertyInfos.Length == 0)
            throw new NotImplementedException($"The class '{type.FullName}', does not have a property decorated with '{nameof(RequiredAttribute)}'");

        Console.WriteLine($"Enter values for the '{type.Name}'.");
        foreach (var pi in propertyInfos)
        {
            Console.WriteLine($"Enter a {pi.PropertyType.Name} for {pi.Name}.");
            while (true)
            {
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine($"{pi.Name} is required. Please enter a value.");
                    continue;
                }

                try
                {
                    var value = Convert.ChangeType(input, pi.PropertyType);
                    pi.SetValue(instance, value);
                    break;
                }
                catch
                {
                    Console.WriteLine($"The value '{input}' is invalid for the type {pi.PropertyType.Name}.");
                }
            }
        }

        return null;
    }
}
