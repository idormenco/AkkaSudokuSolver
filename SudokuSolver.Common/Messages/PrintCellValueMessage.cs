namespace SudokuSolver.Common.Messages
{
    public class PrintCellValueMessage
    {
        public int? CellValue { get; }
        public int RowIndex { get; }
        public int ColumnIndex { get; }

        public PrintCellValueMessage(int rowIndex, int columnIndex,int? cellValue)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            CellValue = cellValue;
        }
    }
}
