using System.IO;

namespace Gzip.Test.Impl
{
    internal sealed class FileReader
    { 
        public static ProgramStateModel OpenFiles(ProgramStateModel stateModel)
        {
            var (sourceFile, targetFile) = stateModel.InputOutputFileInfos;
            
            var sourceFileStream = new FileStream(sourceFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            
            try
            {
                return stateModel.Of(new FileStream(targetFile.FullName, FileMode.Truncate), sourceFileStream);
            }
            catch (FileNotFoundException)
            {
                return stateModel.Of(new FileStream(targetFile.FullName, FileMode.CreateNew), sourceFileStream);
            }
            
        }
    }
}