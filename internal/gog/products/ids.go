package products

import (
	"github.com/boggydigital/vangogh/internal/gog/index"
	"github.com/boggydigital/vangogh/internal/gog/media"
)

func Ids(mt media.Type) *[]int {
	return index.Ids(indexes, mt)
}
