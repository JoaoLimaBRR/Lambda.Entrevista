using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambda.Entrevista.Domain.Constants
{
    public class EmailMensagens
    {
        public static string CorpoEmail(string nome, string link) => $@"
            Olá {nome},
            
            Você foi convidado a responder perguntas sobre um processo jurídico.
            
            Por favor acesse: {link}
            
            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur non risus euismod, varius ligula vitae, convallis justo.
            
            Atenciosamente,
            Equipe de Entrevistas";
    }
}
