using System.ComponentModel.DataAnnotations;

namespace GrooveBoxLibrary.Attributes;

public class NoSpaceAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is null)
            return true;

        string property = value.ToString();
        return !property.Contains(' ');
    }
}
