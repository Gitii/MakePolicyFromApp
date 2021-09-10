using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Microsoft.PowerShell;

namespace MakePolicyFromApp.Services
{
    public interface IPowershell
    {
        public Task ExecuteCommand(string command, IDictionary<string, object> arguments, IEnumerable<string> modules);

        public static readonly object VOID = new object();
    }

    public class Powershell: IPowershell
    {
        public async Task ExecuteCommand(string command, IDictionary<string, object> arguments,
            IEnumerable<string> modules)
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
                if (value == IPowershell.VOID)
                {
                    runtime.AddParameter(paramName);
                }
                else
                {
                    runtime.AddParameter(paramName, value);
                }
                
            }
            runtime.AddStatement();

            var result = await runtime.InvokeAsync();

            if (runtime.HadErrors)
            {
                var stdError = String.Join(Environment.NewLine, runtime.Streams.Error.Select((o) => o.ToString()));

                throw new Exception($"Failed to execute powershell command {command}: " + stdError);
            }
        }
    }
}
