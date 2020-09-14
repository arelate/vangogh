package media

func Parse(mt string) Type {
	switch mt {
	// game
	case gameAbbr:
		fallthrough
	case game:
		fallthrough
	case games:
		return Game
	// movie
	case movieAbbr:
		fallthrough
	case movie:
		fallthrough
	case movies:
		return Movie
	// unknown
	default:
		return Unknown
	}
}
