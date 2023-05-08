using System.ComponentModel.DataAnnotations;


namespace GrooveBoxDesktop.Attributes;
public class RequiredEnumAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value == null || value is not Enum) return false;
        return Enum.IsDefined(value.GetType(), value);
    }
}