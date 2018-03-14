using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SudokuSolver.Common.Messages
{
    public class NakedTweenFindingMessage
    {
        public CellNeighbourhood CellNeighbourhood { get; }
        public IReadOnlyList<int> PairValues { get; }

        public NakedTweenFindingMessage(CellNeighbourhood cellNeighbourhood, List<int> pairValues)
        {
            CellNeighbourhood = cellNeighbourhood;
            PairValues = new ReadOnlyCollection<int>(pairValues);
        }
    }
}