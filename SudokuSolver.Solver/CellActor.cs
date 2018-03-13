using Akka.Actor;
using SudokuSolver.Common.Messages;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SudokuSolver.Solver
{
    public class CellActor : ReceiveActor
    {
        private readonly int rowIndex;
        private readonly int columnIndex;
        private readonly List<int> possibleNumbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        private readonly IActorRef printerActor;
        private int? cellNumber;
        private readonly List<IActorRef> rowCells = new List<IActorRef>();
        private readonly List<IActorRef> columnCells = new List<IActorRef>();
        private readonly List<IActorRef> squareCells = new List<IActorRef>();
        private bool isSolved = false;
        public CellActor(IActorRef printerActor, int rowIndex, int columnIndex) : this(printerActor, rowIndex, columnIndex, null)
        {
        }

        private void StartWork()
        {
            Self.Tell(StartHandShakeMessage.Instance);
        }

        public CellActor(IActorRef printerActor, int rowIndex, int columnIndex, int? cellNumber)
        {
            this.cellNumber = cellNumber;
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
            this.printerActor = printerActor;
            Receive<StartHandshakesMessage>(_ => StartWork());
            Receive<StartHandShakeMessage>(_ => StartHandshake());
            Receive<HandshakeRequestMessage>(request => ProcessHandhakeRequest(request));
            Receive<HandshakeResponseMessage>(response => ProcessHandhakeResponse(response));
            Receive<StartSolvingMessage>(_ => StartSolving());
            Receive<IHaveThisNumber>(message => ProcessNumberReceived(message.Number));
            Receive<SendStateMessage>(_ => SendState());
            Receive<NakedTweenFindingMessage>(message => CheckNakedSingle(message));
        }

        private void CheckNakedSingle(NakedTweenFindingMessage message)
        {
            if (possibleNumbers.Count == 2 && message.PairValues.TrueForAll(x => possibleNumbers.Contains(x)))
            {
                RemoveThisNumbersMessage removeThisNumbersMessage = new RemoveThisNumbersMessage(possibleNumbers.ToArray());

                switch (message.CellNeighbourhood)
                {
                    case CellNeighbourhood.Row:
                        rowCells.AsParallel().ForAll(x => x.Tell(removeThisNumbersMessage));
                        break;
                    case CellNeighbourhood.Column:
                        columnCells.AsParallel().ForAll(x => x.Tell(removeThisNumbersMessage));
                        break;
                    case CellNeighbourhood.Square:
                        squareCells.AsParallel().ForAll(x => x.Tell(removeThisNumbersMessage));
                        break;
                }
            }
        }

        private void SendState()
        {
            WhenHaveCellValue();
        }

        private void ProcessNumberReceived(int number)
        {
            possibleNumbers.Remove(number);
            rowCells.Remove(Sender);
            columnCells.Remove(Sender);
            squareCells.Remove(Sender);

            if (possibleNumbers.Count == 1)
            {
                cellNumber = possibleNumbers[0];
                WhenHaveCellValue();
                return;
            }

            if (possibleNumbers.Count == 2)
            {
                rowCells.AsParallel().ForAll(x => x.Tell(new NakedTweenFindingMessage(CellNeighbourhood.Row, possibleNumbers))); 
                columnCells.AsParallel().ForAll(x => x.Tell(new NakedTweenFindingMessage(CellNeighbourhood.Column, possibleNumbers))); 
                squareCells.AsParallel().ForAll(x => x.Tell(new NakedTweenFindingMessage(CellNeighbourhood.Square, possibleNumbers))); 
            }
        }

        private void StartSolving()
        {
            if (cellNumber.HasValue && !isSolved)
            {
                WhenHaveCellValue();
            }
        }

        private void WhenHaveCellValue()
        {
            if (cellNumber.HasValue && !isSolved)
            {
                isSolved = true;
                IHaveThisNumber message = new IHaveThisNumber(cellNumber.Value);

                BroadcastMessage(message);

                printerActor.Tell(new PrintCellValueMessage(rowIndex, columnIndex, cellNumber));

                Context.Stop(Self);
            }
        }

        private void BroadcastMessage(object message)
        {
            rowCells.AsParallel().ForAll(x => x.Tell(message));
            columnCells.AsParallel().ForAll(x => x.Tell(message));
            squareCells.AsParallel().ForAll(x => x.Tell(message));
        }

        private void ProcessHandhakeResponse(HandshakeResponseMessage response)
        {
            switch (response.Location)
            {
                case CellNeighbourhood.Row:
                    if (Sender != Self)
                    {
                        rowCells.Add(Sender);
                    }
                    break;
                case CellNeighbourhood.Column:
                    if (Sender != Self)
                    {
                        columnCells.Add(Sender);
                    }
                    break;
                case CellNeighbourhood.Square:
                    if (Sender != Self)
                    {
                        squareCells.Add(Sender);
                    }
                    break;
            }

            if (rowCells.Count == 8 && columnCells.Count == 8 && squareCells.Count == 8)
            {
                Context.Parent.Tell(new HandshakeDoneMessage(cellNumber));
            }
        }

        private void ProcessHandhakeRequest(HandshakeRequestMessage request)
        {
            Sender.Tell(new HandshakeResponseMessage(request.Location));
        }

        private void StartHandshake()
        {
            Context.ActorSelection($"../Cell-{rowIndex}-*").Tell(new HandshakeRequestMessage(CellNeighbourhood.Row));
            Context.ActorSelection($"../Cell-*-{columnIndex}").Tell(new HandshakeRequestMessage(CellNeighbourhood.Column));
            int blockIndex = Constants.CellToBlockIndex[$"{rowIndex}-{columnIndex}"];
            var lst = Constants.BlockCoordinates[blockIndex];

            foreach (string index in lst)
            {
                Context.ActorSelection($"../Cell-{index}").Tell(new HandshakeRequestMessage(CellNeighbourhood.Square));
            }
        }

        public static Props Props(IActorRef printerActor, int rowIndex, int columnIndex)
        {
            return Akka.Actor.Props.Create(() => new CellActor(printerActor, rowIndex, columnIndex));
        }

        public static Props Props(IActorRef printerActor, int rowIndex, int columnIndex, int cellValue)
        {
            return Akka.Actor.Props.Create(() => new CellActor(printerActor, rowIndex, columnIndex, cellValue));
        }
    }
}
