package paths

import (
	"github.com/boggydigital/vangogh/internal/cfg"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/paths/dirs"
	"github.com/boggydigital/vangogh/internal/gog/paths/filenames"
	"path"
)

func AccountProductDir() string {
	cfg, _ := cfg.Current()

	return path.Join(
		cfg.Dirs.Data,
		dirs.AccountProducts)
}

func AccountProductIndex() string {
	return path.Join(
		AccountProductDir(),
		filenames.Index)
}

func AccountProduct(id int, mt media.Type) string {
	return path.Join(
		AccountProductDir(),
		mt.String(),
		filenames.AccountProduct(id))
}
