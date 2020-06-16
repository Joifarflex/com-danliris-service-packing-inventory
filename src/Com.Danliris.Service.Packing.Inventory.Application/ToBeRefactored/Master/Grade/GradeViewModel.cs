﻿using Com.Danliris.Service.Packing.Inventory.Application.Utilities;
using Com.Danliris.Service.Packing.Inventory.Infrastructure.Repositories.Master;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Com.Danliris.Service.Packing.Inventory.Application.ToBeRefactored.Master.Grade
{
    public class GradeViewModel : BaseViewModel, IValidatableObject
    {
        public string Type { get; set; }
        public string Code { get; set; }
        public bool IsAvalGrade { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var repository = validationContext.GetService<IGradeRepository>();
            if (string.IsNullOrEmpty(Type))
            {
                yield return new ValidationResult("Grade harus diisi", new List<string> { "Type" });
            }
            else
            {
                if (repository.ReadAll().Any(d => d.Type == Type && d.Id != Id))
                {
                    yield return new ValidationResult("Grade tidak boleh duplikat", new List<string> { "Type" });
                }

            }

            if (string.IsNullOrEmpty(Code))
            {
                yield return new ValidationResult("Kode harus diisi", new List<string> { "Code" });
            }
            else
            {
                int outCode;
                if (!int.TryParse(Code, out outCode))
                {
                    yield return new ValidationResult("Kode harus numerik", new List<string> { "Code" });
                }
                else if (outCode >= 10)
                {
                    yield return new ValidationResult("Panjang digit Kode tidak boleh lebih dari 1", new List<string> { "Code" });
                }
                else if (outCode == 0)
                {
                    yield return new ValidationResult("Kode tidak boleh sama dengan 0", new List<string> { "Code" });
                }
                else
                {
                    if (repository.ReadAll().Any(d => d.Code == Code && d.Id != Id))
                    {
                        yield return new ValidationResult("Kode tidak boleh duplikat", new List<string> { "Code" });
                    }
                }
            }
        }
    }
}
