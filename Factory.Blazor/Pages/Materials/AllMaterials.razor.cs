﻿using Factory.Blazor.Services.Categories;
using Factory.Blazor.Services.Materials;
using Factory.Shared;
using Microsoft.AspNetCore.Components;

namespace Factory.Blazor.Pages.Materials
{
    // Backgroung logic for AllMaterials component
    public partial class AllMaterials
    {
        // Inject IMaterialService
        [Inject]
        private IMaterialService MaterialService { get; set; } = default!;

        // Inject ICategoryService
        [Inject]
        private ICategoryService CategoryService { get; set; } = default!;

        // Inject Navigation Manager
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        // Property that represents collection of MaterialDto objects
        private Pagination<MaterialDto>? MaterialsCollection { get; set; }

        // Field that represents search term
        // used by SearchBarWithCategory component
        private string? _searchText;

        // Field that represents category
        // used by SearchBarWithCategory component
        private string? _category;

        // Field that represents collection
        // of CategoryDto objects used by 
        // SearchWithCategory component
        private List<CategoryDto> _categories = new();

        // Field that represents page number
        // used by PaginationComponent
        private int _pageIndex;

        // Field that represents page size
        // used by PaginationComponent
        private int _pageSize;

        // When component is loaded for the first time,
        // fill MaterialsCollection by invoking MaterialService's
        // method GetMaterialsAsync, and fill _categories collection
        // by invoking CategoryService's method
        // GetAllCategoriesAsync
        protected override async Task OnInitializedAsync()
        {
            MaterialsCollection = (Pagination<MaterialDto>)await MaterialService.GetMaterialsAsync(_searchText, _category, _pageIndex, _pageSize);
            _categories = (List<CategoryDto>)await CategoryService.GetAllCategoriesAsync();
        }

        // Method for handling button click event in Search component
        private async Task OnSearchAsync(string strValue)
        {
            // Set _searchText field value to the value of strValue
            _searchText = strValue;
            // Reset _pageIndex value
            _pageIndex = default!;
            // Fill the MaterialsCollection
            MaterialsCollection = (Pagination<MaterialDto>)await MaterialService.GetMaterialsAsync(_searchText, _category, _pageIndex, _pageSize);
        }

        // Method for handling selection changed event in SearchWithCategory component
        private async Task OnSelectionChangedAsync(string categoryValue)
        {
            _category = categoryValue;
            _pageIndex = default!;
            MaterialsCollection = (Pagination<MaterialDto>)await MaterialService.GetMaterialsAsync(_searchText, _category, _pageIndex, _pageSize);
        }

        // Method for handling PaginationComponent's page number
        // button click event
        private async Task OnPageChangedAsync(int pageNumber)
        {
            // Set _pageIndex field value to the value of pageNumber
            _pageIndex = pageNumber;
            // Fill the MaterialsCollection
            MaterialsCollection = (Pagination<MaterialDto>)await MaterialService.GetMaterialsAsync(_searchText, _category, _pageIndex, _pageSize);
        }

        // Method for handling PaginationComponent's page size
        // button click event
        private async Task OnPageSizeChangedAsync(int pageSize)
        {
            // Reset _pageIndex value
            _pageIndex = default!;
            // If pageSize value is larger than 0 (zero),
            // then set _pageSize value to the value of pageSize.
            // Otherwise, set _pageSize value to 4
            int pageValue = pageSize > 0 ? pageSize : 4;
            _pageSize = pageValue;
            // Fill the MaterialsCollection
            MaterialsCollection = (Pagination<MaterialDto>)await MaterialService.GetMaterialsAsync(_searchText, _category, _pageIndex, _pageSize);
        }

        // Method for navigating to page for creating new Material
        private void NavigateToCreatePage()
        {
            NavManager.NavigateTo("/material");
        }

        // Method for navigating to page for editing selected Material
        private void NavigateToEditPage(int id)
        {
            NavManager.NavigateTo($"/material/{id}");
        }
    }
}
