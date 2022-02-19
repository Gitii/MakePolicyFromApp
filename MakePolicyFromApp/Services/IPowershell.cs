using System.Collections.Generic;
using System.Threading.Tasks;

namespace MakePolicyFromApp.Services;

public interface IPowershell
{
    public Task ExecuteCommandAsync(
        string command,
        IDictionary<string, object> arguments,
        IEnumerable<string> modules
    );

    public static readonly object Void = new object();
}
