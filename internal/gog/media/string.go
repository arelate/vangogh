package media

func (mt Type) String() string {
	switch mt {
	case Unknown:
		return unknown
	case Game:
		return game
	case Movie:
		return movie
	default:
		return ""
	}
}
