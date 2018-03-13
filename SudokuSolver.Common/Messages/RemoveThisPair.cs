using System.Collections.Generic;

namespace SudokuSolver.Common.Messages
{
    public class RemoveThisNumbersMessage
    {
        public List<int> Numbers { get; }

        public RemoveThisNumbersMessage(params int[] numbers)
        {
            Numbers = new List<int>();
            Numbers.AddRange(numbers);
        }
    }
}