using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.PowerShell;
using ObjectPrinter;

namespace MakePolicyFromApp.Services;

public class Powershell : IPowershell
{
    private readonly ILogger<IPowershell> _logger;

    public Powershell(ILogger<IPowershell> logger)
    {
        _logger = logger;
    }

#pragma warning disable MA0051 // Method is too long
    public async Task ExecuteCommandAsync(
#pragma warning restore MA0051 // Method is too long
        string command,
        IDictionary<string, object> arguments,
        IEnumerable<string> modules
    )
    {
        // Create a default initial session state and set the execution policy.
        InitialSessionState initialSessionState = InitialSessionState.CreateDefault();
        initialSessionState.ExecutionPolicy = ExecutionPolicy.Unrestricted;

        // Create a runspace and open it.
        using Runspace runspace = RunspaceFactory.CreateRunspace(initialSessionState);
        runspace.Open();

        using var runtime = PowerShell.Create(runspace);
        runtime.Runspace.SessionStateProxy.SetVariable("ErrorActionPreference", "stop");

        foreach (string module in modules)
        {
            runtime.AddCommand("Import-Module");
            runtime.AddParameter("Name", module);
            runtime.AddParameter("Scope", "Local");
            runtime.AddStatement();
        }

        runtime.AddCommand(command);
        foreach (var (paramName, value) in arguments)
        {
            if (value == IPowershell.Void)
            {
                runtime.AddParameter(paramName);
            }
            else
            {
                runtime.AddParameter(paramName, value);
            }
        }

        runtime.AddStatement();

        try
        {
            await runtime.InvokeAsync().ConfigureAwait(false);
        }
        catch (RuntimeException runtimeException)
        {
            _logger.Log(
                LogLevel.Error,
                "Runtime exception: {0}: {1}\n{2}\n{3}",
                runtimeException.ErrorRecord.InvocationInfo.InvocationName,
                runtimeException.Message,
                runtimeException.ErrorRecord.Exception.DumpToString(),
                runtimeException.ErrorRecord.ErrorDetails.DumpToString()
            );
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Error, $"Failed to invoke powershell session: {e.Message}\n{e}");
        }
        finally
        {
            if (runtime.HadErrors)
            {
                var stdError = String.Join(
                    Environment.NewLine,
                    runtime.Streams.Error.Select((o) => o.ToString())
                );

                throw new Exception($"Failed to execute powershell command {command}: " + stdError);
            }
        }
    }
}
