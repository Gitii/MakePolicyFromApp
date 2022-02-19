using System.Threading.Tasks;

namespace MakePolicyFromApp;

interface IOperation<T>
{
    public Task StartAsync(T arguments);
}
