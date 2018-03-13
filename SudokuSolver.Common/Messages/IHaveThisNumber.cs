namespace SudokuSolver.Common.Messages
{
    public class IHaveThisNumber
    {
        public int Number { get; }

        public IHaveThisNumber(int number)
        {
            Number = number;
        }
    }
}