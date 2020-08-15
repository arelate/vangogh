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
	DetailsPath             = "/account/{mediaType}Details/"
	ProductPagesPath        = "/games/ajax/filtered"
	AccountProductPagesPath = "/account/getFilteredProducts"
	WishlistPath            = "/account/wishlist/search"
	// URLs
	UserDataURL  = HttpsScheme + "://www." + GogHost + "/userData.json"
	ReCaptchaURL = "https://www.recaptcha.net/recaptcha/api.js"
)
