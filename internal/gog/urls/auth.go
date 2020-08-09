package urls

import "net/url"

func Auth(host string) *url.URL {

	const (
		clientId     = "46755278331571209"
		redirectUri  = "https://www.gog.com/on_login_success"
		responseType = "code"
		layout       = "default"
		brand        = "gog"
		gogLc        = "en-US"
	)

	authURL := url.URL{
		Scheme: HttpsScheme,
		Host:   host,
		Path:   AuthPath,
	}

	q := authURL.Query()
	q.Set("client_id", clientId)
	q.Set("redirect_uri", redirectUri)
	q.Set("response_type", responseType)
	q.Set("layout", layout)
	q.Set("brand", brand)
	q.Set("gog_lc", gogLc)
	authURL.RawQuery = q.Encode()

	return &authURL
}
