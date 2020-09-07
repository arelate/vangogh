package origin

import "github.com/boggydigital/vangogh/internal/gog/dest"

type PageTransferor interface {
	Getter
	Transferor
	TransferPage(page int, dest dest.Dest) (totalPages int, err error)
}
