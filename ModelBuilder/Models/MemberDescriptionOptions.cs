
namespace ModelBuilder.Models;
public class MemberDescriptionOptions
{
    public List<Func<string, string>> Cleaner=new();
    public List<Func<string,bool>> Validators = new();
    public List<Func<object,bool>> PosValidators = new();
    public bool AllowNull { get; set; } = false;
    public object? DefaultValue { get; set; }


    public bool PosValidated(object value)
    {
        return !this.PosValidators.Any(validator => !validator(value));
    }

    public bool TryProcessOptions(ref string value)
    {
        foreach (var cleaner in Cleaner)
        {
            value = cleaner(value);
        }

        foreach (var validator in Validators)
        {
            if (!validator(value))
            {
                return false;
            }
        }
        return true;
    }
}

