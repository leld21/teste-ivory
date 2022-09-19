using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace Ivory.TesteEstagio.CampoMinado
{
    class Program
    {
        // Lista com as posições das bombas encontradas.
        static private List<(int, int)> Bombas = new List<(int, int)>();

        // AbreSeguras recebe como parâmetro o objeto do CampoMinado e uma lista de índices(posicoes)
        // que após as bombas serem removidas, chama a função Abrir() para cada uma das posições.
        public static void AbreSeguras(CampoMinado campo, List<(int,int)> indices)
        {
            List<(int, int)> semBomba = indices.Where(x => !Bombas.Any(y => x == y)).ToList();
            for(var i = 0; i < semBomba.Count; i++)
            {
                campo.Abrir(semBomba[i].Item1+1, semBomba[i].Item2+1);
            }
        }

        // AchaIndicesAdjacentes retorna apenas os indices Adjacentes a um elemento da matriz.
        public static List<(int, int)> AchaIndicesAdjacentes(int posx, int posy)
        {
            int linhas = 9;
            int colunas = 9;
            List<(int, int)> resultado = new List<(int, int)>();

            for (var j = posx - 1; j <= posx + 1; j++)
            {

                for (var i = posy - 1; i <= posy + 1; i++)
                {
                    if (i >= 0 && j >= 0 && i < colunas && j < linhas && !(j == posx && i == posy))
                    {
                        resultado.Add((j, i));
                    }
                }
            }
            return resultado;
        }

        // AchaAdjacentes retorna uma tripla com os caracteres e posições adjacentes
        // a um elemento da matriz.
        public static List<(char, int, int)> AchaAdjacentes(string[] arr, int posx, int posy)
        {
            int linhas = 9;
            int colunas = 9;
            List<(char, int, int)> resultado = new List<(char, int, int)>();

            for (var j = posx - 1; j <= posx + 1; j++)
            {

                for (var i = posy - 1; i <= posy + 1; i++)
                {
                    if (i >= 0 && j >= 0 && i < colunas && j < linhas && !(j == posx && i == posy))
                    {
                        resultado.Add((arr[j][i], j, i));
                    }
                }
            }
            return resultado;
        }

        // ComparaBombas é uma função auxiliar que retorna true se a quantidade de bombas 
        // adjacentes a um elemento da matriz é igual a seu número.
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

        // AnalisaBombas é a função principal do algoritmo, recebe a posição que será checada e dependendo de seu conteúdo
        // realiza ações diferentes, se for 0, apenas retorna, se for 1-3, primeiro checa se a quantidade de '-' adjacentes a si
        // é igual a seu número e se for, adiciona a posição dos '-' à lista de bomba, e caso não seja, checa se já existe na
        // lista de bombas quantidade suficiente de bombas adjacentes a ele com ComparaBombas() e caso haja chama AbreSeguros()
        // para abrir as posições seguras. E no caso de ser um '-', o algoritmo analisa seus adjacentes de forma recursiva, ignorando
        // caso seu adjacentes seja outro '-', realizando uma forma de backtracking.
        static void AnalisaBombas(CampoMinado campo, int x, int y)
        {
            string[] matriz = campo.Tabuleiro.Split("\r\n");
            var adjacentes = AchaAdjacentes(matriz, x, y);
            var indicesadj = AchaIndicesAdjacentes(x, y);
            List<int> listaIndice = new List<int>();

            if (matriz[x][y] == '-')
            {
                if (!Bombas.Contains((x, y)))
                {
                    for (var i = 0; i < indicesadj.Count; i++)
                    {
                        if (!(matriz[indicesadj[i].Item1][indicesadj[i].Item2] == '-'))
                        {
                            AnalisaBombas(campo, indicesadj[i].Item1, indicesadj[i].Item2);
                        }
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
            Console.WriteLine("Status {0} \n", campoMinado.JogoStatus);
            Console.WriteLine(campoMinado.Tabuleiro);
            Console.WriteLine();
            for (var i = 0; i < 9; i++)
            {
                for (var j = 0; j < 9; j++)
                {
                    AnalisaBombas(campoMinado, i, j);
                    if (campoMinado.JogoStatus == 1)
                    {
                        break;
                    }
                }
            }
            Console.WriteLine("Resultado\n=========");
            Console.WriteLine("Status {0} \n", campoMinado.JogoStatus);
            Console.WriteLine(campoMinado.Tabuleiro);
            Console.WriteLine("\nBOMBAS:\n");
            foreach(var bomba in Bombas)
            {
                Console.WriteLine("({0},{1})", bomba.Item1 + 1, bomba.Item2 + 1);
            }
        }
    }
}
