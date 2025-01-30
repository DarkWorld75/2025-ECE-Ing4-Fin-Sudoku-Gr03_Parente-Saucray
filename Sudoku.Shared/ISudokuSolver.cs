using Sudoku.NorvigSolvers;  // OU un autre namespace où ISudokuSolver est défini

namespace Sudoku.Shared
{
    public interface ISudokuSolver
    {
        SudokuGrid Solve(SudokuGrid s);
    }

}