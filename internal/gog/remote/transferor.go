package remote

import (
	"github.com/boggydigital/vangogh/internal/gog/local"
)

type Transferor interface {
	Transfer(id int, dest *local.Dest) error
}
