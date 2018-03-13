namespace SudokuSolver.Common.Messages
{
    public class HandshakeDoneMessage
    {
        public int? Value { get; set; }

        public HandshakeDoneMessage(int? value)
        {
            Value = value;
        }
    }
}