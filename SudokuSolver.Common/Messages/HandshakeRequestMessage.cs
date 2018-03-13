namespace SudokuSolver.Common.Messages
{
    public class HandshakeRequestMessage
    {
        public CellNeighbourhood Location { get; }
        public HandshakeRequestMessage(CellNeighbourhood location)
        {
            Location = location;
        }
    }
}
