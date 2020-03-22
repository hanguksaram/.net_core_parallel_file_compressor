using System;
using System.IO;
using Gzip.Test.Core;
using Gzip.Test.Impl;

namespace Gzip.Test
{
    internal static class ProgramStateModelExtensions
    {
        internal static ProgramStateModel OpenStreams(this ProgramStateModel model) =>
            FileReader.OpenFiles(model);

        internal static Result<ExitCode> ProcessFilesOperation(this ProgramStateModel model)
        {
            using var worker = model switch
            {
                {ProgramMode: ProgramStateModel.ProgramModeType.Compress, InputOutputStreams: var (inputStream, outputStream)}
                    => new StreamHandler<Stream>(new ParallelCompressionHandler(new GzipCompressor()), inputStream, outputStream),
                
                {ProgramMode: ProgramStateModel.ProgramModeType.Decompress, InputOutputStreams: var (inputStream, outputStream)}
                    => new StreamHandler<Stream>(new ParallelDecompressionHandler(new GzipDecompressor()), inputStream, outputStream),
                
                _ => throw new ArgumentException()

            };
            try
            { 
                worker.ExecuteOperation();
                return Result.Of(ExitCode.Success);
            }
            catch (Exception ex)
            {
               return Result.Error<ExitCode>(ex);
            }
            
        }
        
    }
}