using ProjectMerlin.Cli.Business;

try
{
    return await ConsoleMenues.MainMenuAsync();
}
catch (Exception ex)
{
    Console.WriteLine("Unexpected error, exiting application:");
    Console.WriteLine(ex.Message);
    return 1;
}
