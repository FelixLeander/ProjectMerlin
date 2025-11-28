using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace ProjectMerlin.Cli.Business;

internal static class Helper
{
    internal sealed record InteractiveCliParser(Type TargetType, Func<(bool Cancel, object? Result)> InteractiveConvertLogic);

    public static readonly InteractiveCliParser DefaultLogic = new(typeof(string), () =>
    {
        try
        {
            var rawInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(rawInput))
                return (true, null);

            return (false, rawInput);
        }
        catch
        {
            return (false, null);
        }
    });

    public static readonly InteractiveCliParser HexColor = new(typeof(Color), () =>
    {
        try
        {
            Console.WriteLine("Also accepts HEX-Color.");

            var rawInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(rawInput))
                return (true, null);

            var color = ColorTranslator.FromHtml(rawInput);
            return (false, color);
        }
        catch
        {
            return (false, null);
        }
    });

    public static T PopulateInstanceFromConsole<T>(params InteractiveCliParser[] customCliParsers) where T : class, new() // type constraints not really reuired.
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
                var interactiveCliParser = customCliParsers.FirstOrDefault(f => f.TargetType == type) ?? DefaultLogic;
                var (cancel, result) = interactiveCliParser.InteractiveConvertLogic.Invoke();
                if (cancel)
                {
                    Console.Write($"{pi.Name} is required. Please enter a value. ");
                    continue;
                }

                if (result == null)
                    break;

                try
                {
                    var value = Convert.ChangeType(result, pi.PropertyType);
                    pi.SetValue(instance, value);
                    break;
                }
                catch
                {
                    Console.WriteLine($"The value is invalid for the type {pi.PropertyType.Name}. Try again.");
                }
            }
        }

        return instance;
    }
}
