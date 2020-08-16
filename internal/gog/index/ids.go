package index

import "github.com/boggydigital/vangogh/internal/gog/media"

func Ids(indexes map[int]Index, mt media.Type) *[]int {
	ids := make([]int, 0, len(indexes))
	for id := range indexes {
		if index, ok := indexes[id]; ok && (index.MediaType == mt) {
			ids = append(ids, id)
		}
	}
	return &ids
}
