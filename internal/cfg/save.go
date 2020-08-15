package cfg

import (
	"github.com/boggydigital/vangogh/internal/storage"
)

// TODO: Do we even need to save config?
func Save(cfg Config) error {
	return storage.Save(cfg, filename)
}
