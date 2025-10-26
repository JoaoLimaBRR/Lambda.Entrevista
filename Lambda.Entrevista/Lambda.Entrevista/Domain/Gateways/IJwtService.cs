using Lambda.Entrevista.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambda.Entrevista.Domain.Gateways
{
    public interface IJwtService
    {
        Task<string> GerarTokenBase64Async(EntrevistaEntity entidade);
    }
}
