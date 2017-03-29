﻿using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.Validation
{
    public interface IValidateFilenameDelegate
    {
        void ValidateFilename(string uri, string expectedFilename);
    }

    public interface IValidateSizeDelegate
    {
        void ValidateSize(string uri, long expectedSize);
    }

    public interface IValidateChunkDelegate
    {
        Task ValidateChunkAsync(System.IO.Stream fileStream, IValidationChunk chunk);
    }

    public interface IValidateDelegate
    {
        Task ValidateAsync(string uri, string validationUri, IStatus status);
    }

    public interface IValidationController:
        IValidateFilenameDelegate,
        IValidateSizeDelegate,
        IValidateChunkDelegate,
        IValidateDelegate
    {
        // ...
    }
}
