package urls

const (
	// GOG.com API endpoints
	HttpsScheme = "https"
	GogHost     = "gog.com"
	AuthHost    = "auth." + GogHost
	LoginHost   = "login." + GogHost
	MenuHost    = "menu." + GogHost
	// Paths
	AuthPath                = "/auth"
	LoginCheckPath          = "/login_check"
	LoginTwoStepPath        = "/login/two_step"
	GameDetailsPath         = "/account/gameDetails/"
	ProductPagesPath        = "/games/ajax/filtered"
	AccountProductPagesPath = "/account/getFilteredProducts"
	// URLs
	UserDataURL  = HttpsScheme + "://www." + GogHost + "/userData.json"
	ReCaptchaURL = "https://www.recaptcha.net/recaptcha/api.js"
)
