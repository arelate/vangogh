package index

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/storage"
)

func Save(indexes *map[int]Index, filename string) error {
	bytes, err := json.Marshal(indexes)
	if err != nil {
		return err
	}
	return storage.Write(bytes, filename)
}
