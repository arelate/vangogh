using System;
using System.IO;
using System.Threading.Tasks;
using Attributes;
using Delegates.Conversions.Hashes;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Conversions;

namespace Delegates.Confirmations.Validation
{
    public class ConfirmChunkHashExpectationAsyncDelegate: IConfirmExpectationAsyncDelegate<(Stream FileStream, long From, long To), string>
    {
        private IConvertAsyncDelegate<byte[], Task<string>> convertBytesToHashAsyncDelegate;

        [Dependencies(
            typeof(ConvertBytesToMd5HashDelegate))]
        public ConfirmChunkHashExpectationAsyncDelegate(
            IConvertAsyncDelegate<byte[], Task<string>> convertBytesToHashAsyncDelegate)
        {
            this.convertBytesToHashAsyncDelegate = convertBytesToHashAsyncDelegate;
        }
        
        public async Task<bool> ConfirmAsync((Stream FileStream, long From, long To) chunk, string expectedHash)
        {
            if (!chunk.FileStream.CanSeek)
                throw new Exception("Unable to seek in the file stream");
            
            chunk.FileStream.Seek(chunk.From, SeekOrigin.Begin);

            var length = (int) (chunk.To - chunk.From + 1);
            var buffer = new byte[length];
            await chunk.FileStream.ReadAsync(buffer, 0, length);

            var actualHash = await convertBytesToHashAsyncDelegate.ConvertAsync(buffer);

            return actualHash == expectedHash;
        }
    }
}