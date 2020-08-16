package paths

import (
	"github.com/boggydigital/vangogh/internal/cfg"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/paths/dirs"
	"github.com/boggydigital/vangogh/internal/gog/paths/filenames"
	"path"
)

func DetailsDir() string {
	cfg, _ := cfg.Current()

	return path.Join(
		cfg.Dirs.Data,
		dirs.Details)
}

func DetailsIndex() string {
	return path.Join(
		DetailsDir(),
		filenames.Index)
}

func Details(id int, mt media.Type) string {
	return path.Join(
		DetailsDir(),
		mt.String(),
		filenames.Details(id))
}
