using Factory.Api.Data.Entities;
using Factory.Api.Database;
using Factory.Api.Extensions;
using Factory.Api.Utilities;
using Factory.Shared;
using Microsoft.EntityFrameworkCore;

namespace Factory.Api.Repositories.Productions
{
    // Implementation class for IProductionRepository
    public class ProductionRepository:IProductionRepository
    {
        private readonly AppDbContext context;

        public ProductionRepository(AppDbContext context)
        {
            this.context = context;
        }

        // Create new Production
        public async Task CreateNewProductionAsync(ProductionDto productionDto)
        {
            // Create new Production object
            Production production = new();

            // Set it's properties to the ones contained in productionDto
            production.Code = productionDto.Code;
            production.ProductionDate = productionDto.ProductionDate;
            production.Product = (await context.Products.Where(e => e.Name == productionDto.ProductName).FirstOrDefaultAsync())!;
            production.Qty = productionDto.Qty;

            // Increase Product quantity by quantity in production
            production.Product.Quantity += production.Qty;

            // Decrease all Material quantity used in Product specification
            foreach (var productDetail in production.Product.ProductDetails)
            {
                productDetail.Material.Quantity -= production.Qty * productDetail.QtyMaterial;
            }

            // Add production to database
            await context.Productions.AddAsync(production);
        }

        // Delete selected Production
        public async Task DeleteProductionAsync(int id)
        {
            // Find production record from database by Primary Key value
            Production production = (await context.Productions
                .Include(e => e.Product)
                .ThenInclude(e => e.ProductDetails)
                .ThenInclude(e => e.Material)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id))!;

            // Restore Product quantity created in production
            production.Product.Quantity -= production.Qty;

            // Restore all Material quantity used in Product specification
            foreach (var productDetail in production.Product.ProductDetails)
            {
                productDetail.Material.Quantity += production.Qty * productDetail.QtyMaterial;
            }

            // Remove production from database
            context.Productions.Remove(production);
        }

        // Edit selected Production
        public async Task EditProductionAsync(ProductionDto productionDto)
        {
            // Find production record from database by Primary Key value
            Production production = (await context.Productions
                .Include(e => e.Product)
                .ThenInclude(e => e.ProductDetails)
                .ThenInclude(e => e.Material)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == productionDto.Id))!;

            // Restore Product quantity created in production
            production.Product.Quantity -= production.Qty;

            // Restore all Material quantity used in Product specification
            foreach (var productDetail in production.Product.ProductDetails)
            {
                productDetail.Material.Quantity += production.Qty * productDetail.QtyMaterial;
            }

            // Set production properties to the ones contained in productionDto
            production.Code = productionDto.Code;
            production.ProductionDate = productionDto.ProductionDate;
            production.Product = (await context.Products.Where(e => e.Name == productionDto.ProductName).FirstOrDefaultAsync())!;
            production.Qty = productionDto.Qty;

            // Increase Product quantity by quantity in production
            production.Product.Quantity += production.Qty;

            // Decrease all Material quantity used in Product specification
            foreach (var productDetail in production.Product.ProductDetails)
            {
                productDetail.Material.Quantity -= production.Qty * productDetail.QtyMaterial;
            }
        }

        // Return paginated collection of ProductionDto objects
        public async Task<Pagination<ProductionDto>> GetProductionsCollectionAsync(string? searchText, string stringDate, string productName, int pageIndex, int pageSize)
        {
            // Return all Production objects
            var allProductions = context.Productions
                .Include(e => e.Product)
                .AsNoTracking()
                .AsQueryable();

            // If searchText is not null or empty string,
            // then filter allProductions by searchText
            if (!string.IsNullOrEmpty(searchText))
            {
                allProductions = allProductions.Where(e => e.Code.ToLower().Contains(searchText.ToLower()))
                    .Include(e => e.Product)
                    .AsNoTracking()
                    .AsQueryable();
            }

            // If stringDate is not null or empty string,
            // then filter allProductions by stringDate
            if (!string.IsNullOrEmpty(stringDate))
            {
                DateTime productionDate = DateTime.Parse(stringDate);

                allProductions = allProductions.Where(e => e.ProductionDate == productionDate)
                    .Include(e => e.Product)
                    .AsNoTracking()
                    .AsQueryable();
            }

            // If productName is not null or empty striung,
            // then filter allProductions by productName
            if (!string.IsNullOrEmpty(productName))
            {
                allProductions = allProductions.Where(e => e.Product.Name == productName)
                    .Include(e => e.Product)
                    .AsNoTracking()
                    .AsQueryable();
            }

            // Variable that will contain ProductionDto objects
            List<ProductionDto> productionDtos = new();

            // Iterate through allProductions and populate productionDtos
            foreach (var production in allProductions)
            {
                productionDtos.Add(production.ConvertToDto());
            }

            // Using PaginationUtility's GetPaginatedResult method, 
            // return Pagination<ProductionDto> object
            var paginatedResult = PaginationUtility<ProductionDto>.GetPaginatedResult(in productionDtos, pageIndex, pageSize);

            return await Task.FromResult(paginatedResult);
        }

        // Return single ProductionDto object
        public async Task<ProductionDto> GetSingleProductionAsync(int id)
        {
            // Find Production record from database by Primary Key value
            Production? production = await context.Productions
                .Include(e => e.Product)
                .ThenInclude(e => e.ProductDetails)
                .ThenInclude(e => e.Material)
                .FirstOrDefaultAsync(e => e.Id == id);

            return production != null ? production.ConvertToDto() : new ProductionDto();
        }

        // Custom validation
        public async Task<Dictionary<string, string>> ValidateProductionAsync(ProductionDto productionDto)
        {
            // Variable that will contain possible validation errors
            Dictionary<string, string> errors = new();

            // Variable that contains all Production records
            var allProductions = context.Productions
                .Include(e => e.Product)
                .ThenInclude(e => e.ProductDetails)
                .ThenInclude(e => e.Material)
                .AsNoTracking()
                .AsQueryable();

            // If productionDto's Id value is larger than 0
            // it means that Production is used in Edit operation
            if (productionDto.Id > 0)
            {
                // Find Production record from database by Primary Key value
                Production production = (await context.Productions
                    .Include(e => e.Product)
                    .ThenInclude(e => e.ProductDetails)
                    .ThenInclude(e => e.Material)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == productionDto.Id))!;

                // If productionDto's Code value is not equal to production's
                // Code value, it means that user has modified Code value. 
                // Therefore we check for Code uniqueness among all Production records
                if (production.Code != productionDto.Code)
                {
                    // If productionDto's Code value is already contained
                    // in any of the Production records in database,
                    // then add validation error to errors Dictionary
                    if (allProductions.Select(e => e.Code.ToLower()).Contains(productionDto.Code.ToLower()))
                    {
                        errors.Add("Code", "There is already Production with this Code in database. Please provide different Code.");
                    }
                }
            }
            // Otherwise it means that Production is used in Create operation
            else
            {
                // If productionDto's Code value is already contained
                // in any of the Production records in database,
                // then add validation error to errors Dictionary
                if (allProductions.Select(e => e.Code.ToLower()).Contains(productionDto.Code.ToLower()))
                {
                    errors.Add("Code", "There is already Production with this Code in database. Please provide different Code.");
                }
            }

            foreach (var productDetail in allProductions.SelectMany(e => e.Product.ProductDetails))
            {
                if (productDetail.Material.Quantity < productDetail.QtyMaterial*productionDto.Qty)
                {
                    errors.Add("ProductName", "There is not enough material for this production!");
                }
            }

            return errors;
        }

        // Return all Production dates
        public async Task<List<string>> GetAllProductionDatesAsync()
        {
            // Return all Productions
            var allProduictions = context.Productions
                .AsNoTracking()
                .AsQueryable();

            // Variable that will hold Production dates
            List<string> productionDates = new();

            // Iterate through allProductions and populate productionDates
            foreach (var production in allProduictions)
            {
                productionDates.Add(production.ProductionDate.ToShortDateString());
            }

            return await Task.FromResult(productionDates.Distinct().ToList());
        }
    }
}
