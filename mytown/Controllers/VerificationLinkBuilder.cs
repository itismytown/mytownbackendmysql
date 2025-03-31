using System;

namespace mytown.Services
{
    public class VerificationLinkBuilder : IVerificationLinkBuilder
    {
        public string BuildLink(string frontendBaseUrl, string token)
        {
            if (string.IsNullOrWhiteSpace(frontendBaseUrl))
            {
                throw new ArgumentException("Frontend base URL must not be null or empty.", nameof(frontendBaseUrl));
            }

            return $"{frontendBaseUrl.TrimEnd('/')}/verify-shopper-email?token={token}";
        }
    }
}
