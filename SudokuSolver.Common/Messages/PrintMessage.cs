namespace SudokuSolver.Common.Messages
{
    public class PrintMessage
    {
        public string Message { get; }

        public PrintMessage(string message)
        {
            Message = message;
        }
    }
}