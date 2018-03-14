namespace SudokuSolver.Common.Messages
{
    public class IHaveThisNumber
    {
        public int Number { get; }
        public int Row { get; }
        public int Column { get; }


        public IHaveThisNumber(int number, int row, int column)
        {
            Number = number;
            Row = row;
            Column = column;
        }


    }
}