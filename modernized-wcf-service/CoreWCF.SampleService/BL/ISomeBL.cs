namespace CoreWCF.SampleService.BL
{
    public interface ISomeBL
    {
        string DivideByZeroErrorMessage { get; }
    }

    public class SomeBL : ISomeBL
    {
        public string DivideByZeroErrorMessage => "Invalid Argument: The second argument must not be zero.";
    }
}
