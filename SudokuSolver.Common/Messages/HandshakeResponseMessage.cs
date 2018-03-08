using Akka.Actor;

namespace SudokuSolver.Common.Messages
{
    public class HandshakeResponseMessage
    {
        public IActorRef Myself { get; }
        public string Location { get; }
        public HandshakeResponseMessage(IActorRef myself,string location)
        {
            Myself = myself;
            Location = location;
        }
    }
}
