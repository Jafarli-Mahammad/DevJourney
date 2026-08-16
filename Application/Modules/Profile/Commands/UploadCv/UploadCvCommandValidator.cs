using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;

namespace Application.Modules.Profile.Commands.UploadCv
{
    public class UploadCvCommandValidator : AbstractValidator<UploadCvCommand>
    {
        public UploadCvCommandValidator()
        {
            RuleFor(x => x.File)
                .NotNull().WithMessage("File is required.")
                .Must(file => file.Length <= 5 * 1024 * 1024).WithMessage("File size must not exceed 5MB.")
                .Must(BeAValidFileSignature).WithMessage("Invalid file type. Only PDF or DOCX files are allowed.");
        }

        private bool BeAValidFileSignature(IFormFile file)
        {
            if (file == null) return false;

            using var stream = file.OpenReadStream();
            var buffer = new byte[4];
            if (stream.Read(buffer, 0, 4) < 4) return false;

            // PDF: 25 50 44 46 (%PDF)
            if (buffer[0] == 0x25 && buffer[1] == 0x50 && buffer[2] == 0x44 && buffer[3] == 0x46)
                return true;

            // DOCX (ZIP): 50 4B 03 04 (PK..)
            if (buffer[0] == 0x50 && buffer[1] == 0x4B && buffer[2] == 0x03 && buffer[3] == 0x04)
                return true;

            return false;
        }
    }
}
