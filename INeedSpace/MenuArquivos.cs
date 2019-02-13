using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INeedSpace
{
    public class MenuArquivo
    {
        public static void Menu(Arquivo arquivo)
        {
            string opcao = string.Empty;
            do
            {
                Console.Clear();
                Console.WriteLine("O que você deseja fazer com {0}?", arquivo.Nome);
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("Nome: " + arquivo.Nome);
                Console.WriteLine("Tamanho: " + MenuPrincipal.ConverteByte(arquivo.Tamanho));
                Console.WriteLine("Caminho: " + arquivo.Caminho);
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("[1] Executar");
                Console.WriteLine("[2] Apagar");
                Console.WriteLine("[3] Renomear");
                Console.WriteLine("[4] Alterar Extensão");
                Console.WriteLine("[5] Voltar para seleção de diretórios");
                if (arquivo.Extensao == "")
                {
                    Console.WriteLine();
                }

                opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        ExecutarArquivo(arquivo);
                        break;

                    //case "2":
                    //    if (ApagaPasta(arquivo))
                    //    {
                    //        return;
                    //    }
                    //    break;
                    //
                    //case "3":
                    //    MostrarArquivos(arquivo);
                    //    break;
                    //
                    //case "4":
                    //    Process.Start(arquivo.Caminho);
                    //    break;
                    //
                    //case "5":
                    //    RenomeiaPasta(arquivo);
                    //    break;
                    //
                    //case "6":
                    //    return;

                    default:
                        Console.WriteLine("Opção desconhecida. Aperte qualquer tecla para voltar.");
                        break;
                }
            } while (true);
        }

        public static void ExecutarArquivo(Arquivo arquivo)
        {
            Process.Start(@arquivo.Caminho);
        }
    }
}
