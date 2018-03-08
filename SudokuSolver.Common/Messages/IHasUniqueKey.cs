using System;

namespace SudokuSolver.Common.Messages
{
    public class HasUniqueKey
    {
        public Guid Id =>Guid.NewGuid();
    }
}