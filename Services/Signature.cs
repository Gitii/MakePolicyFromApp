using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MakePolicyFromApp.Security;

namespace MakePolicyFromApp.Services
{
    public readonly struct Signature
    {
        public string FileName { get; init; }

        public bool IsValid { get; init; }

        public string VerificationDetails { get; init; }
    }

    public interface ISignatureVerifier
    {
        public Signature VerifySignature(string fileName);
    }

    class SignatureVerifier : ISignatureVerifier
    {
        public Signature VerifySignature(string fileName)
        {
            var (ok, details) = WinTrust.VerifyEmbeddedSignature(fileName);

            return new Signature()
            {
                FileName = fileName,
                IsValid = ok,
                VerificationDetails = details
            };
        }
    }
}