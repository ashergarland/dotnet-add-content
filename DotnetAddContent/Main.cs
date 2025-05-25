using System.CommandLine;
using DotnetAddContent.Commands;

return await AddContentCommand.Build().InvokeAsync(args);
