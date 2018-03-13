using System.Collections.Generic;

namespace SudokuSolver.Common.Messages
{
    public class PrintCluesResponseMessage
    {
        public int Row { get; }
        public int Column { get; }
        public List<int> PossibleInts { get; }

        public PrintCluesResponseMessage(int row, int column, List<int> possibleInts)
        {
            Row = row;
            Column = column;
            PossibleInts = possibleInts;
        }
    }
}