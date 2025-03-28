using Craft.LocalizationModule.Dtos;
using Craft.LocalizationModule.Services;
using FluentValidation;

namespace Craft.LocalizationModule.validators;

/// <summary>
///   Validator for CreateLocaleDto and UpdateLocaleDto
/// </summary>
public class CreateLocaleDtoValidator : AbstractValidator<CreateLocaleDto>
{
    /// <summary>
    ///     Default constructor
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public CreateLocaleDtoValidator()
    {
        RuleFor(l => l.CultureCode)
            .Custom(
                (c, ctx) =>
                {
                    var codes = CultureInfo
                        .AllCultures.Select(c => c.Key)
                        .ToList()
                        .AsReadOnly();
                    if (!codes.Contains(c.ToString()))
                    {
                        throw new InvalidOperationException(
                            $"Culture code {c} is not valid."
                        );
                    }
                }
            );

        RuleFor(l => l.Key)
            .Custom(
                (c, ctx) =>
                {
                    if (
                        string.IsNullOrEmpty(c) && c.Length < 3
                        || c.Length > 30
                    )
                    {
                        throw new InvalidOperationException(
                            "Key must be between 3 and 30 characters."
                        );
                    }
                }
            );

        RuleFor(l => l.Value)
            .Custom(
                (c, ctx) =>
                {
                    if (c.Length > 1000)
                    {
                        throw new InvalidOperationException(
                            "Value must not be more than 1000 characters."
                        );
                    }
                }
            );
    }
}
