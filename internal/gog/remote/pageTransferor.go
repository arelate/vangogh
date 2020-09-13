package remote

import "github.com/boggydigital/vangogh/internal/gog/local"

type PageTransferor interface {
	TransferPage(page int, setter local.Setter, itemSet func(int)) (totalPages int, err error)
}
