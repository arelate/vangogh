package urls

import "net/url"

func LoginTwoStep() *url.URL {
	return &url.URL{
		Scheme: HttpsScheme,
		Host:   LoginHost,
		Path:   LoginTwoStepPath,
	}
}
