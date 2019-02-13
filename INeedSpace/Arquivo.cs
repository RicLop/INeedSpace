using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INeedSpace
{
    public class Arquivo
    {
        public long ID { get; set; }

        public string Nome { get; set; }

        public string Extensao { get; set; }

        public string Caminho { get; set; }

        public long Tamanho { get; set; }
    }
}
