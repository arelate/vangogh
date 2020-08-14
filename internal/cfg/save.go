package cfg

import (
	"github.com/boggydigital/vangogh/internal/storage"
)

func Save(cfg Config) error {
	return storage.Save(cfg, filename)
}
