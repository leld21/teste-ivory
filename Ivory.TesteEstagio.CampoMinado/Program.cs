using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace Ivory.TesteEstagio.CampoMinado
{
    class Program
    {

        static private List<(int,int)> Bombas = new List<(int, int)>();
        public static void AbreSeguras (CampoMinado campo, List<(int,int)> indices)
        {
            List<(int, int)> semBomba = indices.Where(x => !Bombas.Any(y => x == y)).ToList();
            for(int i=0;i<semBomba.Count; i++)
            {
                campo.Abrir(semBomba[i].Item1+1, semBomba[i].Item2+1);
            }
        }

        public static List<(int, int)> AchaIndicesAdjacentes(string[] arr, int linha, int coluna)
        {
            int linhas = 9,colunas = 9;
            List<(int, int)> resultado = new List<(int, int)>();

            for (int j = linha - 1; j <= linha + 1; j++)
            {

                for (int i = coluna - 1; i <= coluna + 1; i++)
                {
                    if (i >= 0 && j >= 0 && i < colunas && j < linhas && !(j == linha && i == coluna))
                    {
                        resultado.Add((j, i));
                    }
                }
            }
            return resultado;
        }
        public static List<(char, int, int)> AchaAdjacentes(string[] arr, int linha, int coluna)
        {
            int linhas = 9, colunas = 9;
            List<(char,int,int)> resultado = new List<(char, int, int)>();

            for (int j = linha - 1; j <= linha + 1; j++)
            {

                for (int i = coluna - 1; i <= coluna + 1; i++)
                {
                    if (i >= 0 && j >= 0 && i < colunas && j < linhas && !(j == linha && i == coluna))
                    {
                        resultado.Add((arr[j][i], j, i));
                    }
                }
            }
            return resultado;
        }

        static bool ComparaBombas(int x, List<(int, int)> lista)
        {
            int quantbomba = 0;
            for(int i = 0; i < lista.Count; i++)
            {
                if (Bombas.Contains((lista[i].Item1, lista[i].Item2)))
                {
                    quantbomba++;
                }
            }
            if (x == quantbomba)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static void AnalisaBombas(CampoMinado campo, int x, int y, bool secundario)
        {
            string[] matriz = campo.Tabuleiro.Split("\r\n");
            var adjacentes = AchaAdjacentes(matriz, x, y);
            var indicesadj = AchaIndicesAdjacentes(matriz, x, y);
            List<int> listaIndice = new List<int>();

            if (matriz[x][y] == '-')
            {
                if (secundario == true)
                {
                    return;
                }
                if (!Bombas.Contains((x, y)))
                {
                    for (int i = 0; i < indicesadj.Count; i++)
                    {
                        AnalisaBombas(campo, indicesadj[i].Item1, indicesadj[i].Item2, true);
                    }
                }
            }
            else if (matriz[x][y] == '0')
            {
                return;
            }else
            {
                if (adjacentes.FindAll(x => x.Item1 == '-').Count == (matriz[x][y] - '0'))
                {
                    foreach (var tuple in adjacentes)
                    {
                        if (tuple.Item1 == '-')
                        {
                            if (!Bombas.Contains((tuple.Item2, tuple.Item3))) {
                                Bombas.Add((tuple.Item2, tuple.Item3));
                            }
                        }
                    }
                }
                else if (ComparaBombas((matriz[x][y]-'0'), indicesadj))
                {
                    AbreSeguras(campo, indicesadj);
                }
            }
        }


        static void Main(string[] args)
        {
            var campoMinado = new CampoMinado();
            Console.WriteLine("Início do jogo\n=========");
            Console.WriteLine(campoMinado.Tabuleiro);
            Console.WriteLine();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    AnalisaBombas(campoMinado, i, j, false);
                    if (campoMinado.JogoStatus == 1)
                    {
                        break;
                    }
                }
            }
            Console.WriteLine("Resultado\n=========");
            Console.WriteLine(campoMinado.JogoStatus);
            Console.WriteLine(campoMinado.Tabuleiro);
        }
    }
}
