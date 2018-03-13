using System.Collections.Generic;

namespace SudokuSolver.Common.Messages
{
    public class NakedTweenFindingMessage
    {
        public CellNeighbourhood CellNeighbourhood { get; }
        public List<int> PairValues { get; }

        public NakedTweenFindingMessage(CellNeighbourhood cellNeighbourhood, List<int> pairValues)
        {
            CellNeighbourhood = cellNeighbourhood;
            PairValues = pairValues;
        }
    }
}