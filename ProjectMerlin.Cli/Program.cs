using ProjectMerlin.Cli.Business;
using System.Drawing;


var coolor = ColorTranslator.FromHtml("#0496C7");

return 7;
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
