namespace SudokuSolver.Common.Messages
{
    public class HandshakeRequestMessage
    {
        public string Location { get; }
        public HandshakeRequestMessage(string location)
        {
            Location = location;
        }
    }
}
