package cookies

import (
	"github.com/arelate/gog_urls"
	"net/url"
)

var gogHost = &url.URL{Scheme: gog_urls.HttpsScheme, Host: gog_urls.GogHost}
