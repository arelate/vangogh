package products

import (
	"github.com/boggydigital/vangogh/internal/gog/index"
	"time"
)

func Modified(after, before time.Time) *[]int {
	return index.Modified(indexes, after, before)
}
