package schema

type WishlistPage struct {
	Page
	Products                   []Product   `json:"products"`
	SortBy                     string      `json:"sortBy"`
	TotalProducts              int         `json:"totalProducts"`
	ProductsPerPage            int         `json:"productsPerPage"`
	ContentSystemCompatibility interface{} `json:"contentSystemCompatibility"`
}
