package urls

import "net/url"

const (
	// GOG.com API endpoints
	HttpsScheme      = "https"
	GogHost          = "gog.com"
	AuthHost         = "auth." + GogHost
	LoginHost        = "login." + GogHost
	MenuHost         = "menu." + GogHost
	AuthPath         = "/auth"
	LoginCheckPath   = "/login_check"
	LoginTwoStepPath = "/login/two_step"
	UserDataURL      = HttpsScheme + "://www." + GogHost + "/userData.json"
)

func AuthURL(host string) *url.URL {

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
