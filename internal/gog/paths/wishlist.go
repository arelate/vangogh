package paths

import (
	"github.com/boggydigital/vangogh/internal/cfg"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/paths/dirs"
	"github.com/boggydigital/vangogh/internal/gog/paths/filenames"
	"path"
)

func WishlistDir() string {
	cfg, _ := cfg.Current()

	return path.Join(
		cfg.Dirs.Data,
		dirs.Wishlist)
}

func Wishlist(id int, mt media.Type) string {
	return path.Join(
		WishlistDir(),
		mt.String(),
		filenames.Wishlist(id))
}
