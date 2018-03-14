namespace SudokuSolver.Common.Messages
{
    public class HandshakeDoneMessage
    {
        public int? Value { get; }

        public HandshakeDoneMessage(int? value)
        {
            Value = value;
        }
    }
}