using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace INeedSpace
{
    public class MenuPrincipal
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = "INeedSpace";

            do
            {
                Console.Clear();
                infoHDD();
                Console.WriteLine("Atenção, na versão atual deste projeto, não é recomendado ler o disco rigído onde o Windows está instalado!");
                Console.WriteLine("");
                Console.Write("Informe o caminho: ");
                string caminho = Console.ReadLine();
                Console.WriteLine("");
                Console.Write("Lendo os arquivos... ");

                Menu(null, caminho);

            } while (true);
        }

        public static void infoHDD()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                Console.WriteLine("Drive {0} : {1}", d.Name, TIpoHDD(d));
                if (d.IsReady == true)
                {
                    Console.WriteLine("--Espaço disp. para o usuário atual: {0}", ConverteByte(d.AvailableFreeSpace));
                    Console.WriteLine("--Espaço disponivel total: {0}", ConverteByte(d.TotalFreeSpace));
                    Console.WriteLine("--Tamanho do disco: {0}", ConverteByte(d.TotalSize));
                }
                Console.WriteLine("");
            }
        }

        public static string TIpoHDD(DriveInfo drive)
        {
            string tipo = drive.DriveType.ToString();
            if (tipo == "Fixed")
            {
                return "Fixo";
            }
            else if (tipo == "Removable")
            {
                return "Removivel";
            }
            else if (tipo == "Network")
            {
                return "Network";
            }
            else if (tipo == "CDRom")
            {
                return "Disco ótico";
            }
            else if (tipo == "Ram")
            {
                return "RAM";
            }
            else
            {
                return "Desconhecido";
            }
        }

        public static void Menu(List<Diretorio> listaSubpastasOrder, string caminho)
        {
            var subpasta = new Diretorio();
            if ((caminho == "") || (caminho == string.Empty))
            {
                Console.WriteLine("");
                Console.WriteLine("Escolha um ID para realizar uma ação, ou aperte ENTER para voltar.");
                string Prosseguir = Console.ReadLine();

                if (!ExisteID(listaSubpastasOrder, Prosseguir))
                {
                    return;
                }

                subpasta = listaSubpastasOrder.Find(x => x.ID == Convert.ToInt64(Prosseguir));
            }
            else
            {
                subpasta = AcessaPastaPrimeiraVez(caminho);
                if (subpasta == null)
                {
                    return;
                }
            }

            string opcao = string.Empty;
            do
            {
                Console.Clear();
                Console.WriteLine("O que você deseja fazer com {0}?", subpasta.Nome);
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("Nome: " + subpasta.Nome);
                Console.WriteLine("Tamanho: " + ConverteByte(subpasta.Tamanho));
                Console.WriteLine("Caminho: " + subpasta.Caminho);
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("[1] Acessar");  
                Console.WriteLine("[2] Apagar");
                Console.WriteLine("[3] Mostrar arquivos do diretório");
                Console.WriteLine("[4] Abrir diretório no explorer");
                Console.WriteLine("[5] Renomear diretório");
                Console.WriteLine("[6] Voltar para seleção de diretórios");
                opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        AcessaPasta(subpasta.Caminho);
                        break;

                    case "2":
                        if (ApagaPasta(subpasta))
                        {
                            return;
                        }
                        break;

                    case "3":
                        MostrarArquivos(subpasta);
                        break;

                    case "4":
                        Process.Start(subpasta.Caminho);
                        break;

                    case "5":
                        RenomeiaPasta(subpasta);
                        break;

                    case "6":
                        return;

                    default:
                        Console.WriteLine("Opção desconhecida. Aperte qualquer tecla para voltar.");
                        break;
                }
            } while (true);
        }

        public static void AcessaPasta(string caminho)
        {
            try
            {
                List<Diretorio> listaSubpastas = new List<Diretorio>();
                DirectoryInfo infoDir = new DirectoryInfo(@caminho);
                DirectoryInfo[] arrayDirs = infoDir.GetDirectories();

                long tamanhoTotal = LerDiretorios(arrayDirs, listaSubpastas);

                Console.Clear();
                Console.WriteLine("Diretório {0} contém os seguintes subdiretórios:", infoDir.Name);
                Console.WriteLine("");

                Console.WriteLine("[ID] Nome do subdiretório = Tamanho");
                Console.WriteLine("");
                List<Diretorio> listaSubpastasOrder = listaSubpastas.OrderByDescending(o => o.Tamanho).ToList();
                foreach (Diretorio subpastas in listaSubpastasOrder)
                {
                    Console.WriteLine("[{0}] {1} = {2}", subpastas.ID, subpastas.Nome, ConverteByte(subpastas.Tamanho));
                }

                Console.WriteLine("");
                Console.WriteLine("Tamanho total do diretório é de " + ConverteByte(tamanhoTotal));

                Menu(listaSubpastasOrder, "");
            }
            catch (ArgumentException)
            {
                Console.Clear();
                Console.WriteLine("Diretório não encontrado");
                Console.WriteLine("");
                Console.WriteLine("Aperte qualquer tecla para selecionar outro diretório.");
                Console.ReadKey();
            }
            catch (UnauthorizedAccessException)
            {
                Console.Clear();
                Console.WriteLine("Você não tem permissão para acessar este diretório ");
                Console.WriteLine("");
                Console.WriteLine("Aperte qualquer tecla para selecionar outro diretório.");
                Console.ReadKey();
            }
            catch (DirectoryNotFoundException)
            {
                Console.Clear();
                Console.WriteLine("Diretório não encontrado");
                Console.WriteLine("");
                Console.WriteLine("Aperte qualquer tecla para selecionar outro diretório.");
                Console.ReadKey();
            }
        }

        public static bool ApagaPasta(Diretorio diretorio)
        {
            Console.Clear();
            Console.WriteLine("Você tem certeza que quer apagar {0}? (S/N)", diretorio.Nome);
            string opcao = Console.ReadLine().ToUpper();
            if (opcao == "S")
            {
                Directory.Delete(diretorio.Caminho, true);
                Console.Clear();
                Console.WriteLine("{0} apagado com sucesso.", diretorio.Nome);
                Console.WriteLine("");
                Console.WriteLine("Aperte qualquer tecla para voltar para seleção de diretórios");
                Console.ReadKey();

                return true;
            }
            else
            {
                return false;
            }
        }

        public static void RenomeiaPasta(Diretorio diretorio)
        {
            Console.Clear();
            Console.WriteLine("Informe o novo nome do diretório {0}", diretorio.Nome);
            string nome = Console.ReadLine();

            string caminhoSemPasta = diretorio.Caminho.Remove(diretorio.Caminho.IndexOf(diretorio.Nome), diretorio.Nome.Length);
            string novoCaminho = Path.Combine(caminhoSemPasta, nome);

            try
            {
                Directory.Move(diretorio.Caminho, novoCaminho);
                diretorio.Nome = nome;
                diretorio.Caminho = novoCaminho;
            }
            catch (Exception e)
            {
                Console.WriteLine("");
                Console.WriteLine("Erros acontecem, não fique pensando muito nesse: \n" + e.Message
                    + "\n Aperte qualquer tecla para voltar.");
                Console.ReadKey();
            }
        }

        public static void MostrarArquivos(Diretorio diretorio)
        {
            List<Arquivo> listaArquivos = new List<Arquivo>();
            DirectoryInfo infoDir = new DirectoryInfo(diretorio.Caminho);

            Console.Clear();
            Console.WriteLine("Diretório {0} contém os seguintes arquivos:", infoDir.Name);
            Console.WriteLine("");

            Console.WriteLine("[ID] Nome do arquivo = Tamanho");
            Console.WriteLine("");

            long id = 1;
            FileInfo[] files = infoDir.GetFiles();
            foreach (FileInfo file in files)
            {
                Arquivo arquivo = new Arquivo();

                arquivo.Caminho = file.FullName;
                arquivo.Nome = file.Name;
                arquivo.Tamanho = file.Length;
                arquivo.ID = id;

                Console.WriteLine("[{0}] {1} = {2}", arquivo.ID, arquivo.Nome, ConverteByte(arquivo.Tamanho));

                id++;
            }
            Console.WriteLine("");

            MenuArquivo.Menu(listaArquivos);
        }

        public static Diretorio AcessaPastaPrimeiraVez(string caminho)
        {
            try
            {
                List<Diretorio> listaSubpastas = new List<Diretorio>();
                DirectoryInfo infoDir = new DirectoryInfo(@caminho);
                DirectoryInfo[] arrayDirs = infoDir.GetDirectories();

                Diretorio subpasta = new Diretorio();
                subpasta.Nome = infoDir.Name;
                SalvarTamanhoETipo(subpasta, ObterTamanhoDaPasta(@caminho));
                subpasta.Caminho = infoDir.FullName;

                return subpasta;
            }
            catch (ArgumentException)
            {
                Console.Clear();
                Console.WriteLine("Diretório não encontrado");
                Console.WriteLine("");
                Console.WriteLine("Aperte qualquer tecla para selecionar outro diretório.");
                Console.ReadKey();

                return null;
            }
            catch (UnauthorizedAccessException)
            {
                Console.Clear();
                Console.WriteLine("Você não tem permissão para acessar este diretório ");
                Console.WriteLine("");
                Console.WriteLine("Aperte qualquer tecla para selecionar outro diretório.");
                Console.ReadKey();

                return null;
            }
            catch (DirectoryNotFoundException)
            {
                Console.Clear();
                Console.WriteLine("Diretório não encontrado");
                Console.WriteLine("");
                Console.WriteLine("Aperte qualquer tecla para selecionar outro diretório.");
                Console.ReadKey();

                return null;
            }
        }

        public static long LerDiretorios(DirectoryInfo[] arrayDirs, List<Diretorio> listaDiretorios)
        {
            long tamanhoTotal = 0;
            long id = 1;
            foreach (DirectoryInfo dir in arrayDirs)
            {
                Diretorio subpastas = new Diretorio();
                long tamanhoUnitario = 0;

                DirectoryInfo infoDir = new DirectoryInfo(dir.FullName);

                tamanhoUnitario = ObterTamanhoDaPasta(infoDir.FullName);
                tamanhoTotal += tamanhoUnitario;

                subpastas.Nome = infoDir.Name;
                subpastas.Caminho = infoDir.FullName;
                SalvarTamanhoETipo(subpastas, tamanhoUnitario);

                subpastas.ID = id;
                id++;

                listaDiretorios.Add(subpastas);
            }

            return tamanhoTotal;
        }

        public static void SalvarTamanhoETipo(Diretorio diretorio, long tamanhoUnitario)
        {
            string tamanhoETipo = ConverteByte(tamanhoUnitario);
            string tipo = tamanhoETipo.Substring(tamanhoETipo.Length - 2);

            diretorio.Tamanho = tamanhoUnitario;
            diretorio.Tipo = tipo;
        }

        public static long ObterTamanhoDaPasta(string diretorio)
        {
            if (!temAcesso(diretorio))
            {
                return 0;
            }
            long tamanhoAtual = 0;
            long subDir = 0;
            try
            {

                var files = Directory.EnumerateFiles(diretorio);

                tamanhoAtual = (from file in files let fileInfo = new FileInfo(file) select fileInfo.Length).Sum();

                var diretorios = Directory.EnumerateDirectories(diretorio);

                subDir = (from dir in diretorios select ObterTamanhoDaPasta(dir)).Sum();

                return tamanhoAtual + subDir;
            }
            catch (PathTooLongException)
            {
                return tamanhoAtual + subDir;
            }
        }

        static string ConverteByte(long bytes)
        {
            double tamanho = 0;
            if (bytes > 1000000000)
            {
                tamanho = ((bytes / 1024f) / 1024f) / 1024f;

                return Math.Round(tamanho, 2) + " GB";
            }
            else if (bytes > 1000000)
            {
                tamanho = (bytes / 1024f) / 1024f;

                return Math.Round(tamanho, 2) + " MB";
            }
            else
            {
                tamanho = bytes / 1024f;

                return Math.Round(tamanho, 2) + " KB";
            }
        }

        public static bool ExisteID(List<Diretorio> listaOdernadaDiretorios, string id)
        {
            try
            {
                Diretorio subpasta = listaOdernadaDiretorios.Find(x => x.ID == Convert.ToInt64(id));

                if (subpasta != null)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("ID desconhecido. Aperte qualquer tecla para voltar.");
                    Console.ReadKey();

                    return false;
                }
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static bool temAcesso(string diretorio)
        {
            try
            {
                var files = Directory.EnumerateFiles(diretorio);

                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
    }
}