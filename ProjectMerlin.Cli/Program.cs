using ProjectMerlin.Cli.Business;
using ProjectMerlin.Cli.Enums;
using ProjectMerlin.Core;

try
{
    return await Interactive.MainMenuAsync();
}
catch (Exception ex)
{
    Console.WriteLine("Unexpected error, exiting application:");
    Console.WriteLine(ex.Message);
    return 1;
}
