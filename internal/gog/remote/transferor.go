package remote

import (
	"github.com/boggydigital/vangogh/internal/gog/local"
)

type Transferor interface {
	Transfer(id int, setter local.Setter) error
}
