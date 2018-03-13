namespace SudokuSolver.Common.Messages
{
    public class HandshakeResponseMessage
    {
        public CellNeighbourhood Location { get; }

        public HandshakeResponseMessage(CellNeighbourhood location)
        {
            Location = location;
        }
    }
}
