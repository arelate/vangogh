package wishlist

import (
	"github.com/boggydigital/vangogh/internal/gog/pages"
	"github.com/boggydigital/vangogh/internal/gog/products"
)

type WishlistPage struct {
	pages.Page
	Products                   []products.Product `json:"products"`
	SortBy                     string             `json:"sortBy"`
	TotalProducts              int                `json:"totalProducts"`
	ProductsPerPage            int                `json:"productsPerPage"`
	ContentSystemCompatibility interface{}        `json:"contentSystemCompatibility"`
}
