using System;
using System.IO;
using System.Threading;
using Worker = Gzip.Test.CommandArgsParser;
using static Gzip.Test.PlatformDependentDataConfiguration;

namespace Gzip.Test
{
    /// <summary>
    /// target framework: .net core 3.1
    /// application was successfully tested on macOs with 12 virtual processor cores
    /// recommend publish as .net core 3.1 framework-dependent portable deployment and run using command
    /// dotnet run Gzip.Test.dll and following command args:
    /// Usage:
    /// Gzip.Test [options]
    /// Options:
    ///--mode <Compress|Decompress|Default>    mode
    ///--source-file <source-file>             sourceFile
    ///--target-file <target-file>             targetFile
    /// </summary>
    internal class Program
    {
        static int Main(ProgramStateModel.ProgramModeType mode, FileInfo sourceFile, FileInfo targetFile)
        {
            //should add mutex to prevent running the same application in second process;
            var _ = Mutex.TryOpenExisting(AppName, out var mtx)
                ? throw new WaitHandleCannotBeOpenedException()
                : new Mutex(false, AppName);

            return Worker
                .InitializeProgramState(mode, sourceFile, targetFile)
                .OpenStreams()
                .ProcessFilesOperation()
                .OnError(exception => Console.WriteLine(exception.Message))
                .OnSuccess(code => Console.WriteLine("Process successfully completed"))
                .OnComplete(code => (int) code);
        }
    }
}