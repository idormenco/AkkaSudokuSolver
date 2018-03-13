using Akka.Actor;
using SudokuSolver.Common.Messages;
using System;
using System.Diagnostics;
using System.Threading;

namespace SudokuSolver.Printer
{
    public class PrinterActor : TypedActor,
        IHandle<FindSudokuGameMessage>,
        IHandle<PrintCellValueMessage>,
        IHandle<PrintMessage>
    {
        private readonly ActorSelection _server = Context.ActorSelection("akka.tcp://SudokuSolverActorSystem@localhost:8081/user/Sudoku");

        private int?[,] sudokuBoard = new int?[9, 9];

        public PrinterActor()
        {
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new PrinterActor());
        }

        public void Handle(PrintCellValueMessage message)
        {
            sudokuBoard[message.RowIndex - 1, message.ColumnIndex - 1] = message.CellValue;
            Console.Clear();
            PrintBoard();
            Thread.Sleep(200);
        }

        private void PrintBoard()
        {
            for (int i = 0; i < sudokuBoard.GetLength(0); i++)
            {
                if (i == 0) Console.WriteLine("--------+-------+--------");
                string line = "";
                for (int j = 0; j < sudokuBoard.GetLength(1); j++)
                {
                    int? cell = sudokuBoard[i, j];
                    line += (j == 0 ? "| " : "") + (cell.HasValue ? cell.ToString() : "_") + " " + ((j + 1) % 3 == 0 ? "| " : "");
                }
                Console.WriteLine(line);
                if ((i + 1) % 3 == 0) Console.WriteLine("--------+-------+--------");

            }
            Console.WriteLine();
        }

        public void Handle(FindSudokuGameMessage message)
        {
            _server.Tell(new IAmPrinterMessage());
        }

        public void Handle(PrintMessage message)
        {
            Console.WriteLine(message.Message);
        }
    }
}
