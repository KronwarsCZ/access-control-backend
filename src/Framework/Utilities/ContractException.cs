namespace Framework.Utilities;

public class ContractException : Exception
{
    public ContractException(string message)
    {
        Message = message;
    }

    public override string Message { get; }
}
