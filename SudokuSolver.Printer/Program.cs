using Akka.Actor;
using Akka.Configuration;
using SudokuSolver.Common.Messages;

namespace SudokuSolver.Printer
{
    class Program
    {
        static void Main(string[] args)
        {

            var config = ConfigurationFactory.ParseString(@"
            akka {
                actor {
                    provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                }
                remote {
                    dot - netty.tcp {
                        port = 0
              
            hostname = localhost
                      }
                }
            }
            ");

            using (var system = ActorSystem.Create("printerClient", config))
            {
                var chatClient = system.ActorOf(PrinterActor.Props());
                chatClient.Tell(new FindSudokuGameMessage());

                system.WhenTerminated.Wait();
            }
        }
    }
}
