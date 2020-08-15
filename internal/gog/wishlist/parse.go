package wishlist

import (
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/parse"
	"github.com/boggydigital/vangogh/internal/gog/paths"
)

func Parse(filename string) (int, media.Type, error) {
	return parse.Parse(filename, paths.WishlistDir())
}
