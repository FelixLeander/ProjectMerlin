using System.Reflection;

namespace ProjectMerlin.Cli.Business;

[AttributeUsage(AttributeTargets.Property)]
public sealed class CliCreatorAttribute(string consoleText) : Attribute
{
    public string ConsoleText { get; } = consoleText;

    public static T? CreateFromCli<T>() where T : class, new()
    {
        var instance = new T();
        foreach (var propertyInfo in typeof(T).GetProperties())
        {
            var attribute = propertyInfo.GetCustomAttribute<CliCreatorAttribute>();
            if (attribute == null)
                continue;

            Console.WriteLine(attribute.ConsoleText);
            var rawInput = Console.ReadLine();
            try
            {
                propertyInfo.SetValue(instance, rawInput);
            }
            catch
            {
                Console.WriteLine($"Value '{rawInput}' is invalid for {propertyInfo.Name} it has to be of type {propertyInfo.PropertyType.Name}.");
                return null;
            }
        }

        return instance;
    }
}
