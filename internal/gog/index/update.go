package index

import (
	"github.com/boggydigital/vangogh/internal/gog/media"
	"time"
)

func Update(indexes map[int]Index, id int, mt media.Type, hash string) bool {

	if ind, ok := indexes[id]; ok {
		if ind.Hash != hash {
			ind.Hash = hash
			ind.Modified = time.Now().Unix()
			indexes[id] = ind
			return true
		}
		return false
	} else {
		indexes[id] = Index{
			MediaType: mt,
			Hash:      hash,
			Created:   time.Now().Unix(),
			Modified:  time.Now().Unix(),
		}
		return true
	}
}
