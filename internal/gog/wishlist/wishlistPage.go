package wishlist

import "github.com/boggydigital/vangogh/internal/gog/pages"

type WishlistPage struct {
	pages.Page
	Products                   []WishlistProduct `json:"products"`
	SortBy                     string            `json:"sortBy"`
	TotalProducts              int               `json:"totalProducts"`
	ProductsPerPage            int               `json:"productsPerPage"`
	ContentSystemCompatibility interface{}       `json:"contentSystemCompatibility"`
}
