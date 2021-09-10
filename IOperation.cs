using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakePolicyFromApp
{
    interface IOperation<T>
    {
        public Task StartAsync(T arguments);
    }
}
