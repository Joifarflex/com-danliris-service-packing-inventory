﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Packing.Inventory.Application.ToBeRefactored.MaterialDeliveryNote
{
    public class MaterialDeliveryNoteValidator : AbstractValidator<MaterialDeliveryNoteViewModel>
    {
        public MaterialDeliveryNoteValidator()
        {
            RuleFor(data => data.DateSJ).Must(s => s != default(DateTimeOffset)).WithMessage("Tanggal S.J Harus Diisi!");
            RuleFor(data => data.BonCode).NotNull().WithMessage("Kode Bon Harus Diisi!");

            //RuleFor(data => data.Date).Must(s => s != default(DateTimeOffset)).WithMessage("Tanggal Harus Diisi!")
            //    .Must(s => s >= DateTimeOffset.UtcNow || ((DateTimeOffset.UtcNow - s).TotalDays <= 1 && (DateTimeOffset.UtcNow - s).TotalDays >= 0)).WithMessage("Tanggal Harus Lebih Besar atau Sama Dengan Hari Ini");
        }

    }
}
