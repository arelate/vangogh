package origin

import "github.com/boggydigital/vangogh/internal/gog/dest"

type Transferor interface {
	Transfer(id int, dest *dest.Dest) error
}
