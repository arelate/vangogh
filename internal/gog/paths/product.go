package paths

import (
	"github.com/boggydigital/vangogh/internal/cfg"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/paths/dirs"
	"github.com/boggydigital/vangogh/internal/gog/paths/filenames"
	"path"
)

func ProductDir() string {
	cfg, _ := cfg.Current()

	return path.Join(
		cfg.Dirs.Data,
		dirs.Products)
}

func Product(id int, mt media.Type) string {
	return path.Join(
		ProductDir(),
		mt.String(),
		filenames.Product(id))
}

func ProductIndex() string {
	return path.Join(
		ProductDir(),
		filenames.Index)
}
