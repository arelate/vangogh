package paths

import (
	"github.com/boggydigital/vangogh/internal/cfg"
	"github.com/boggydigital/vangogh/internal/gog/paths/dirs"
	"github.com/boggydigital/vangogh/internal/gog/paths/filenames"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"path"
)

func GameDetails(id int) string {
	cfg, _ := cfg.Current()

	return path.Join(
		cfg.Dirs.Data,
		dirs.GameDetails,
		filenames.GameDetails(id))
}

func Product(id int, mediaType urls.MediaType) string {
	cfg, _ := cfg.Current()

	return path.Join(
		cfg.Dirs.Data,
		dirs.Products,
		mediaType.String(),
		filenames.Product(id))
}
