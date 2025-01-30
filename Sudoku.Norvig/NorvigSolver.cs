using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sudoku.Shared;


namespace Sudoku.Solvers
{
    public class NorvigSolver : ISudokuSolver
    {
        private const string Digits = "123456789";
        private const int GridSize = 9;

        private Dictionary<string, string> _values;
        private Dictionary<string, List<string>> _peers;
        private Dictionary<string, List<List<string>>> _units;

        public NorvigSolver()
        {
            (_peers, _units) = InitializePeersAndUnits();
        }

        public bool Solve(int[,] board)
        {
            _values = InitializeValues(board);
            return Search(_values) != null;
        }

        private Dictionary<string, string> InitializeValues(int[,] board)
        {
            var values = new Dictionary<string, string>();
            var squares = GenerateSquares();

            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    string square = squares[i * GridSize + j];
                    values[square] = board[i, j] == 0 ? Digits : board[i, j].ToString();
                }
            }
            return values;
        }

        private Dictionary<string, List<string>> InitializePeersAndUnits()
        {
            var squares = GenerateSquares();
            var unitList = GenerateUnitList(squares);
            var units = new Dictionary<string, List<List<string>>>();
            var peers = new Dictionary<string, List<string>>();

            foreach (var square in squares)
            {
                units[square] = unitList.Where(unit => unit.Contains(square)).ToList();
                peers[square] = units[square].SelectMany(unit => unit).Distinct().Where(s => s != square).ToList();
            }

            return (peers, units);
        }

        private List<string> GenerateSquares()
        {
            var rows = "ABCDEFGHI".ToCharArray();
            var cols = Digits.ToCharArray();
            var squares = new List<string>();

            foreach (var r in rows)
            {
                foreach (var c in cols)
                {
                    squares.Add($"{r}{c}");
                }
            }
            return squares;
        }

        private List<List<string>> GenerateUnitList(List<string> squares)
        {
            var rows = "ABCDEFGHI".ToCharArray();
            var cols = Digits.ToCharArray();
            var unitList = new List<List<string>>();

            // Rows
            foreach (var r in rows)
            {
                unitList.Add(cols.Select(c => $"{r}{c}").ToList());
            }

            // Columns
            foreach (var c in cols)
            {
                unitList.Add(rows.Select(r => $"{r}{c}").ToList());
            }

            // 3x3 Boxes
            var rowGroups = new[] { "ABC", "DEF", "GHI" };
            var colGroups = new[] { "123", "456", "789" };

            foreach (var rg in rowGroups)
            {
                foreach (var cg in colGroups)
                {
                    unitList.Add(rg.SelectMany(r => cg.Select(c => $"{r}{c}")).ToList());
                }
            }
            return unitList;
        }

        private Dictionary<string, string> Search(Dictionary<string, string> values)
        {
            if (values.Values.All(v => v.Length == 1))
                return values;

            var square = values.Where(v => v.Value.Length > 1)
                               .OrderBy(v => v.Value.Length)
                               .First().Key;

            foreach (var digit in values[square])
            {
                var newValues = new Dictionary<string, string>(values);
                if (Assign(newValues, square, digit.ToString()))
                {
                    var result = Search(newValues);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }

        private bool Assign(Dictionary<string, string> values, string square, string digit)
        {
            var otherValues = values[square].Replace(digit, "");
            foreach (var d in otherValues)
            {
                if (!Eliminate(values, square, d.ToString()))
                    return false;
            }
            return true;
        }

        private bool Eliminate(Dictionary<string, string> values, string square, string digit)
        {
            if (!values[square].Contains(digit))
                return true;

            values[square] = values[square].Replace(digit, "");

            if (values[square].Length == 0)
                return false;

            if (values[square].Length == 1)
            {
                var d2 = values[square];
                foreach (var peer in _peers[square])
                {
                    if (!Eliminate(values, peer, d2))
                        return false;
                }
            }

            foreach (var unit in _units[square])
            {
                var places = unit.Where(s => values[s].Contains(digit)).ToList();
                if (places.Count == 0)
                    return false;
                if (places.Count == 1 && !Assign(values, places[0], digit))
                    return false;
            }

            return true;
        }
    }
}
