using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using Microsoft.PowerShell;

namespace MakePolicyFromApp.Services;

public class Powershell : IPowershell
{
    public async Task ExecuteCommandAsync(
        string command,
        IDictionary<string, object> arguments,
        IEnumerable<string> modules
    )
    {
        // Create a default initial session state and set the execution policy.
        InitialSessionState initialSessionState = InitialSessionState.CreateDefault();
        initialSessionState.ExecutionPolicy = ExecutionPolicy.Unrestricted;

        // Create a runspace and open it. This example uses C#8 simplified using statements
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

        await runtime.InvokeAsync().ConfigureAwait(false);

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
