package remote

import "github.com/boggydigital/vangogh/internal/gog/local"

type PageTransferor interface {
	TransferPage(page int, dest local.Dest) (totalPages int, err error)
}
