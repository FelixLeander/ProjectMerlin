using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace ProjectMerlin.Cli.Business;

internal static class Helper
{
    internal sealed record InteractiveCliParser(Type TargetType, Func<(bool, object?)> InteractiveConvertLogic);

    public static readonly InteractiveCliParser DefaultLogic = new InteractiveCliParser(typeof(object), () =>
    {
        return new { };
    });

    public static readonly InteractiveCliParser HexColor = new InteractiveCliParser(typeof(Color), () =>
    {
        try
        {
            Console.WriteLine("Also accepts HEX-Color.");

            var rawInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(rawInput))
                return (true, null);

            var color = ColorTranslator.FromHtml(rawInput);
            return (true, color);
        }
        catch
        {
            // TODO: Change return type to handle empty and cancel
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
            string? rawInput = null;
            while (true)
            {
                var interactiveCliParser = customCliParsers.FirstOrDefault(f => f.TargetType == type) ?? DefaultLogic;
                var result = interactiveCliParser.InteractiveConvertLogic.Invoke();
                if (result == null)
                {
                    Console.Write($"{pi.Name} is required. Please enter a value. ");
                    continue;
                }

                try
                {
                    var value = resultObject ?? Convert.ChangeType(rawInput, pi.PropertyType);
                    pi.SetValue(instance, value);
                    break;
                }
                catch
                {
                    Console.WriteLine($"The value '{rawInput}' is invalid for the type {pi.PropertyType.Name}. Try again.");
                }
            }
        }

        return instance;
    }
}
