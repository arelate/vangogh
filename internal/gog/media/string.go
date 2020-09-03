package media

func (mt Type) String() string {
	switch mt {
	case Unknown:
		return "unknown"
	case All:
		return "all"
	case Game:
		return "game"
	case Movie:
		return "movie"
	default:
		return ""
	}
}
