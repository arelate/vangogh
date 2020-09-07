package urls

import (
	"github.com/boggydigital/vangogh/internal/gog/media"
	"net/url"
	"strings"
)

func Details(mt media.Type) *url.URL {
	path := strings.Replace(DetailsPath, "{mediaType}", mt.String(), 1)
	return &url.URL{
		Scheme: HttpsScheme,
		Host:   GogHost,
		Path:   path + "{id}.json",
	}
}
