using Factory.Api.Data.Entities;
using Factory.Api.Database;
using Factory.Api.Extensions;
using Factory.Api.Utilities;
using Factory.Shared;
using Microsoft.EntityFrameworkCore;

namespace Factory.Api.Repositories.Purchases
{
    // Implementation class for IPurchaseRepository
    public class PurchaseRepository:IPurchaseRepository
    {
        private readonly AppDbContext context;

        public PurchaseRepository(AppDbContext context)
        {
            this.context = context;
        }

        // Create new Purchase
        public async Task CreateNewPurchaseAsync(PurchaseDto purchaseDto)
        {
            // Create new Purchase object
            Purchase purchase = new();

            // Set it's properties to the ones contained in purchaseDto
            purchase.Code = purchaseDto.Code;
            purchase.PurchaseDate = purchaseDto.PurchaseDate;
            purchase.Supplier = (await context.Suppliers.Where(e => e.Name == purchaseDto.SupplierName).FirstOrDefaultAsync())!;

            // Add purchase to database
            await context.Purchases.AddAsync(purchase);

            // Iterate through purchaseDto.PurchaseDetailsList,
            // set properties of each purchaseDetail to the ones
            // contained in purchaseDto.PurchaseDetailsList,
            // and add each purchaseDetail to database
            foreach (var purchaseDetailDto in purchaseDto.PurchaseDetailList)
            {
                PurchaseDetail purchaseDetail = new();

                purchaseDetail.Purchase = purchase;
                purchaseDetail.Material = (await context.Materials.Where(e => e.Name == purchaseDetailDto.MaterialName).FirstOrDefaultAsync())!;
                purchaseDetail.Qty = purchaseDetailDto.Qty;
                purchaseDetail.Material.Quantity += purchaseDetailDto.Qty;

                await context.PurchaseDetails.AddAsync(purchaseDetail);
            }
        }

        // Delete selected Purchase
        public async Task DeletePurchaseAsync(int id)
        {
            // Find Purchase record from database by Primary Key value
            Purchase purchase = (await context.Purchases
                .Include(e => e.PurchaseDetails)
                .ThenInclude(e => e.Material)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id))!;

            // Delete all PurchaseDetail records
            // contained in purchase.PurchaseDetails
            foreach (var purchaseDetail in purchase.PurchaseDetails)
            {
                context.PurchaseDetails.Remove(purchaseDetail);

                purchaseDetail.Material.Quantity -= purchaseDetail.Qty;
            }

            // Remove purchase from database
            context.Purchases.Remove(purchase);
        }

        // Edit selected Purchase
        public async Task EditPurchaseAsync(PurchaseDto purchaseDto)
        {
            // Find Purchase record from database by Primary Key value
            Purchase purchase = (await context.Purchases
                .Include(e => e.Supplier)
                .Include(e => e.PurchaseDetails)
                .ThenInclude(e => e.Material)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == purchaseDto.Id))!;

            // Set it's properties to the ones contained in purchaseDto
            purchase.Code = purchaseDto.Code;
            purchase.PurchaseDate = purchaseDto.PurchaseDate;
            purchase.Supplier = (await context.Suppliers.Where(e => e.Name == purchaseDto.SupplierName).FirstOrDefaultAsync())!;

            // Delete all PurchaseDetail records
            // contained in purchase.PurchaseDetails
            foreach (var purchaseDetail in purchase.PurchaseDetails)
            {
                context.PurchaseDetails.Remove(purchaseDetail);

                purchaseDetail.Material.Quantity -= purchaseDetail.Qty;
            }

            // Iterate through purchaseDto.PurchaseDetailsList,
            // set properties of each purchaseDetail to the ones
            // contained in purchaseDto.PurchaseDetailsList,
            // and add each purchaseDetail to database
            foreach (var purchaseDetailDto in purchaseDto.PurchaseDetailList)
            {
                PurchaseDetail purchaseDetail = new();

                purchaseDetail.Purchase = purchase;
                purchaseDetail.Material = (await context.Materials.Where(e => e.Name == purchaseDetailDto.MaterialName).FirstOrDefaultAsync())!;
                purchaseDetail.Qty = purchaseDetailDto.Qty;
                purchaseDetail.Material.Quantity += purchaseDetailDto.Qty;

                await context.PurchaseDetails.AddAsync(purchaseDetail);
            }
        }

        // Return paginated collection of PurchaseDto objects
        public async Task<Pagination<PurchaseDto>> GetPurchasesCollectionAsync(string? searchText, string? stringDate, string? supplier, int pageIndex, int pageSize)
        {
            // Return all Purchase objects
            var allPurchases = context.Purchases
                .Include(e => e.Supplier)
                .Include(e => e.PurchaseDetails)
                .ThenInclude(e => e.Material)
                .AsNoTracking()
                .AsQueryable();

            // If searchText is not null or empty string,
            // then filter allPurchases by searchText
            if (!string.IsNullOrEmpty(searchText))
            {
                allPurchases = allPurchases.Where(e => e.Code.ToLower().Contains(searchText.ToLower()))
                    .Include(e => e.Supplier)
                    .Include(e => e.PurchaseDetails)
                    .ThenInclude(e => e.Material)
                    .AsNoTracking()
                    .AsQueryable();
            }

            // If purchaseDate is not null or empty string,
            // then filter allPurchases by purchaseDate
            if (!string.IsNullOrEmpty(stringDate))
            {
                DateTime purchaseDate = DateTime.Parse(stringDate);

                allPurchases = allPurchases.Where(e => e.PurchaseDate == purchaseDate)
                    .Include(e => e.Supplier)
                    .Include(e => e.PurchaseDetails)
                    .ThenInclude(e => e.Material)
                    .AsNoTracking()
                    .AsQueryable();
            }

            // If supplier is not null or empty string,
            // then filter allPurchases by supplier
            if (!string.IsNullOrEmpty(supplier))
            {
                allPurchases = allPurchases.Where(e => e.Supplier == context.Suppliers.FirstOrDefault(e => e.Name == supplier))
                    .Include(e => e.Supplier)
                    .Include(e => e.PurchaseDetails)
                    .ThenInclude(e => e.Material)
                    .AsNoTracking()
                    .AsQueryable();
            }

            // Variable that will contain PurchaseDto objects
            List<PurchaseDto> purchaseDtos = new();

            // Iterate through allPurchases and populate purchaseDtos
            foreach (var purchase in allPurchases)
            {
                purchaseDtos.Add(purchase.ConvertToDto());
            }

            // Using PaginationUtility's GetPaginatedResult method, 
            // return Pagination<PurchaseDto> object
            var paginatedResult = PaginationUtility<PurchaseDto>.GetPaginatedResult(in purchaseDtos, pageIndex, pageSize);

            return await Task.FromResult(paginatedResult);
        }

        // Return single PurchaseDto object
        public async Task<PurchaseDto> GetSinglePurchaseAsync(int id)
        {
            // Find Purchase record from database by Primary Key value
            Purchase? purchase = await context.Purchases
                .Include(e => e.Supplier)
                .Include(e => e.PurchaseDetails)
                .ThenInclude(e => e.Material)
                .FirstOrDefaultAsync(e => e.Id == id);

            return purchase != null ? purchase.ConvertToDto() : new PurchaseDto();
        }

        // Custom validation
        public async Task<Dictionary<string, string>> ValidatePurchaseAsync(PurchaseDto purchaseDto)
        {
            // Variable that will contain possible validation errors
            Dictionary<string, string> errors = new();

            // Return all Purchase objects
            var allPurchases = context.Purchases
                .Include(e => e.Supplier)
                .Include(e => e.PurchaseDetails)
                .ThenInclude(e => e.Material)
                .AsNoTracking()
                .AsQueryable();

            // If purchaseDto's Id value is larger than 0
            // it means that Purchase is used in Edit operation
            if (purchaseDto.Id > 0)
            {
                // Find Purchase record from database by Primary Key value
                Purchase purchase = (await context.Purchases
                    .Include(e => e.Supplier)
                    .Include(e => e.PurchaseDetails)
                    .ThenInclude(e => e.Material)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == purchaseDto.Id))!;

                // If purchaseDtoDto's Code value is not equal to purchase's
                // Code value, it means that user has modified Code value. 
                // Therefore we check for Code uniqueness among all Purchase records
                if (purchase.Code != purchaseDto.Code)
                {
                    // If purchaseDto's Code value is already contained
                    // in any of the Purchase records in database,
                    // then add validation error to errors Dictionary
                    if (allPurchases.Select(e => e.Code.ToLower()).Contains(purchaseDto.Code.ToLower()))
                    {
                        errors.Add("Code", "There is already Purchase with this Code in database. Please provide different Code.");
                    }
                }
            }
            // Otherwise it means that Purchase is used in Create operation
            else
            {
                // If purchaseDto's Code value is already contained
                // in any of the Purchase records in database,
                // then add validation error to errors Dictionary
                if (allPurchases.Select(e => e.Code.ToLower()).Contains(purchaseDto.Code.ToLower()))
                {
                    errors.Add("Code", "There is already Purchase with this Code in database. Please provide different Code.");
                }
            }

            // Validate that user has entered at least
            // one material in Purchase's products list
            if (purchaseDto.PurchaseDetailList.Count <= 0)
            {
                errors.Add("PurchaseDetailsList", "There must be at least one Material in purchase's materials list!");
            }

            return errors;
        }

        // Return all Purchase dates
        public async Task<List<string>> GetAllPurchaseDatesAsync()
        {
            // Return all Purchases
            var allPurchases = context.Purchases
                .AsNoTracking()
                .AsQueryable();

            // Variable that will hold Purchase dates
            List<string> purchaseDates = new();

            // Iterate through allPurchases and populate purchaserDates
            foreach (var purchase in allPurchases)
            {
                purchaseDates.Add(purchase.PurchaseDate.ToShortDateString());
            }

            return await Task.FromResult(purchaseDates.Distinct().ToList());
        }
    }
}
