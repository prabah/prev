namespace SqlInjection.Prevention.IO.Borker
{
    public interface ISqlQueryProcessor
    {
        string GetQueryValidated();
        string ValidateQuery();
    }
}
