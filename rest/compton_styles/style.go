package compton_styles

import (
	"embed"
	_ "embed"
)

var (
	//go:embed "style/*.css"
	VangoghStyles embed.FS
)
