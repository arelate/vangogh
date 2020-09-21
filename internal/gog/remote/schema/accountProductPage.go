// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package schema

type AccountProductPage struct {
	Page
	Products                   []AccountProduct `json:"products"`
	ProductsPerPage            int              `json:"productsPerPage"`
	ContentSystemCompatibility interface{}      `json:"contentSystemCompatibility"`
	TotalProducts              int              `json:"totalProducts"`
	MoviesCount                int              `json:"moviesCount"`
	SortBy                     string           `json:"sortBy"`
	UpdatedProductsCount       int              `json:"updatedProductsCount"`
	HiddenUpdatedProductsCount int              `json:"hiddenUpdatedProductsCount"`
	HasHiddenProducts          bool             `json:"hasHiddenProducts"`
	Tags                       []struct {
		ID           string `json:"id"`
		Name         string `json:"name"`
		ProductCount string `json:"productCount"`
	} `json:"tags"`
}
