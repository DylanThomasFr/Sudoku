using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSudoku
{
    class Program
    {
        public int[,] grille = new int[9, 9];
        public bool?[, ,] valeurs_cases = new bool?[9, 9, 9];

        private static readonly Random getrandom = new Random();
        private static readonly object syncLock = new object();
        private bool nestpasdansligne(int val, int line, int col)
        {
            bool tmp = true;
            for (int i = 0; i < col; i++)
            {
                if (val == grille[line, i])
                {
                    tmp = false;
                }
            }
            return tmp;
            //retourne faux si le nombre est dans la ligne et vrai sinon
        } // on vérifie que la valeur tirée n’est pas dans la ligne  

        private bool nestpasdanscolonne(int val, int line, int col)
        {
            bool tmp = true;
            for (int i = 0; i < line; i++)
            {
                if (val == grille[i, col])
                {
                    tmp = false;
                }
            }
            return tmp;
            //retourne faux si le nombre est dans la colonne et vrai sinon
        } // on vérifie que la valeur tirée n’est pas dans la colonne 

        private bool nestpasdanscarre(int val, int line, int col)
        {
            //pour une certaine valeur on doit délimiter le carré qui le contient
            bool tmp = true;
            int i_tmp = (line) - (line % 3);
            int j_tmp = (col) - (col % 3);
            for (int i = i_tmp; i <= i_tmp + 2; i++)
                for (int j = j_tmp; j <= j_tmp + 2; j++)
                    if (val == grille[i, j])
                    {
                       // Console.Write("grille " + grille[i_tmp, j_tmp] + i_tmp + j_tmp + "\n");
                        tmp = false;
                    }
            return tmp;
            //retourne faux si le nombre est dans la carre et vrai sinon
        } // on vérifie que la valeur tirée n’est pas dans le carré

        private bool nestpasdanscarre2(int value, int indR, int indC)
        {
            int divC, divR;
            bool p = true;
            divC = indC / 3;
            divR = indR / 3;
            for (int i = divC * 3; i < divC * 3 + 3; i++)
            {
                for (int j = divR * 3; j < divR * 3 + 3; j++)
                {
                    if (grille[i, j]== value) p = false;
                }
            }
            return p;
        }
        // Fonction d'affichage
        public void affichage()
        {
            for (int i = 0; i < 9; i++)
            {
                Console.Write("|");
                for (int j = 0; j < 9; j++)
                {
                    if (((j + 1) % 3) != 0)
                        Console.Write(grille[i, j]);
                    else Console.Write(grille[i, j] + "|");
                }
                Console.Write('\n');
                if (((i + 1) % 3) == 0)
                    Console.WriteLine("-------------");
            }
            Console.WriteLine("\n\n");
        }

        public void init_grille()
        {
            //Initialisation des valeurs possibles
            for (int i = 0; i < 9; i++)
                for (int j = 0; i < 9; i++)
                {
                    grille[i, j] = 0;
                }
            initValPossibleCase(valeurs_cases);
        }

        private static int GetRandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return getrandom.Next(min, max);
            }
        }

        public bool genere_grille(int id_emplacement)
       {
            //récupération le numero de ligne et de colonne en fonction de la position :
            int line = id_emplacement / 9;
            int col = id_emplacement % 9;
            
            bool verif_possibilite_nbchoix = false;
            bool verif_possibilite_valeur = false;
  
            if (id_emplacement >= 81) return true;         

            bool?[] possibilite_case = new bool?[9];
            initCase(possibilite_case);

            int nb_possibilite = 0;
  
            for (int i = 0; i < 9; i++)
            {
                possibilite_case[i] = valeurs_cases[line, col, i];
                if (valeurs_cases[line, col, i] == null)
                {
                    verif_possibilite_valeur = true;
                }
                else if(valeurs_cases[line, col, i]==true)
                {
                    verif_possibilite_valeur = true;
                    nb_possibilite++;                
                }
            }

            if (verif_possibilite_valeur)
            {
                if (nb_possibilite == 0)
                {
                    for (int i = 1; i <= 9; i++)
                    {
                        if (nestpasdansligne(i, line, col) && nestpasdanscolonne(i, line, col) && nestpasdanscarre(i, line, col))
                        {
                            valeurs_cases[line, col, i - 1] = true;
                            possibilite_case[i - 1] = true;
                            nb_possibilite++;
                        }
                        else valeurs_cases[line, col, i - 1] = false;
                    }
                }
                if (nb_possibilite > 0)
                {
                    while (!verif_possibilite_nbchoix)
                    {
                        Random rnd = new Random();
                        int randomNumber = GetRandomNumber(1, 10);
                        if (possibilite_case[randomNumber - 1] == true)
                        {
                            valeurs_cases[line, col, randomNumber - 1] = false;
                            grille[line, col] = randomNumber;
                            verif_possibilite_nbchoix = true;
                        }
                    }
                    genere_grille(id_emplacement + 1);
                }
                else
                {
                    grille[line, col] = 0;
                    for (int k = 0; k < 9; k++)
                    {
                        valeurs_cases[line, col, k] = null;
                    }
                    genere_grille(id_emplacement - 1);
                }
                return true;
            }
            else
            {
                grille[line, col] = 0;
                for (int k = 0; k < 9; k++)
                {
                    valeurs_cases[line, col, k] = null;
                }
                genere_grille(id_emplacement - 1);
                return false;
            }
        }

        private static void initCase(bool?[] tabValeurs)
        {

            for (int i = 0; i < 9; i++)
            {
                tabValeurs[i] = false;
            }
        }
        private static void initValPossibleCase(bool?[, ,] tabValeurs)
        {
            tabValeurs = new bool?[9, 9, 9];
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    for (int k = 0; k < 9; k++)
                    {
                        tabValeurs[i, j, k] = null;
                    }
        }

        public void grille_sudoku(int[,] tab){

        }

        static void Main(string[] args)
        {
            Program test = new Program();
            test.init_grille();
            bool b = test.genere_grille(0);

            if (b == true) test.affichage();
            Console.ReadLine();
        }
    }
}
