package remote

import (
	"errors"
	"fmt"
	"github.com/boggydigital/vangogh/internal/gog/local"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"net/http"
)

type PageTransferor interface {
	TransferPage(page int, setter local.Setter, itemSet func(int)) (totalPages int, err error)
}

func GetPageTransferorByName(name string, mt media.Type, httpClient *http.Client) (PageTransferor, error) {
	switch name {
	case ProductsAlias:
		fallthrough
	case ProductsName:
		return NewProductPage(httpClient, mt), nil
	case AccountProductsAlias:
		fallthrough
	case AccountProductsName:
		return NewAccountProductPage(httpClient, mt), nil
	case WishlistAlias:
		fallthrough
	case WishlistName:
		return NewWishlistPage(httpClient, mt), nil
	default:
		return nil, errors.New(fmt.Sprintf("unknown source: %s", name))
	}

}
