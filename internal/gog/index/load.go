package index

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/storage"
)

func Load(filename string, indexes *map[int]Index) error {
	bytes, err := storage.Load(filename)
	if err != nil {
		return err
	}
	if len(bytes) > 0 {
		return json.Unmarshal(bytes, &indexes)
	} else {
		*indexes = make(map[int]Index)
		return nil
	}
}
