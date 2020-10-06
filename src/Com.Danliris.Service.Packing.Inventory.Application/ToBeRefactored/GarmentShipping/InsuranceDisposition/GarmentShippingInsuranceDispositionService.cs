﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.Danliris.Service.Packing.Inventory.Application.CommonViewModelObjectProperties;
using Com.Danliris.Service.Packing.Inventory.Application.ToBeRefactored.CommonViewModelObjectProperties;
using Com.Danliris.Service.Packing.Inventory.Application.Utilities;
using Com.Danliris.Service.Packing.Inventory.Data.Models.Garmentshipping.InsuranceDisposition;
using Com.Danliris.Service.Packing.Inventory.Infrastructure.Repositories.GarmentShipping.InsuranceDisposition;
using Com.Danliris.Service.Packing.Inventory.Infrastructure.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Com.Danliris.Service.Packing.Inventory.Application.ToBeRefactored.GarmentShipping.InsuranceDisposition
{
    public class GarmentShippingInsuranceDispositionService : IGarmentShippingInsuranceDispositionService
    {
        private readonly IGarmentShippingInsuranceDispositionRepository _repository;
        private readonly IServiceProvider serviceProvider;

        public GarmentShippingInsuranceDispositionService(IServiceProvider serviceProvider)
        {
            _repository = serviceProvider.GetService<IGarmentShippingInsuranceDispositionRepository>();

            this.serviceProvider = serviceProvider;
        }

        private GarmentShippingInsuranceDispositionViewModel MapToViewModel(GarmentShippingInsuranceDispositionModel model)
        {
            var vm = new GarmentShippingInsuranceDispositionViewModel()
            {
                Active = model.Active,
                Id = model.Id,
                CreatedAgent = model.CreatedAgent,
                CreatedBy = model.CreatedBy,
                CreatedUtc = model.CreatedUtc,
                DeletedAgent = model.DeletedAgent,
                DeletedBy = model.DeletedBy,
                DeletedUtc = model.DeletedUtc,
                IsDeleted = model.IsDeleted,
                LastModifiedAgent = model.LastModifiedAgent,
                LastModifiedBy = model.LastModifiedBy,
                LastModifiedUtc = model.LastModifiedUtc,

                bankName = model.BankName,
                dispositionNo = model.DispositionNo,
                insurance = new Insurance
                {
                    Id = model.InsuranceId,
                    Code = model.InsuranceCode,
                    Name=model.InsuranceName
                },
                paymentDate = model.PaymentDate,
                policyType = model.PolicyType,
                
                rate = model.Rate,
                remark = model.Remark,
                items = model.Items.Select(i => new GarmentShippingInsuranceDispositionItemViewModel
                {
                    Active = i.Active,
                    Id = i.Id,
                    CreatedAgent = i.CreatedAgent,
                    CreatedBy = i.CreatedBy,
                    CreatedUtc = i.CreatedUtc,
                    DeletedAgent = i.DeletedAgent,
                    DeletedBy = i.DeletedBy,
                    DeletedUtc = i.DeletedUtc,
                    IsDeleted = i.IsDeleted,
                    LastModifiedAgent = i.LastModifiedAgent,
                    LastModifiedBy = i.LastModifiedBy,
                    LastModifiedUtc = i.LastModifiedUtc,
                    amount = i.Amount,
                    currencyRate = i.CurrencyRate,
                    BuyerAgent = new BuyerAgent
                    {
                        Id = i.BuyerAgentId,
                        Code = i.BuyerAgentCode,
                        Name = i.BuyerAgentName

                    },
                    insuranceDispositionId = i.InsuranceDispositionId,
                    invoiceId = i.InvoiceId,
                    invoiceNo = i.InvoiceNo,
                    policyDate = i.PolicyDate,
                    policyNo = i.PolicyNo,
                }).ToList(),
                unitCharge = model.UnitCharge.Select(i => new GarmentShippingInsuranceDispositionUnitChargeViewModel
                {
                    unit = new Unit
                    {
                        Id = i.UnitId,
                        Code = i.UnitCode
                    },
                    Id = i.Id,
                    amount = i.Amount,
                    insuranceDispositionId=i.InsuranceDispositionId

                }).ToList(),

            };
            return vm;
        }
        private GarmentShippingInsuranceDispositionModel MapToModel(GarmentShippingInsuranceDispositionViewModel viewModel)
        {
            var items = (viewModel.items ?? new List<GarmentShippingInsuranceDispositionItemViewModel>()).Select(i =>
            {

                i.BuyerAgent = i.BuyerAgent ?? new BuyerAgent();
                return new GarmentShippingInsuranceDispositionItemModel(i.policyDate,i.policyNo,i.invoiceNo,i.invoiceId,i.BuyerAgent.Id,i.BuyerAgent.Code,i.BuyerAgent.Name,i.amount,i.currencyRate)
                {
                    Id = i.Id
                };

            }).ToList();


            viewModel.insurance = viewModel.insurance ?? new Insurance();

            var unitCharge = (viewModel.unitCharge ?? new List<GarmentShippingInsuranceDispositionUnitChargeViewModel>()).Select(m => {

                m.unit = m.unit ?? new Unit();
                return new GarmentShippingInsuranceDispositionUnitChargeModel(m.unit.Id, m.unit.Code, m.amount)
                {
                    Id = m.Id
                };
            }).ToList();

            GarmentShippingInsuranceDispositionModel garmentShippingInvoiceModel = new GarmentShippingInsuranceDispositionModel(GenerateNo(viewModel), viewModel.policyType, viewModel.paymentDate.GetValueOrDefault(),viewModel.bankName,viewModel.insurance.Id,viewModel.insurance.Name,viewModel.insurance.Code,viewModel.rate,viewModel.remark,unitCharge,items);

            return garmentShippingInvoiceModel;
        }

        private string GenerateNo(GarmentShippingInsuranceDispositionViewModel vm)
        {
            var year = DateTime.Now.ToString("yyyy");
            var month = DateTime.Now.ToString("MM");

            var prefix = $"DL/Polis Asuransi/{month}/{year}/";

            var lastInvoiceNo = _repository.ReadAll().Where(w => w.DispositionNo.StartsWith(prefix))
                .OrderByDescending(o => o.DispositionNo)
                .Select(s => int.Parse(s.DispositionNo.Replace(prefix, "")))
                .FirstOrDefault();
            var invoiceNo = $"{prefix}{(lastInvoiceNo + 1).ToString("D3")}";

            return invoiceNo;
        }
        public async Task<int> Create(GarmentShippingInsuranceDispositionViewModel viewModel)
        {
            GarmentShippingInsuranceDispositionModel model = MapToModel(viewModel);

            int Created = await _repository.InsertAsync(model);

            return Created;
        }

        public async Task<int> Delete(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public ListResult<GarmentShippingInsuranceDispositionViewModel> Read(int page, int size, string filter, string order, string keyword)
        {
            var query = _repository.ReadAll();
            List<string> SearchAttributes = new List<string>()
            {
                "DispositionNo","InsuranceNo","BankName","PolicyType"
            };
            query = QueryHelper<GarmentShippingInsuranceDispositionModel>.Search(query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            query = QueryHelper<GarmentShippingInsuranceDispositionModel>.Filter(query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<GarmentShippingInsuranceDispositionModel>.Order(query, OrderDictionary);

            var data = query
                .Skip((page - 1) * size)
                .Take(size)
                .Select(model => MapToViewModel(model))
                .ToList();

            return new ListResult<GarmentShippingInsuranceDispositionViewModel>(data, page, size, query.Count());
        }

        public async Task<GarmentShippingInsuranceDispositionViewModel> ReadById(int id)
        {
            var data = await _repository.ReadByIdAsync(id);

            var viewModel = MapToViewModel(data);

            return viewModel;
        }

        public async Task<int> Update(int id, GarmentShippingInsuranceDispositionViewModel viewModel)
        {
            GarmentShippingInsuranceDispositionModel model = MapToModel(viewModel);

            return await _repository.UpdateAsync(id, model);
        }
    }
}
