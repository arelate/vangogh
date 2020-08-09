package urls

import (
	"fmt"
	"github.com/boggydigital/vangogh/internal/gog/paths/filenames"
	"net/url"
)

func GameDetails(id int) *url.URL {
	return &url.URL{
		Scheme: HttpsScheme,
		Host:   GogHost,
		Path:   fmt.Sprintf(GameDetailsPath + filenames.GameDetails(id)),
	}
}
