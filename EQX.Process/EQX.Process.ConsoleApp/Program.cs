// See https://aka.ms/new-console-template for more information
using EQX.Core.Sequence;
using EQX.Process;

Console.WriteLine("Hello, World!");

IProcess<ERunMode> process = new ProcessBase<ERunMode>();
process.Start();

while (true)
{
    Console.ReadLine();
    Console.Clear();
}