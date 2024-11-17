using Factory.Api.Data.Entities;
using Factory.Api.Database;
using Factory.Api.Extensions;
using Factory.Api.Utilities;
using Factory.Shared;
using Microsoft.EntityFrameworkCore;

namespace Factory.Api.Repositories.Materials
{
    // Implementation class for IMaterialRepository
    public class MaterialRepository:IMaterialRepository
    {
        private readonly AppDbContext context;

        public MaterialRepository(AppDbContext context)
        {
            this.context = context;
        }

        // Create new Material
        public async Task CreateNewMaterialAsync(MaterialDto materialDto)
        {
            // Create new Material object
            Material material = new();

            // Set it's properties to the ones contained in materialDto
            material.Name = materialDto.Name;
            material.Category = (await context.Categories.Where(e => e.Name == materialDto.CategoryName).FirstOrDefaultAsync())!;
            material.Quantity = 0;
            material.Price = materialDto.Price;

            // Add material to database
            await context.Materials.AddAsync(material);
        }

        // Delete selected Material
        public async Task DeleteMaterialAsync(int id)
        {
            // Find Material record from database by Primary Key value
            Material material = (await context.Materials.FindAsync(id))!;

            // Remove material from database
            context.Materials.Remove(material);
        }

        // Edit selected Material
        public async Task EditMaterialAsync(MaterialDto materialDto)
        {
            // Find Material record from database by Primary Key value
            Material material = (await context.Materials.FindAsync(materialDto.Id))!;

            // Set it's property values to the ones contained in materialDto
            material.Name = materialDto.Name;
            material.Category = (await context.Categories.Where(e => e.Name == materialDto.CategoryName).FirstOrDefaultAsync())!;
            material.Price = materialDto.Price;
        }

        // Return paginated collection of MaterialDto objects
        public async Task<Pagination<MaterialDto>> GetMaterialsCollectionAsync(string? searchText, string? categoryName, int pageIndex, int pageSize)
        {
            // Return all Material objects
            var allMaterials = context.Materials
                .Include(e => e.Category)
                .AsNoTracking()
                .AsQueryable();

            // If searchText is not null or empty string,
            // then filter allMaterials by searchText
            if (!string.IsNullOrEmpty(searchText))
            {
                allMaterials = allMaterials.Where(e => e.Name.ToLower().Contains(searchText.ToLower()))
                                .Include(e => e.Category)
                                .AsNoTracking()
                                .AsQueryable();
            }

            // If category is not null or empty string,
            // then filter allMaterials by category
            if (!string.IsNullOrEmpty(categoryName))
            {
                allMaterials = allMaterials.Where(e => e.Category == context.Categories.FirstOrDefault(e => e.Name == categoryName))
                                .Include(e => e.Category)
                                .AsNoTracking()
                                .AsQueryable();
            }

            // Variable that will contain MaterialDto objects
            List<MaterialDto> materialDtos = new();

            // Iterate through allMaterials and populate materialDtos
            foreach (var material in allMaterials)
            {
                materialDtos.Add(material.ConvertToDto());
            }

            // Using PaginationUtility's GetPaginatedResult method, 
            // return Pagination<MaterialDto> object
            var paginatedResult = PaginationUtility<MaterialDto>.GetPaginatedResult(in materialDtos, pageIndex, pageSize);

            return await Task.FromResult(paginatedResult);
        }

        // Return single MaterialDto object
        public async Task<MaterialDto> GetSingleMaterialAsync(int id)
        {
            // Find Material record from database by Primary Key value
            Material? material = await context.Materials
                                .Include(e => e.Category)
                                .FirstOrDefaultAsync(e => e.Id == id);

            return material != null ? material.ConvertToDto() : new MaterialDto();
        }

        // Custom validation
        public async Task<Dictionary<string, string>> ValidateMaterialAsync(MaterialDto materialDto)
        {
            // Variable that will contain possible validation errors
            Dictionary<string, string> errors = new();

            // Variable that contains all Material records
            var allMaterials = context.Materials
                             .Include(e => e.Category)
                             .AsNoTracking()
                             .AsQueryable();

            // If materialDto's Id value is larger than 0
            // it means that Material is used in Edit operation
            if (materialDto.Id > 0)
            {
                // Find Material record from database by Primary Key value
                Material material = (await context.Materials.Include(e => e.Category)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == materialDto.Id))!;

                // If materialDto's Name value is not equal to material's
                // Name value, it means that user has modified Name value. 
                // Therefore we check for Name uniqueness among all Material records
                if (material!.Name != materialDto.Name)
                {
                    // If materialDto's Name value is already contained
                    // in any of the Material records in database,
                    // then add validation error to errors Dictionary
                    if (allMaterials.Select(e => e.Name.ToLower()).Contains(materialDto.Name.ToLower()))
                    {
                        errors.Add("Name", "There is already Material with this Name in database. Please provide different Name.");
                    }
                }
            }
            // Otherwise it means that Material is used in Create operation
            else
            {
                // If materialDto's Name value is already contained
                // in any of the Material records in database,
                // then add validation error to errors Dictionary
                if (allMaterials.Select(e => e.Name.ToLower()).Contains(materialDto.Name.ToLower()))
                {
                    errors.Add("Name", "There is already Material with this Name in database. Please provide different Name.");
                }
            }

            // Validate that user has entered positive
            // price value greater than zero
            if (materialDto.Price <= 0)
            {
                errors.Add("Price", "Price must be positive value.");
            }

            return errors;
        }

        // Return all Materials
        public async Task<List<MaterialDto>> GetAllMaterialsAsync()
        {
            // Return all Material records
            var allMaterials = context.Materials
                .Include(e => e.Category)
                .AsNoTracking()
                .AsQueryable();

            // Variable that will hold MaterialDto objects
            List<MaterialDto> materialDtos = new();

            // Iterate through allMaterials and populate
            // materialDtos using Material's extension
            // method ConvertToDto
            foreach (var material in allMaterials)
            {
                materialDtos.Add(material.ConvertToDto());
            }

            return await Task.FromResult(materialDtos);
        }
    }
}
