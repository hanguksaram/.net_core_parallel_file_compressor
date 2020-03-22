using System.IO;

namespace Gzip.Test
{
    internal sealed class ProgramStateModel
    {
        private static ProgramStateModel _stateModel;
        internal (FileInfo, FileInfo) InputOutputFileInfos { get; }
        internal (Stream, Stream) InputOutputStreams { get; private set; }
        internal ProgramModeType ProgramMode { get; }
        
        internal static ProgramStateModel Of(FileInfo inputFileInfo, FileInfo outputFileInfo, ProgramModeType modeType)
        {
            if (_stateModel != null)
                return _stateModel;
            _stateModel = new ProgramStateModel(inputFileInfo, outputFileInfo, modeType);
            return _stateModel;
        }
        internal ProgramStateModel Of(Stream inputStream, Stream outputStream)
        {
            InputOutputStreams = (inputStream, outputStream);
            return this;
        }

        private ProgramStateModel(FileInfo inputFileInfo, FileInfo outputFileInfo, ProgramModeType modeType)
        {
            InputOutputFileInfos = (inputFileInfo, outputFileInfo);
            ProgramMode = modeType;
        }

        private ProgramStateModel(FileInfo inputFileInfo, FileInfo outputFileInfo,
            ProgramModeType modeType, Stream inputStream, Stream outputStream) : this(inputFileInfo, outputFileInfo,
            modeType)
        {
            InputOutputStreams = (inputStream, outputStream);
        }

        public void Deconstruct(out (Stream, Stream) streams, out ProgramModeType mode)
        {
            streams = InputOutputStreams;
            mode = ProgramMode;
        }
        
        
        public enum ProgramModeType : byte
        {
            Default = 0,
            Compress = 1,
            Decompress = 2
        }
    }
}