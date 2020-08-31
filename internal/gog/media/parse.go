package media

func Parse(mt string) Type {
	switch mt {
	case "all":
		return All
	case "game":
		return Game
	case "games":
		return Game
	case "movie":
		return Movie
	case "movies":
		return Movie
	default:
		return Unknown
	}
}
