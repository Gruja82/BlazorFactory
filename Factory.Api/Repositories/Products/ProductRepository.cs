using Factory.Api.Data.Entities;
using Factory.Api.Database;
using Factory.Api.Extensions;
using Factory.Api.Utilities;
using Factory.Shared;
using Microsoft.EntityFrameworkCore;

namespace Factory.Api.Repositories.Products
{
    // Implementation class for IProductRepository
    public class ProductRepository:IProductRepository
    {
        private readonly AppDbContext context;

        public ProductRepository(AppDbContext context)
        {
            this.context = context;
        }

        // Create new Product
        public async Task CreateNewProductAsync(ProductDto productDto)
        {
            // Create new Product object
            Product product = new();

            // Set it's properties to the ones contained in productDto
            product.Name = productDto.Name;
            product.Category = (await context.Categories.Where(e => e.Name == productDto.CategoryName).FirstOrDefaultAsync())!;
            product.Quantity = 0;
            product.Price = productDto.Price;

            // Add product to database
            await context.Products.AddAsync(product);

            // Iterate through productDto.ProductDetailsList,
            // set properties of each productDetail to the ones
            // contained in productDto.ProductDetailList,
            // and add each productDetail to database
            foreach (var productDetailDto in productDto.ProductDetailsList)
            {
                ProductDetail productDetail = new();

                productDetail.Product = product;
                productDetail.Material = (await context.Materials.Where(e => e.Name == productDetailDto.MaterialName).FirstOrDefaultAsync())!;
                productDetail.QtyMaterial = productDetailDto.Quantity;

                await context.ProductDetails.AddAsync(productDetail);
            }
        }

        // Delete selected Product
        public async Task DeleteProductAsync(int id)
        {
            // Find Product record from database by Primary Key value
            Product product = (await context.Products
                .Include(e => e.ProductDetails)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id))!;

            // Delete all ProductDetail records
            // contained in product.ProductDetails
            foreach (var productDetail in product.ProductDetails)
            {
                context.ProductDetails.Remove(productDetail);
            }

            // Remove product from database
            context.Products.Remove(product);
        }

        // Edit selected Product
        public async Task EditProductAsync(ProductDto productDto)
        {
            // Find Product record from database by Primary Key value
            Product product = (await context.Products
                .Include(e => e.Category)
                .Include(e => e.ProductDetails)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == productDto.Id))!;

            // Set it's properties to the ones contained in productDto
            product.Name = productDto.Name;
            product.Category = (await context.Categories.Where(e => e.Name == productDto.CategoryName).FirstOrDefaultAsync())!;
            product.Price = productDto.Price;

            // Delete all ProductDetail records
            // contained in product.ProductDetails
            foreach (var productDetail in product.ProductDetails)
            {
                context.ProductDetails.Remove(productDetail);
            }

            // Iterate through productDto.ProductDetailsList,
            // set properties of each productDetail to the ones
            // contained in productDto.ProductDetailList,
            // and add each productDetail to database
            foreach (var productDetailDto in productDto.ProductDetailsList)
            {
                ProductDetail productDetail = new();

                productDetail.Product = product;
                productDetail.Material = (await context.Materials.Where(e => e.Name == productDetailDto.MaterialName).FirstOrDefaultAsync())!;
                productDetail.QtyMaterial = productDetailDto.Quantity;

                await context.ProductDetails.AddAsync(productDetail);
            }
        }

        // Return paginated collection of ProductDto objects
        public async Task<Pagination<ProductDto>> GetProductsCollectionAsync(string? searchText, string? categoryName, int pageIndex, int pageSize)
        {
            // Return all Product objects
            var allProducts = context.Products
                .Include(e => e.Category)
                .Include(e => e.ProductDetails)
                .ThenInclude(e => e.Material)
                .AsNoTracking()
                .AsQueryable();

            // If searchText is not null or empty string,
            // then filter allProducts by searchText
            if (!string.IsNullOrEmpty(searchText))
            {
                allProducts = allProducts.Where(e => e.Name.ToLower().Contains(searchText.ToLower()))
                    .Include(e => e.Category)
                    .Include(e => e.ProductDetails)
                    .ThenInclude(e => e.Material)
                    .AsNoTracking()
                    .AsQueryable();
            }

            // If category is not null or empty string,
            // then filter allProducts by category
            if (!string.IsNullOrEmpty(categoryName))
            {
                allProducts = allProducts.Where(e => e.Category == context.Categories.FirstOrDefault(e => e.Name == categoryName))
                    .Include(e => e.Category)
                    .Include(e => e.ProductDetails)
                    .ThenInclude(e => e.Material)
                    .AsNoTracking()
                    .AsQueryable();
            }

            // Variable that will contain ProductDto objects
            List<ProductDto> productDtos = new();

            // Iterate through allProducts and populate productDtos
            foreach (var product in allProducts)
            {
                productDtos.Add(product.ConvertToDto());
            }

            // Using PaginationUtility's GetPaginatedResult method, 
            // return Pagination<ProductDto> object
            var paginatedResult = PaginationUtility<ProductDto>.GetPaginatedResult(in productDtos, pageIndex, pageSize);

            return await Task.FromResult(paginatedResult);
        }

        // Return single ProductDto object
        public async Task<ProductDto> GetSingleProductAsync(int id)
        {
            // Find Product record from database by Primary Key value
            Product? product = await context.Products
                .Include(e => e.Category)
                .Include(e => e.ProductDetails)
                .ThenInclude(e => e.Material)
                .FirstOrDefaultAsync(e => e.Id == id);

            return product != null ? product.ConvertToDto() : new ProductDto();
        }

        // Custom validation
        public async Task<Dictionary<string, string>> ValidateProductAsync(ProductDto productDto)
        {
            // Variable that will contain possible validation errors
            Dictionary<string, string> errors = new();

            // Variable that contains all Product records
            var allProducts = context.Products
                .Include(e => e.Category)
                .Include(e => e.ProductDetails)
                .AsNoTracking()
                .AsQueryable();

            // If productDto's Id value is larger than 0
            // it means that Product is used in Edit operation
            if (productDto.Id > 0)
            {
                // Find Product record from database by Primary Key value
                Product product = (await context.Products
                    .Include(e => e.Category)
                    .Include(e => e.ProductDetails)
                    .ThenInclude(e => e.Material)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == productDto.Id))!;

                // If productDto's Name value is not equal to product's
                // Name value, it means that user has modified Name value. 
                // Therefore we check for Name uniqueness among all Product records
                if (product.Name != productDto.Name)
                {
                    // If productDto's Name value is already contained
                    // in any of the Product records in database,
                    // then add validation error to errors Dictionary
                    if (allProducts.Select(e => e.Name.ToLower()).Contains(productDto.Name.ToLower()))
                    {
                        errors.Add("Name", "There is already Product with this Name in database. Please provide different Name.");
                    }
                }
            }
            // Otherwise it means that Product is used in Create operation
            else
            {
                // If productDto's Name value is already contained
                // in any of the Product records in database,
                // then add validation error to errors Dictionary
                if (allProducts.Select(e => e.Name.ToLower()).Contains(productDto.Name.ToLower()))
                {
                    errors.Add("Name", "There is already Product with this Name in database. Please provide different Name.");
                }
            }

            // Validate that user has entered positive
            // price value greater than zero
            if (productDto.Price <= 0)
            {
                errors.Add("Price", "Price must be positive value.");
            }

            // Validate that user has entered at least
            // one material in Product's production
            // specification list
            if (productDto.ProductDetailsList.Count <= 0)
            {
                errors.Add("ProductDetailsList", "There must be at least one Material in product's production specification list!");
            }

            return errors;
        }

        // Return all Products
        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            // Return all Product objects
            var allProducts = context.Products
                .AsNoTracking()
                .AsQueryable();

            // Variable that will contain ProductDto objects
            List<ProductDto> productDtos = new();

            // Iterate through allProducts and populate productDtos
            foreach (var product in allProducts)
            {
                productDtos.Add(product.ConvertToDto());
            }

            return await Task.FromResult(productDtos);
        }
    }
}
