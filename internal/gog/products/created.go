package products

import (
	"github.com/boggydigital/vangogh/internal/gog/index"
	"time"
)

func Created(after, before time.Time) *[]int {
	return index.Created(indexes, after, before)
}
