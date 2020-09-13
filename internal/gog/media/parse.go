package media

func Parse(mt string) Type {
	switch mt {
	// Section: game
	case "g":
		fallthrough
	case "game":
		fallthrough
	case "games":
		return Game
	// Section: movie
	case "m":
		fallthrough
	case "movie":
		fallthrough
	case "movies":
		return Movie
	// Section: unknown
	default:
		return Unknown
	}
}
