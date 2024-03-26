// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

namespace Adventure.Abstractions.Grains;

public interface IInventoryGrain : IGrainWithStringKey
{
    Task<HashSet<ProductDetails>> GetAllProductsAsync();

    Task AddOrUpdateProductAsync(ProductDetails productDetails);

    Task RemoveProductAsync(string productId);
}
