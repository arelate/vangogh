package urls

import (
	"fmt"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/paths/filenames"
	"net/url"
	"strings"
)

func Details(id int, mt media.Type) *url.URL {
	path := strings.Replace(DetailsPath, "{mediaType}", mt.String(), 1)
	return &url.URL{
		Scheme: HttpsScheme,
		Host:   GogHost,
		Path:   fmt.Sprintf(path + filenames.Details(id)),
	}
}
