using System;

namespace FitnessAssistant.Api.Shared.FileUpload;

public class FileUploadResult
{
    public bool IsSuccess { get; set; }
    public string? FileUrl { get; set; }
    public string? ErrorMessage { get; set; }
}
