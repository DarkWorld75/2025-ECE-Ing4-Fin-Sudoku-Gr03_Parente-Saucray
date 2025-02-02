using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku.Norvig
{
    public class NorvigSolver
    {
        private const int GridSize = 9;
        private const int BoxSize = 3;
        private readonly int[,] _grid;

        public NorvigSolver()
        {
            _grid = new int[GridSize, GridSize];
        }

        // Initialiser la grille (vide)
        public void InitializeGrid()
        {
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    _grid[i, j] = 0; // 0 représente une case vide
                }
            }
        }

        // Résoudre le puzzle
        public bool Solve()
        {
            return Solve(0, 0);
        }

        private bool Solve(int row, int col)
        {
            // Si on a atteint la fin de la grille, c'est résolu
            if (row == GridSize)
                return true;

            if (col == GridSize)
                return Solve(row + 1, 0);

            if (_grid[row, col] != 0)  // Si la cellule est déjà remplie, on passe à la suivante
                return Solve(row, col + 1);

            // Essayer tous les chiffres possibles
            for (int num = 1; num <= GridSize; num++)
            {
                if (IsValid(row, col, num))
                {
                    _grid[row, col] = num;

                    if (Solve(row, col + 1)) // Continuer avec la prochaine cellule
                        return true;

                    _grid[row, col] = 0;  // Revenir en arrière (backtracking)
                }
            }

            return false;  // Aucun chiffre valide trouvé
        }

        // Vérifier si le numéro est valide pour une case donnée
        private bool IsValid(int row, int col, int num)
        {
            // Vérifier la ligne
            for (int i = 0; i < GridSize; i++)
            {
                if (_grid[row, i] == num)
                    return false;
            }

            // Vérifier la colonne
            for (int i = 0; i < GridSize; i++)
            {
                if (_grid[i, col] == num)
                    return false;
            }

            // Vérifier le sous-grille 3x3
            int boxRowStart = (row / BoxSize) * BoxSize;
            int boxColStart = (col / BoxSize) * BoxSize;

            for (int i = 0; i < BoxSize; i++)
            {
                for (int j = 0; j < BoxSize; j++)
                {
                    if (_grid[boxRowStart + i, boxColStart + j] == num)
                        return false;
                }
            }

            return true;
        }

        // Obtenir la grille résolue
        public int[,] GetSolvedBoard()
        {
            return _grid;
        }

        // Retirer des chiffres en fonction de la difficulté
        public static void RemoveDigits(int[,] board, int difficulty)
        {
            Random rand = new Random();
            int emptyCells = difficulty switch
            {
                1 => 36,  // Facile : 36 cellules vides
                2 => 45,  // Moyen : 45 cellules vides
                3 => 54,  // Difficile : 54 cellules vides
                _ => 45   // Par défaut : Moyen
            };

            int count = 0;
            while (count < emptyCells)
            {
                int row = rand.Next(0, GridSize);
                int col = rand.Next(0, GridSize);

                if (board[row, col] != 0)
                {
                    board[row, col] = 0;
                    count++;
                }
            }
        }

        // Affichage de la grille
        public void DisplayBoard()
        {
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    Console.Write(_grid[i, j] == 0 ? ". " : _grid[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
    }
}




