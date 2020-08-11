package accountProducts

import (
	"github.com/boggydigital/vangogh/internal/gog/pages"
)

type AccountProductPage struct {
	pages.Page
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
