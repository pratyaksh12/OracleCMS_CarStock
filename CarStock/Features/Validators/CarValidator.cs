using System;
using CarStock.Features.Cars;
using FastEndpoints;
using FluentValidation;
using CarStock.Features;

namespace CarStock.Features.Validators;

public class CarValidator : Validator<AddCarRequest>
{
    public CarValidator()
    {
        RuleFor(x => x.Make).NotEmpty();
        RuleFor(x => x.Model).NotEmpty();
        RuleFor(x => x.Year).GreaterThan(1885);
        RuleFor(x => x.StockLevel).GreaterThanOrEqualTo(0);
    }
}
