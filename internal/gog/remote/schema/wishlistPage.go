// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package schema

type WishlistPage struct {
	Page
	Products                   []Product   `json:"products"`
	SortBy                     string      `json:"sortBy"`
	TotalProducts              int         `json:"totalProducts"`
	ProductsPerPage            int         `json:"productsPerPage"`
	ContentSystemCompatibility interface{} `json:"contentSystemCompatibility"`
}
