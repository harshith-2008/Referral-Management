namespace Referral_Management.Api.Exceptions;

public class ForbiddenException : ApiException
{
    public ForbiddenException(string message): base(message, 403)
    {
    }
}