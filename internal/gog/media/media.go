package media

type Type int

const (
	Game Type = iota + 1
	Movie
)

func (mt Type) String() string {
	switch mt {
	case Game:
		return "game"
	case Movie:
		return "movie"
	default:
		return ""
	}
}
