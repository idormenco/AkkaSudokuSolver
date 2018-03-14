using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SudokuSolver.Common.Messages
{
    public class PrintCluesResponseMessage
    {
        public int Row { get; }
        public int Column { get; }
        public IReadOnlyList<int> PossibleInts { get; }

        public PrintCluesResponseMessage(int row, int column, List<int> possibleInts)
        {
            Row = row;
            Column = column;
            PossibleInts = new ReadOnlyCollection<int>(possibleInts);
        }
    }
}