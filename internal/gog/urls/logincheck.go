package urls

import "net/url"

func LoginCheck() *url.URL {
	return &url.URL{
		Scheme: HttpsScheme,
		Host:   LoginHost,
		Path:   LoginCheckPath,
	}
}
