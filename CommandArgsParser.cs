using System;
using System.IO;

namespace Gzip.Test
{
    public static class CommandArgsParser
    {
        internal static ProgramStateModel InitializeProgramState(ProgramStateModel.ProgramModeType mode, FileInfo sourceFile, FileInfo targetFile)
        {
            if (!sourceFile.Exists)
                throw new ArgumentNullException("Source file does not exists");

            return ProgramStateModel.Of(sourceFile, targetFile, mode);

        }
    }
}