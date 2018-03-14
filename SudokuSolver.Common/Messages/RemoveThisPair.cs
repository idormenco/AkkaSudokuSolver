using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SudokuSolver.Common.Messages
{
    public class RemoveThisNumbersMessage
    {
        public IReadOnlyList<int> Numbers { get; }

        public RemoveThisNumbersMessage(params int[] numbers)
        {
            Numbers = new ReadOnlyCollection<int>(numbers);
        }
    }
}