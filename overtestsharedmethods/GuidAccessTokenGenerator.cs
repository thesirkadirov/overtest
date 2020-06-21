using System;

namespace Sirkadirov.Overtest.Libraries.Shared.Methods
{
    
    public static class GuidAccessTokenGenerator
    {

        public static string Generate()
        {
            return Convert
                .ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("=", "")
                .Replace("+", "")
                .Replace("/", "");
        }
        
    }
    
}