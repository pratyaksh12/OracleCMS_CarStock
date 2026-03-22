using System;
using System.ComponentModel.DataAnnotations;
using CarStock.Features.Cars;
using FastEndpoints;
using FluentValidation;
using CarStock.Features;

namespace CarStock.Features.Validators;

public class UpdateStockValidator : Validator<UpdateStockRequest>
{
    public UpdateStockValidator()
    {
        RuleFor(x => x.CarId).GreaterThan(0);
        RuleFor(x => x.StockLevel).GreaterThanOrEqualTo(0);
    }
}
